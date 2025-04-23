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

                    RegisterElement(moduleName);
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
            foreach (Dependency dependency in module.Dependencies)
            {
                string consumerName = $"{packageName}.{module.Name}";

                Module? providerModule = FindProviderModule(dependency);

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
                        RegisterRelation(consumerName, providerName, dependency.Lineno);
                    }
                }
                else
                {
                    string providerName = $"External.{dependency.Target}";
                    RegisterElement(providerName);

                    RegisterRelation(consumerName, providerName, dependency.Lineno);
                }
            }
        }

        private Module? FindProviderModule(Dependency dependency)
        {
            Module? provider = null;

            string fullTargetName = dependency.Target;
            provider = FindTargetModule(fullTargetName);

            if (provider == null)
            {
                int lastDot = dependency.Target.LastIndexOf('.');
                if (lastDot != -1)
                {
                    string partialTargetName = dependency.Target.Substring(0, lastDot);
                    provider = FindTargetModule(partialTargetName);
                }
            }

            return provider;
        }

        private Module? FindTargetModule(string targetName)
        {
            Module? provider = null;

            foreach (string key in _modules.Keys)
            {
                if (key.EndsWith(targetName))
                {
                    provider = _modules[key];
                }
            }

            if (provider != null)
            {
                Logger.LogInfo($"Found {targetName}");
            }
            else
            {
                Logger.LogInfo($"Not found {targetName}");
            }

            return provider;
        }


        private void RegisterElement(string moduleName)
        {
            Logger.LogInfo($"Added element name={moduleName}");
            _model.AddElement(moduleName, "module", null);
        }

        private void RegisterRelation(string consumerName, string providerName, int line)
        {
            Dictionary<string, string> relationProperties = new Dictionary<string, string>();
            relationProperties["line"] = line.ToString();

            Logger.LogInfo($"Added relation from consumer='{consumerName}' to provoider='{providerName}'");
            _model.AddRelation(consumerName, providerName, "dependency", 1, relationProperties);
        }
    }
}
