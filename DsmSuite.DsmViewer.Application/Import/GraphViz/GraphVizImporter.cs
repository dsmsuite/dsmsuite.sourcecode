using System;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Import.Common;
using System.IO;

namespace DsmSuite.DsmViewer.Application.Import.GraphViz
{
    public class GraphVizImporter : ImporterBase
    {
        private readonly string _dotFilename;
        private readonly IDsmModel _dsmModel;
        private readonly IImportPolicy _importPolicy;
        private readonly bool _autoPartition;

        public GraphVizImporter(string dotFilename, IDsmModel dsmModel, IImportPolicy importPolicy, bool autoPartition) : base(dsmModel)
        {
            _dotFilename = dotFilename;
            _dsmModel = dsmModel;
            _importPolicy = importPolicy;
            _autoPartition = autoPartition;
        }

        public void Import(IProgress<ProgressInfo> progress)
        {
            int lineNumber = 0;
            FileInfo dotFile = new FileInfo(_dotFilename);
            using (FileStream stream = dotFile.Open(FileMode.Open))
            {
                StreamReader sr = new StreamReader(stream);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lineNumber++;
                    UpdateLineProgress(lineNumber, false, progress);

                    if (line.Contains("->"))
                    {
                        string[] items = line.Split('"');
                        if (items.Length == 5)
                        {
                            string consumer = ReplaceNestedClassMarker(items[1]);
                            string provider = ReplaceNestedClassMarker(RemoveTrailingText(items[3]));

                            RegisterRelation(consumer, provider);
                        }
                    }
                }
                UpdateLineProgress(lineNumber, true, progress);
            }

            if (_autoPartition)
            {
                Partition(progress);
            }
        }

        private string RemoveTrailingText(string provider)
        {
            // example "sun.security.util.Debug (JDK internal API (rt.jar))"
            char[] separators = { ' ' };
            string[] elements = provider.Split(separators);
            return elements[0];
        }

        private string ReplaceNestedClassMarker(string className)
        {
            // example "javax.crypto.Cipher$Transform"
            return className.Replace("$", ".");
        }

        private void RegisterRelation(string consumerName, string providerName)
        {
            IDsmElement consumer = _dsmModel.AddElement(consumerName, "", null);
            IDsmElement provider = _dsmModel.AddElement(providerName, "", null);
            _dsmModel.AddRelation(consumer, provider, "dependency", 1);
        }

        private void UpdateLineProgress(int lineNumber, bool done, IProgress<ProgressInfo> progress)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = "Parse lines";
            progressInfo.CurrentItemCount = lineNumber;
            progressInfo.TotalItemCount = 0;
            progressInfo.ItemType = "lines";
            progressInfo.Percentage = null;
            progressInfo.Done = done;
            progress?.Report(progressInfo);
        }
    }
}
