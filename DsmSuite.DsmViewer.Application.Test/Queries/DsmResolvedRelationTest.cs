using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using DsmSuite.DsmViewer.Application.Queries;

namespace DsmSuite.DsmViewer.Application.Test.Queries
{
    [TestClass]
    public class DsmResolvedRelationTest
    {
        private const int _relationId = 1;
        private const int _consumerId = 2;
        private const int _providerId = 3;
        private const string _relationType = "type";
        private const int _relationWeight = 4;

        [TestMethod]
        public void WhenConstructedThenConsumerAndProviderAreResolved()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Mock<IDsmRelation> relation = new Mock<IDsmRelation>();
            Mock<IDsmElement> consumer = new Mock<IDsmElement>();
            Mock<IDsmElement> provider = new Mock<IDsmElement>();

            relation.Setup(x => x.Id).Returns(_relationId);
            relation.Setup(x => x.ConsumerId).Returns(_consumerId);
            relation.Setup(x => x.ProviderId).Returns(_providerId);
            relation.Setup(x => x.Type).Returns(_relationType);
            relation.Setup(x => x.Weight).Returns(_relationWeight);

            model.Setup(x => x.GetElementById(_consumerId)).Returns(consumer.Object);
            model.Setup(x => x.GetElementById(_providerId)).Returns(provider.Object);

            DsmResolvedRelation resolvedRelation = new DsmResolvedRelation(model.Object, relation.Object);
            Assert.AreEqual(_relationId, resolvedRelation.Id);
            Assert.AreEqual(consumer.Object, resolvedRelation.ConsumerElement);
            Assert.AreEqual(provider.Object, resolvedRelation.ProviderElement);
            Assert.AreEqual(_relationType, resolvedRelation.Type);
            Assert.AreEqual(_relationWeight, resolvedRelation.Weight);
        }
    }
}
