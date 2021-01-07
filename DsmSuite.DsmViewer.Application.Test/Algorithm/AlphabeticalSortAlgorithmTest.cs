using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Sorting;

namespace DsmSuite.DsmViewer.Application.Test.Algorithm
{
    [TestClass]
    public class AlphabeticalSortAlgorithmTest
    {
        private const string NameA = "a";
        private const string NameB = "b";
        private const string NameC = "c";
        private const string NameD = "d";

        [TestMethod]
        public void GivenNonSortedElementLIstWhenSortedAlphabeticallyThemElementsAreInAlphabeticalOrder()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();

            Mock<IDsmElement> a = new Mock<IDsmElement>();
            a.Setup(x => x.Id).Returns(34);
            a.Setup(x => x.Name).Returns(NameA);
            Mock<IDsmElement> b = new Mock<IDsmElement>();
            b.Setup(x => x.Id).Returns(45);
            b.Setup(x => x.Name).Returns(NameB);
            Mock<IDsmElement> c = new Mock<IDsmElement>();
            c.Setup(x => x.Id).Returns(12);
            c.Setup(x => x.Name).Returns(NameC);
            Mock<IDsmElement> d = new Mock<IDsmElement>();
            d.Setup(x => x.Id).Returns(23);
            d.Setup(x => x.Name).Returns(NameD);
            List<IDsmElement> children = new List<IDsmElement> { c.Object, d.Object, a.Object, b.Object };

            Mock<IDsmElement> parent = new Mock<IDsmElement>();
            parent.Setup(x => x.Children).Returns(children);

            object[] args = { model.Object, parent.Object };
            AlphabeticalSortAlgorithm algorithm = new AlphabeticalSortAlgorithm(args);
            SortResult result = algorithm.Sort();
            Assert.AreEqual("2,3,0,1", result.Data);

            Assert.AreEqual(a.Object, children[result.GetIndex(0)]);
            Assert.AreEqual(b.Object, children[result.GetIndex(1)]);
            Assert.AreEqual(c.Object, children[result.GetIndex(2)]);
            Assert.AreEqual(d.Object, children[result.GetIndex(3)]);
        }
    }
}
