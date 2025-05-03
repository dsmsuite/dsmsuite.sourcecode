using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Import.Common;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.Data.Sqlite;
using Dapper;
using System;
using SQLitePCL;

namespace DsmSuite.DsmViewer.Application.Import.Dsi
{
    public class SqlImporter : ImporterBase
    {
        private readonly string _databaseFilename;
        private readonly IDsmModel _dsmModel;
        private readonly bool _autoPartition;
        private static bool batteriesInitDone = false;

        public class Node
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string Name { get; set; }
            public int NodeTypeId { get; set; }
            public int? SourceFileId { get; set; }
            public int? BeginLineNo { get; set; }
            public int? EndLineNo { get; set; }
            public int? Complexity { get; set; }
            public string Documentation { get; set; }
            public string Annotation { get; set; }
        }

        public class Edge
        {
            public int Id { get; set; }
            public int SourceId { get; set; }
            public int TargetId { get; set; }
            public int EdgeTypeId { get; set; }
            public int Strength { get; set; }
            public int? SourceFileId { get; set; }
            public int? LineNo { get; set; }
            public string Annotation { get; set; }
        }

        public class NodeType
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class EdgeType
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public SqlImporter(string databaseFilename, IDsmModel dsmModel, bool autoPartition) : base(dsmModel)
        {
            _databaseFilename = databaseFilename;
            _dsmModel = dsmModel;
            _autoPartition = autoPartition;

            if (!batteriesInitDone)
            {
                Batteries.Init();
                batteriesInitDone = true;
            }
        }

        public void Import(IProgress<ProgressInfo> progress)
        {
            using (var connection = new SqliteConnection($"Data Source={_databaseFilename}"))
            {
                Dictionary<int, string> nodeTypes = new Dictionary<int, string>();
                Dictionary<int, string> edgeTypes = new Dictionary<int, string>();
                IDictionary<int, IDsmElement> elements = new Dictionary<int, IDsmElement>();
                IDictionary<int, int?> parentIds = new Dictionary<int, int?>();

                connection.Open();

                UpdateProgress(progress, "Importing sql", 4, 0);
                QueryNodeTypes(connection, nodeTypes);
                UpdateProgress(progress, "Importing sql", 4, 1);
                QueryEdgeTypes(connection, edgeTypes);
                UpdateProgress(progress, "Importing sql", 4, 2);
                QueryNodes(connection, nodeTypes, elements, parentIds);
                UpdateProgress(progress, "Importing sql", 4, 3);
                QueryEdges(connection, edgeTypes, elements);
                UpdateProgress(progress, "Importing sql", 4, 4);

                BuildHierarchy(elements, parentIds);

                if (_autoPartition)
                {
                    Partition(progress);
                }

                FinalizeImport(progress);
            }
        }

        private static void QueryNodeTypes(SqliteConnection connection, Dictionary<int, string> nodeTypes)
        {
            foreach (NodeType nodeType in connection.Query<NodeType>("SELECT * FROM NodeType"))
            {
                nodeTypes[nodeType.Id] = nodeType.Name;
            }
        }

        private static void QueryEdgeTypes(SqliteConnection connection, Dictionary<int, string> edgeTypes)
        {
            foreach (EdgeType edgeType in connection.Query<EdgeType>("SELECT * FROM EdgeType"))
            {
                edgeTypes[edgeType.Id] = edgeType.Name;
            }
        }

        private void QueryNodes(SqliteConnection connection, Dictionary<int, string> nodeTypes, IDictionary<int, IDsmElement> elements, IDictionary<int, int?> parentIds)
        {
            foreach (Node node in connection.Query<Node>("SELECT * FROM Node"))
            {
                if (nodeTypes.ContainsKey(node.NodeTypeId))
                {
                    elements[node.Id] = _dsmModel.AddElement(node.Id, node.Name, nodeTypes[node.NodeTypeId], null, 0);
                    parentIds[node.Id] = node.ParentId;
                }
            }
        }
        private void QueryEdges(SqliteConnection connection, Dictionary<int, string> edgeTypes, IDictionary<int, IDsmElement> elements)
        {
            foreach (Edge edge in connection.Query<Edge>("SELECT * FROM Edge"))
            {
                if (elements.ContainsKey(edge.SourceId) && elements.ContainsKey(edge.TargetId) && edgeTypes.ContainsKey(edge.EdgeTypeId))
                {
                    _dsmModel.AddRelation(edge.Id, elements[edge.SourceId], elements[edge.TargetId], edgeTypes[edge.EdgeTypeId], edge.Strength, null);
                }
            }
        }

        private static void BuildHierarchy(IDictionary<int, IDsmElement> elements, IDictionary<int, int?> parentIds)
        {
            foreach (IDsmElement element in elements.Values)
            {
                if (parentIds.ContainsKey(element.Id))
                {
                    int? parentId = parentIds[element.Id];
                    if (parentId.HasValue)
                    {
                        if (elements.ContainsKey(parentId.Value))
                        {
                            IDsmElement parentElement = elements[parentId.Value];
                            if (parentElement != null)
                            {
                                parentElement.AllChildren.Add(element);
                            }
                        }
                    }
                }
            }
        }
    }
}
