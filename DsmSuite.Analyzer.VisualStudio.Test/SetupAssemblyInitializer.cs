using System;
using System.Reflection;
using DsmSuite.Common.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.VisualStudio.Test
{
    [TestClass]
    public class SetupAssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // https://github.com/dotnet/msbuild/issues/2369#issuecomment-323153193
            Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe");

            Logger.Init(Assembly.GetExecutingAssembly(), true);
        }
    }
}
