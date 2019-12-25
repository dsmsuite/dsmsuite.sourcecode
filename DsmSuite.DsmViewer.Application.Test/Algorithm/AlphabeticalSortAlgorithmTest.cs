using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Sorting;
using DsmSuite.DsmViewer.Application.Algorithm;

namespace DsmSuite.DsmViewer.Application.Test.Algorithm
{
    [TestClass]
    public class AlphabeticalSortAlgorithmTest
    {
        private const string _nameA = "a";
        private const string _nameB = "b";
        private const string _nameC = "c";
        private const string _nameD = "d";

        [TestMethod]
        public void GivenNonSortedElementLIstWhenSortedAlpabeticallyThemElementsAreInAlphabeticalOrder()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();

            Mock<IDsmElement> a = new Mock<IDsmElement>();
            a.Setup(x => x.Id).Returns(34);
            a.Setup(x => x.Name).Returns(_nameA);
            Mock<IDsmElement> b = new Mock<IDsmElement>();
            b.Setup(x => x.Id).Returns(45);
            b.Setup(x => x.Name).Returns(_nameB);
            Mock<IDsmElement> c = new Mock<IDsmElement>();
            c.Setup(x => x.Id).Returns(12);
            c.Setup(x => x.Name).Returns(_nameC);
            Mock<IDsmElement> d = new Mock<IDsmElement>();
            d.Setup(x => x.Id).Returns(23);
            d.Setup(x => x.Name).Returns(_nameD);
            List<IDsmElement> children = new List<IDsmElement> { c.Object, d.Object, a.Object, b.Object };

            Mock<IDsmElement> parent = new Mock<IDsmElement>();
            parent.Setup(x => x.Children).Returns(children);

            object[] args = { model.Object, parent.Object };
            AlphabeticalSortAlgorithm algortim = new AlphabeticalSortAlgorithm(args);
            SortResult result = algortim.Sort();
            Assert.AreEqual("2,3,0,1", result.Data);

            Assert.AreEqual(a.Object, children[result.GetIndex(0)]);
            Assert.AreEqual(b.Object, children[result.GetIndex(1)]);
            Assert.AreEqual(c.Object, children[result.GetIndex(2)]);
            Assert.AreEqual(d.Object, children[result.GetIndex(3)]);
        }
    }
}
