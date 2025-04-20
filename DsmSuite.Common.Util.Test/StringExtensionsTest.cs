namespace DsmSuite.Common.Util.Test
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void GivenNotMatchingReplaceTextWhenReplaceIgnoreCaseIsCalledWithThenStringIsNotModified()
        {
            Assert.AreEqual("abc", "abc".ReplaceIgnoreCase("d", "e"));
        }

        [TestMethod]
        public void GivenMatchingLowerCaseReplaceTextWhenReplaceIgnoreCaseIsCalledTheStringIsModified()
        {
            Assert.AreEqual("xyzdef", "abcdef".ReplaceIgnoreCase("abc", "xyz"));
        }

        [TestMethod]
        public void GivenMatchingUpperCaseReplaceTextWhenReplaceIgnoreCaseIsCalledTheStringIsModified()
        {
            Assert.AreEqual("xyzdef", "ABCdef".ReplaceIgnoreCase("abc", "xyz"));
        }

        [TestMethod]
        public void GivenMatchingNonAlphabeticReplaceTextWhenReplaceIgnoreCaseIsCalledTheStringIsModified()
        {
            Assert.AreEqual("xyzdef", "A*BCdef".ReplaceIgnoreCase("a*bc", "xyz"));
            Assert.AreEqual("x*yzdef", "abcdef".ReplaceIgnoreCase("abc", "x*yz"));
            Assert.AreEqual("$def", "abcdef".ReplaceIgnoreCase("abc", "$"));
        }
    }
}
