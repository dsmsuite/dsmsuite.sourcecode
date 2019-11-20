using System;
using System.IO;

namespace DsmSuite.Analyzer.VisualStudio.Test.Util
{
    class TestData
    {
        public static string TestDataDirectory
        {
            get
            {
                string executableDir = AppDomain.CurrentDomain.BaseDirectory;
                return Path.GetFullPath(Path.Combine(executableDir, "../../DsmSuite.Analyzer.VisualStudio.Test.Data"));
            }
        }

        public static string SolutionDirectory
        {
            get
            {
                string executableDir = AppDomain.CurrentDomain.BaseDirectory;
                return Path.GetFullPath(Path.Combine(executableDir, "../../"));
            }
        }
    }
}
