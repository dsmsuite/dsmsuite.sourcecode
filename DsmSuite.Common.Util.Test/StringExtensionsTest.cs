using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Common.Util.Test
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void Given_NotMatchingReplaceText_When_ReplaceIgnoreCaseIsCalledWith_Then_StringIsNotModified()
        {
            Assert.AreEqual("abc", "abc".ReplaceIgnoreCase("d", "e"));
        }

        [TestMethod]
        public void Given_MatchingLowerCaseReplaceText_When_ReplaceIgnoreCaseIsCalled_The_StringIsModified()
        {
            Assert.AreEqual("xyzdef", "abcdef".ReplaceIgnoreCase("abc", "xyz"));
        }

        [TestMethod]
        public void Given_MatchingUpperCaseReplaceText_When_ReplaceIgnoreCaseIsCalled_The_StringIsModified()
        {
            Assert.AreEqual("xyzdef", "ABCdef".ReplaceIgnoreCase("abc", "xyz"));
        }

        [TestMethod]
        public void Given_MatchingNonAlphabeticReplaceText_When_ReplaceIgnoreCaseIsCalled_The_StringIsModified()
        {
            Assert.AreEqual("xyzdef", "A*BCdef".ReplaceIgnoreCase("a*bc", "xyz"));
            Assert.AreEqual("x*yzdef", "abcdef".ReplaceIgnoreCase("abc", "x*yz"));
            Assert.AreEqual("$def", "abcdef".ReplaceIgnoreCase("abc", "$"));
        }
    }
}
