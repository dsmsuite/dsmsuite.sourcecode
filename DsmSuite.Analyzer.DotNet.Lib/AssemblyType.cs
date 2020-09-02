namespace DsmSuite.Analyzer.DotNet.Lib
{
    public class AssemblyType
    {
        public AssemblyType(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public string Type { get; }
    }
}
