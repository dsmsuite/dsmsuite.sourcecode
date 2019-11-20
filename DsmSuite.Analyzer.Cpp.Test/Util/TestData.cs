using System;
using System.IO;

namespace DsmSuite.Analyzer.Cpp.Test.Util
{
    class TestData
    {
        public static string RootDirectory
        {
            get
            {
                string pathExecutingAssembly = AppDomain.CurrentDomain.BaseDirectory;
                return Path.GetFullPath(Path.Combine(pathExecutingAssembly, @"..\..\DsmSuite.Analyzer.Cpp.Test.Data"));
            }
        }
    }
}
