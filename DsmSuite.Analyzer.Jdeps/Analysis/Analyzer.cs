using System;
using System.Diagnostics;
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

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
        }

        public void Analyze(IProgress<ProgressInfo> progress)
        {
            int lineNumber = 0;
            FileInfo dotFile = new FileInfo(_analyzerSettings.InputFilename);
            using (FileStream stream = dotFile.Open(FileMode.Open))
            {
                StreamReader sr = new StreamReader(stream);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lineNumber++;
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

                    UpdateProgress(progress, lineNumber, false);
                }
                UpdateProgress(progress, lineNumber, true);
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
            _model.AddElement(consumerName, "", null);
            _model.AddElement(providerName, "", null);
            _model.AddRelation(consumerName, providerName, "dependency", 1, "dot file");
        }

        protected void UpdateProgress(IProgress<ProgressInfo> progress, int lineNumber, bool done)
        {
            if (progress != null)
            {
                ProgressInfo progressInfoInfo = new ProgressInfo
                {
                    ActionText = "Reading input file",
                    TotalItemCount = 0,
                    CurrentItemCount = lineNumber,
                    ItemType = "lines",
                    Percentage = null,
                    Done = done
                };

                progress.Report(progressInfoInfo);
            }
        }

    }
}
