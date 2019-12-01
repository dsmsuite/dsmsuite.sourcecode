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
        private readonly IDataModel _model;
        private readonly AnalyzerSettings _analyzerSettings;

        public Analyzer(IDataModel model, AnalyzerSettings analyzerSettings)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
        }

        public void Analyze()
        {
            Logger.LogUserMessage("Analyzing");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            AnalyzeInputFile();

            Logger.LogResourceUsage();

            stopWatch.Stop();
            Logger.LogUserMessage($" total elapsed time={stopWatch.Elapsed}");
        }

        private void AnalyzeInputFile()
        {
            FileInfo dotFile = new FileInfo(_analyzerSettings.InputFilename);
            using (FileStream stream = dotFile.Open(FileMode.Open))
            {
                StreamReader sr = new StreamReader(stream);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
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
    }
}
