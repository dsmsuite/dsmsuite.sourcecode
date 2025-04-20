using DsmSuite.DsmViewer.Application.Actions.Snapshot;
using DsmSuite.DsmViewer.Model.Interfaces;
using Moq;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Snapshot
{
    [TestClass]
    public class MakeSnapshotActionTest
    {
        private Mock<IDsmModel> _model;
        private Dictionary<string, string> _data;
        private const string Name = "name";

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();

            _data = new Dictionary<string, string>
            {
                ["name"] = Name
            };
        }

        [TestMethod]
        public void WhenCreatingNewActionThenActionAttributesMatch()
        {
            MakeSnapshotAction action = new MakeSnapshotAction(_model.Object, Name);

            Assert.AreEqual(1, action.Data.Count);
            Assert.AreEqual(Name, _data["name"]);
        }
    }
}
