namespace DsmSuite.Analyzer.DotNet.Test.Util
{
    class TestData
    {
        public static string RootDirectory
        {
            get
            {
                string testData = "";
                string pathExecutingAssembly = AppDomain.CurrentDomain.BaseDirectory;
                return Path.GetFullPath(Path.Combine(pathExecutingAssembly, testData));
            }
        }
    }
}
