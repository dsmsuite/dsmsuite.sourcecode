using DsmSuite.Common.Util;
using System.Reflection;

namespace DsmSuite.Analyzer.Transformations.Test
{
    [TestClass]
    public class SetupAssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Logger.Init(Assembly.GetExecutingAssembly(), true);
        }
    }
}
