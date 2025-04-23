using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzers.Python.Settings;
using DsmSuite.Common.Util;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

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
        private readonly Dictionary<string, Module> _modules = new Dictionary<string, Module>();
        private readonly Dictionary<Module, Package> _moduleParentPackages = new Dictionary<Module, Package>();
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private string rootPackageName;

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

                foreach (var rootPackage in root.Packages)
                {
                    FindElements(rootPackage);
                }

                foreach (var rootPackage in root.Packages)
                {
                    FindRelations(rootPackage);
                }
            }
        }

        private void FindElements(Package package)
        {
            foreach (Module module in package.Modules)
            {
                if (module.Name != "__init__")
                {
                    string moduleName = $"{package.Name}.{module.Name}";
                    _modules[moduleName] = module;
                    _moduleParentPackages[module] = package;
                    Console.WriteLine($"Node {moduleName}");
                    _model.AddElement(moduleName, "module", null);
                }
            }

            foreach (var subPackage in package.Packages)
            {
                FindElements(subPackage);
            }
        }

        private void FindRelations(Package package)
        {
            foreach (Module module in package.Modules)
                FindRelations(package.Name, module);

            foreach (Package subPackage in package.Packages)
                FindRelations(subPackage);
        }

        void FindRelations(string packageName, Module module)
        {
            foreach (Dependency dep in module.Dependencies)
            {
                string consumerName = $"{packageName}.{module.Name}";

                Module? providerModule = FindProviderModule(dep);

                if (providerModule != null)
                {
                    Package providerPackage = _moduleParentPackages[providerModule];
                    string providerName = $"{providerPackage.Name}.{providerModule.Name}";

                    if (!_modules.ContainsKey(consumerName))
                    {
                        Logger.LogError($"Consumer element '{consumerName}' not found");
                    }
                    else if (!_modules.ContainsKey(providerName))
                    {
                        Logger.LogError($"Provider element '{providerName}' not found");
                    }
                    else
                    {
                        Console.WriteLine($"Edge from {consumerName} to {providerName}");
                        _model.AddRelation(consumerName, providerName, "dependency", 1, null);
                    }
                }
                else
                {
                    string providerName = $"External.{dep.Target}";
                    _model.AddElement(providerName, "module", null);

                    Console.WriteLine($"Edge from {consumerName} to {providerName}");
                    _model.AddRelation(consumerName, providerName, "dependency", 1, null);
                }
            }
        }

        private Module? FindProviderModule(Dependency dependency)
        {
            Module? provider = null;

            int lastDot = dependency.Target.LastIndexOf('.');
            if (lastDot != -1)
            {
                string moduleName = dependency.Target.Substring(0, lastDot);

                foreach (string key in _modules.Keys)
                {
                    if (key.EndsWith(moduleName))
                    {
                        provider = _modules[key];
                    }
                }
            }

            return provider;
        }
    }
}
