using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;
using DsmSuite.DsmViewer.Application.Queries;

namespace DsmSuite.DsmViewer.Application.Test.Queries
{
    [TestClass]
    public class DsmResolvedRelationTest
    {
        private const int RelationId = 1;
        private const int ConsumerId = 2;
        private const int ProviderId = 3;
        private const string RelationType = "type";
        private const int RelationWeight = 4;

        [TestMethod]
        public void WhenConstructedThenConsumerAndProviderAreResolved()
        {
            Mock<IDsmModel> model = new Mock<IDsmModel>();
            Mock<IDsmRelation> relation = new Mock<IDsmRelation>();
            Mock<IDsmElement> consumer = new Mock<IDsmElement>();
            Mock<IDsmElement> provider = new Mock<IDsmElement>();

            relation.Setup(x => x.Id).Returns(RelationId);
            relation.Setup(x => x.ConsumerId).Returns(ConsumerId);
            relation.Setup(x => x.ProviderId).Returns(ProviderId);
            relation.Setup(x => x.Type).Returns(RelationType);
            relation.Setup(x => x.Weight).Returns(RelationWeight);

            model.Setup(x => x.GetElementById(ConsumerId)).Returns(consumer.Object);
            model.Setup(x => x.GetElementById(ProviderId)).Returns(provider.Object);

            DsmResolvedRelation resolvedRelation = new DsmResolvedRelation(model.Object, relation.Object);
            Assert.AreEqual(RelationId, resolvedRelation.Id);
            Assert.AreEqual(consumer.Object, resolvedRelation.ConsumerElement);
            Assert.AreEqual(provider.Object, resolvedRelation.ProviderElement);
            Assert.AreEqual(RelationType, resolvedRelation.Type);
            Assert.AreEqual(RelationWeight, resolvedRelation.Weight);
        }
    }
}
