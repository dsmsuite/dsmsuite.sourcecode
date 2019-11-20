using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Util.Test
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void TestReplaceNothing()
        {
            Assert.AreEqual("abc", "abc".ReplaceIgnoreCase("d", "e"));
        }

        [TestMethod]
        public void TestReplaceLowerCase()
        {
            Assert.AreEqual("xyzdef", "abcdef".ReplaceIgnoreCase("abc", "xyz"));
        }

        [TestMethod]
        public void TestReplaceUpperCase()
        {
            Assert.AreEqual("xyzdef", "ABCdef".ReplaceIgnoreCase("abc", "xyz"));
        }

        [TestMethod]
        public void TestReplaceRegex()
        {
            Assert.AreEqual("xyzdef", "A*BCdef".ReplaceIgnoreCase("a*bc", "xyz"));
            Assert.AreEqual("x*yzdef", "abcdef".ReplaceIgnoreCase("abc", "x*yz"));
            Assert.AreEqual("$def", "abcdef".ReplaceIgnoreCase("abc", "$"));
        }
    }
}
