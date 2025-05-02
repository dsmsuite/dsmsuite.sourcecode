using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Import.Common;
using DsmSuite.DsmViewer.Model.Interfaces;
using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;

namespace DsmSuite.DsmViewer.Application.Import.Dsi
{
    public class SqlImporter
    {
        private readonly string _filename;
        private readonly IDsmBuilder _importPolicy;
        private readonly bool _autoPartition;
        private readonly Dictionary<int, int> _dsiToDsmMapping;

        public class AnalysisRunInfo
        {
            public int Id { get; set; }
            public DateTime Timestamp { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }
            public string SourceCodeLanguage { get; set; }
        }

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

        public class SourceFile
        {
            public int Id { get; set; }
            public string Filename { get; set; }
        }


        public SqlImporter(string filename, IDsmModel dsmModel, IDsmBuilder importPolicy, bool autoPartition) 
        {
            _filename = filename;
            _importPolicy = importPolicy;
            _autoPartition = autoPartition;
            _dsiToDsmMapping = new Dictionary<int, int>();
        }

        public void Import(IProgress<ProgressInfo> progress)
        {
            using (var connection = new SqliteConnection($"Data Source={_filename}"))
            {
                connection.Open();

                var runs = connection.Query<AnalysisRunInfo>("SELECT * FROM AnalysisRunInfo");
                var nodes = connection.Query<Node>("SELECT * FROM Node");
                var edges = connection.Query<Edge>("SELECT * FROM Edge");
                var nodeTypes = connection.Query<NodeType>("SELECT * FROM NodeType");
                var edgeTypes = connection.Query<EdgeType>("SELECT * FROM EdgeType");
                var sourceFiles = connection.Query<SourceFile>("SELECT * FROM SourceFile");
            }

            _importPolicy.FinalizeImport(progress);
        }
    }
}
