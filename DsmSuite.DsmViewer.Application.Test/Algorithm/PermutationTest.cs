using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Sorting;

namespace DsmSuite.DsmViewer.Application.Test.Algorithm
{
    [TestClass]
    public class PermutationTest
    {
        [TestMethod]
        public void GivenTwoPermutationsWithIdenticalInputValuesWhenCheckingForEqualityThenTheyAreTheSame()
        {
            Permutation permutation1 = new Permutation(12, 34);
            Permutation permutation2 = new Permutation(12, 34);
            Assert.IsTrue(permutation1.Equals(permutation2));
            Assert.AreEqual(permutation1.GetHashCode(), permutation2.GetHashCode());
        }

        [TestMethod]
        public void GivenTwoPermutationsWithIdenticalInputValuesButDifferentOrderWhenCheckingForEqualityThenTheyAreTheSame()
        {
            Permutation permutation1 = new Permutation(12, 34);
            Permutation permutation2 = new Permutation(34, 12);
            Assert.IsTrue(permutation1.Equals(permutation2));
            Assert.AreEqual(permutation1.GetHashCode(), permutation2.GetHashCode());
        }

        [TestMethod]
        public void GivenTwoPermutationsWithDifferentInputValuesWhenCheckingForEqualityThenTheyAreDifferent()
        {
            Permutation permutation1 = new Permutation(12, 34);
            Permutation permutation2 = new Permutation(12, 56);
            Assert.IsFalse(permutation1.Equals(permutation2));
            Assert.AreNotEqual(permutation1.GetHashCode(), permutation2.GetHashCode());
        }

        [TestMethod]
        public void GivenOnePermutationsWhenCheckingForEqualityWithNonPermutationObjectThenTheyAreDifferent()
        {
            Permutation permutation1 = new Permutation(12, 34);
            Assert.IsFalse(permutation1.Equals(this));
        }
    }
}
