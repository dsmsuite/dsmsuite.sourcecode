using System;
using System.IO;

namespace DsmSuite.Analyzer.DotNet.Test.Util
{
    class TestData
    {
        public static string RootDirectory
        {
            get
            {
                string testData = @"..\..\DsmSuite.Analyzer.DotNet.Test.Data\bin";
                string pathExecutingAssembly = AppDomain.CurrentDomain.BaseDirectory;
                return Path.GetFullPath(Path.Combine(pathExecutingAssembly, testData));
            }
        }
    }
}
