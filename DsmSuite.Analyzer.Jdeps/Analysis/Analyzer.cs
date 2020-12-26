using System;
using System.IO;
using DsmSuite.Analyzer.Jdeps.Settings;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Jdeps.Analysis
{
    /// <summary>
    /// Java code analyzer which uses Java SDK 8 jdeps tools to analyze dependencies between types.
    /// </summary>
    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IProgress<ProgressInfo> _progress;

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _progress = progress;
        }

        public void Analyze()
        {
            int lineNumber = 0;
            FileInfo dotFile = new FileInfo(_analyzerSettings.Input.Filename);
            using (FileStream stream = dotFile.Open(FileMode.Open))
            {
                StreamReader sr = new StreamReader(stream);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lineNumber++;
                    UpdateLineProgress(lineNumber, false);

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
            }
            UpdateLineProgress(lineNumber, true);
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
            _model.AddElement(consumerName, "", null);
            _model.AddElement(providerName, "", null);
            _model.AddRelation(consumerName, providerName, "dependency", 1, null);
        }

        private void UpdateLineProgress(int lineNumber, bool done)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = "Parse lines";
            progressInfo.CurrentItemCount = lineNumber;
            progressInfo.TotalItemCount = 0;
            progressInfo.ItemType = "lines";
            progressInfo.Percentage = null;
            progressInfo.Done = done;
            _progress?.Report(progressInfo);
        }
    }
}
