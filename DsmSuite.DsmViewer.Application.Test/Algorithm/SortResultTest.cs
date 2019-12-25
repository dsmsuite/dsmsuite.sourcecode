using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Sorting;

namespace DsmSuite.DsmViewer.Application.Test.Algorithm
{
    [TestClass]
    public class SortResultTest
    {
        [TestMethod]
        public void WhenSortResultConstructedWithZeroSizeThenItIsInvalid()
        {
            SortResult result = new SortResult(0);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void WhenSortResultConstructedWithNonZeroSizeThenItIsValid()
        {
            SortResult result = new SortResult(4);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.GetIndex(0));
            Assert.AreEqual(1, result.GetIndex(1));
            Assert.AreEqual(2, result.GetIndex(2));
            Assert.AreEqual(3, result.GetIndex(3));
        }
        
        [TestMethod]
        public void WhenSortResultConstructedWithEmptyStringThenOrderItIsInvalid()
        {
            SortResult result = new SortResult("");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void WhenSortResultConstructedWithTextStringThenItIsInvalid()
        {
            SortResult result = new SortResult("abc");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void WhenSortResultConstructedWithSingleNumberStringThenItIsValid()
        {
            string input = "0";
            SortResult result = new SortResult(input);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.GetIndex(0));
        }

        [TestMethod]
        public void WhenSortResultConstructedWithInvalidCommaSeparatedNumberStringThenItIsInvalid()
        {
            SortResult result = new SortResult("3,1,0");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void WhenSortResultConstructedWithCommaSeparatedNumberStringThenItIsValid()
        {
            SortResult result = new SortResult("2,1,0");
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(1, result.GetIndex(1));
            Assert.AreEqual(0, result.GetIndex(2));
        }
        
        [TestMethod]
        public void WhenSortResultConstructedWithCommaSeparatedNumberStringThenDataReturnsSameString()
        {
            string input = "3,2,1,0";
            SortResult result = new SortResult(input);
            Assert.AreEqual(input, result.Data);
        }

        [TestMethod]
        public void WhenSwapWithValidArgumentThenTheOrderIsChanged()
        {
            SortResult result = new SortResult("2,1,0");
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(1, result.GetIndex(1));
            Assert.AreEqual(0, result.GetIndex(2));

            result.Swap(0, 1);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.GetIndex(0));
            Assert.AreEqual(2, result.GetIndex(1));
            Assert.AreEqual(0, result.GetIndex(2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WhenSwapWithOutOfBoundArgumentThenExceptionIsThrown()
        {
            SortResult result = new SortResult("2,1,0");
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(1, result.GetIndex(1));
            Assert.AreEqual(0, result.GetIndex(2));

            result.Swap(0, 3);
        }

        [TestMethod]
        public void WhenInvertOrderThenTheOrderIsChanged()
        {
            SortResult result = new SortResult("2,0,1");
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(0, result.GetIndex(1));
            Assert.AreEqual(1, result.GetIndex(2));

            result.InvertOrder();

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.GetIndex(0));
            Assert.AreEqual(2, result.GetIndex(1));
            Assert.AreEqual(0, result.GetIndex(2));
        }

        [TestMethod]
        public void WhenInvertOrderIsCalledTwiceThenTheOrderIsUnchanged()
        {
            SortResult result = new SortResult("2,0,1");
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(0, result.GetIndex(1));
            Assert.AreEqual(1, result.GetIndex(2));

            result.InvertOrder();
            result.InvertOrder();

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.GetIndex(0));
            Assert.AreEqual(0, result.GetIndex(1));
            Assert.AreEqual(1, result.GetIndex(2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WhenGetIndexWithOutOfBoundArgumentThenExceptionIsThrown()
        {
            SortResult result = new SortResult("2,1,0");
            Assert.IsTrue(result.IsValid);
            result.GetIndex(3);
        }
    }
}
