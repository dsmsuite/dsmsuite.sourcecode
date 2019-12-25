using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Algorithm;

namespace DsmSuite.DsmViewer.Application.Test.Algorithm
{
    [TestClass]
    public class PermutationTest
    {
        [TestMethod]
        public void GivenTwoPermutationsAreIdentcalWhenCheckForEqualsRetrunsTrue()
        {
            Permutation permutation1 = new Permutation(12, 34);
            Permutation permutation2 = new Permutation(12, 34);
            Assert.IsTrue(permutation1.Equals(permutation2));
        }

        [TestMethod]
        public void GivenTwoPermutationsAreIdentcalWhenGetHashThenReturnSameValue()
        {
            Permutation permutation1 = new Permutation(12, 34);
            Permutation permutation2 = new Permutation(12, 34);
            Assert.AreEqual(permutation1.GetHashCode(), permutation2.GetHashCode());
        }

        [TestMethod]
        public void GivenTwoPermutationsAreDiffentWhenCheckForEqualsReturnsFalse()
        {
            Permutation permutation1 = new Permutation(12, 34);
            Permutation permutation2 = new Permutation(12, 56);
            Assert.IsFalse(permutation1.Equals(permutation2));
        }

        [TestMethod]
        public void GivenTwoPermutationsAreIdentcalWhenGetHashThenReturnDifferentValue()
        {
            Permutation permutation1 = new Permutation(12, 34);
            Permutation permutation2 = new Permutation(12, 56);
            Assert.AreNotEqual(permutation1.GetHashCode(), permutation2.GetHashCode());
        }
    }
}
