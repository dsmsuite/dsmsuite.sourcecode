namespace DsmSuite.Analyzer.Cpp.Test.Util
{
    class TestData
    {
        public static string RootDirectory
        {
            get
            {
                // Assemblies in build\Release\net8.0 or 
                string pathExecutingAssembly = AppDomain.CurrentDomain.BaseDirectory;
                return Path.GetFullPath(Path.Combine(pathExecutingAssembly, @"..\..\..\DsmSuite.Analyzer.Cpp.Test.Data"));
            }
        }
    }
}
