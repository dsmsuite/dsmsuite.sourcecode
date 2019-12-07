using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationDeleteAction : ActionBase
    {
        private readonly int _consumerId;
        private readonly int _providerId;
        private readonly string _type;
        private readonly int _weight;

        public RelationDeleteAction(IDsmModel model, int consumerId, int providerId, string type, int weight) : base(model)
        {
            _consumerId = consumerId;
            _providerId = providerId;
            _type = type;
            _weight = weight;
        }

        public override void Do()
        {
            Model.RemoveRelation(_consumerId, _providerId, _type, _weight);
        }

        public override void Undo()
        {
            Model.UnremoveRelation(_consumerId, _providerId, _type, _weight);
        }

        public override string Description => "Delete relation";
    }
}
