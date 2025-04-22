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
    
                foreach (var pkg in root.Packages)
                {
                    FindElements(pkg);
                }

                foreach (var pkg in root.Packages)
                    FindRelations(pkg);
            }
        }

         void FindElements(Package pkg)
        {
            foreach (Module module in pkg.Modules)
            {
                if (module.Name != "__init__")
                {
                    string name = $"{pkg.Name}.{module.Name}";
                    _modules[name] = module;
                    string elementName = $"{pkg.Name}.{module.Name}";

                    _model.AddElement(elementName, "module", null);
                }
            }

            foreach (var sub in pkg.Packages)
            {
                FindElements(sub);
            }
        }

        void FindRelations(Package pkg)
        {
            foreach (var module in pkg.Modules)
                FindRelations(pkg.Name, module);

            foreach (var sub in pkg.Packages)
                FindRelations(sub);
        }

        void FindRelations(string packageName, Module module)
        {
            foreach (var dep in module.Dependencies)
            {
                // module info
                //   "name": "{moduleName}>",
                //   "path": "{sourcefolder}\\{packageName}\\{namespaceElement}\\{namespaceElement}\\{moduleName}py",

                // dependency info
                //   "target": "{namespace}.{moduleName}.{moduleName}",
                // external seems always true so is not usable

                int lastDot = dep.Target.LastIndexOf('.');
                string depTarget = (lastDot != -1) ? dep.Target.Substring(0, lastDot) : dep.Target;

                string consumerName = $"{packageName}.{module.Name}";
                string providerName = $"{packageName}.{depTarget}";
                Console.WriteLine($"Dep from {consumerName} to {providerName}");

                if (!_modules.ContainsKey(consumerName))
                {
                    Logger.LogError($"Consumer element '{consumerName}' not found");
                }
                else if (!_modules.ContainsKey(providerName))
                {
                    string externalElementName = $"External.{dep.Target}";
                    //_model.AddElement(externalElementName, "module", null);

                    //RegisterRelation(consumerName, externalElementName);
                }
                else
                {
                    RegisterRelation(consumerName, providerName);
                }
            
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
