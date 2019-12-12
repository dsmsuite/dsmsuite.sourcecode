using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Common.Util.Test
{
    [TestClass]
    public class SystemInfoTest
    {
        [TestMethod]
        public void When_GetExecutableInfoIsCalled_Then_InfoContainingTheAssemblyNameIsReturned()
        {
            string executableInfo = SystemInfo.GetExecutableInfo(Assembly.GetExecutingAssembly());
            char[] itemSeparators = {'=', ' '};
            string[] elements = executableInfo.Split(itemSeparators);
            Assert.IsTrue(elements.Length >= 6); // Depending on locale PM/AM might be post fixed

            Assert.AreEqual("DsmSuite.Common.Util.Test", elements[0]);
        }

        [TestMethod]
        public void When_GetExecutableInfoIsCalled_Then_InfoIdentifyingTheAssemblyVersionIsReturned()
        {
            string executableInfo = SystemInfo.GetExecutableInfo(Assembly.GetExecutingAssembly());
            char[] itemSeparators = { '=', ' ' };
            string[] elements = executableInfo.Split(itemSeparators);
            Assert.IsTrue(elements.Length >= 6); // Depending on locale PM/AM might be post fixed

            Assert.AreEqual("version", elements[1]);

            char[] versionNumberSeparators = { '.' };
            string[] versionNumberItems = elements[2].Split(versionNumberSeparators);
            Assert.AreEqual(4, versionNumberItems.Length);

            int versionNumberPart0;
            Assert.IsTrue(int.TryParse(versionNumberItems[0], out versionNumberPart0));
            Assert.AreEqual(1, versionNumberPart0);

            int versionNumberPart1;
            Assert.IsTrue(int.TryParse(versionNumberItems[1], out versionNumberPart1));
            Assert.AreEqual(0, versionNumberPart1);

            int versionNumberPart2;
            Assert.IsTrue(int.TryParse(versionNumberItems[2], out versionNumberPart2));

            int versionNumberPart3;
            Assert.IsTrue(int.TryParse(versionNumberItems[3], out versionNumberPart3));
        }

        [TestMethod]
        public void When_GetExecutableInfoIsCalled_Then_InfoIdentifyingTheAssemblyBuildTimeStampIsReturned()
        {
            string executableInfo = SystemInfo.GetExecutableInfo(Assembly.GetExecutingAssembly());
            char[] itemSeparators = { '=', ' ' };
            string[] elements = executableInfo.Split(itemSeparators);
            Assert.IsTrue(elements.Length >= 6); // Depending on locale PM/AM might be post fixed

            Assert.AreEqual("build", elements[3]);

            DateTime date;
            Assert.IsTrue(DateTime.TryParse(elements[4], out date));

            DateTime time;
            Assert.IsTrue(DateTime.TryParse(elements[5], out time));
        }
    }
}
