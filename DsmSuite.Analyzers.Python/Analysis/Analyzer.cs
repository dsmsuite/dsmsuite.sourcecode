using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzers.Python.Settings;
using DsmSuite.Common.Util;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DsmSuite.Analyzers.Python.Analysis
{
    public class DependencyRoot
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("modules")]
        public List<Module> Modules { get; set; } = new();

        [JsonPropertyName("packages")]
        public List<Package> Packages { get; set; } = new();
    }

    public class Package
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("modules")]
        public List<Module> Modules { get; set; } = new();

        [JsonPropertyName("packages")]
        public List<Package> Packages { get; set; } = new(); // nested packages
    }

    public class Module
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;

        [JsonPropertyName("dependencies")]
        public List<Dependency> Dependencies { get; set; } = new();
    }

    public class Dependency
    {
        [JsonPropertyName("target")]
        public string Target { get; set; } = string.Empty;

        [JsonPropertyName("lineno")]
        public int Lineno { get; set; }

        [JsonPropertyName("what")]
        public string? What { get; set; }

        [JsonPropertyName("external")]
        public bool External { get; set; }
    }

    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
        }

        public void Analyze()
        {
            FileInfo fileInfo = new FileInfo(_analyzerSettings.Input.JsonFilename);
            if (fileInfo.Extension == ".json")
            {
                string content = File.ReadAllText(_analyzerSettings.Input.JsonFilename);
                DependencyRoot root = JsonSerializer.Deserialize<DependencyRoot>(content);

                // top-level modules
                foreach (var module in root.Modules)
                    ProcessModule(root.Name, module);

                // nested packages
                foreach (var pkg in root.Packages)
                    ProcessPackage(pkg);
            }
        }

        void ProcessPackage(Package pkg)
        {
            foreach (var module in pkg.Modules)
                ProcessModule(pkg.Name, module);

            foreach (var sub in pkg.Packages)
                ProcessPackage(sub);
        }

        void ProcessModule(string packageName, Module module)
        {
            foreach (var dep in module.Dependencies)
            {
                string consumerName = $"{packageName}.{module.Name}";
                string providerName = dep.Target;
                RegisterRelation(consumerName, providerName);
            }
        }

        private void RegisterRelation(string consumerName, string providerName)
        {
            _model.AddElement(consumerName, "", null);
            _model.AddElement(providerName, "", null);
            _model.AddRelation(consumerName, providerName, "dependency", 1, null);
        }
    }
}
