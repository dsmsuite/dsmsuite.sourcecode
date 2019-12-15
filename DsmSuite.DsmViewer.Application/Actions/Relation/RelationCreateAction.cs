using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationCreateAction : ActionBase
    {
        private IDsmRelation _relation;
        private readonly int _consumerId;
        private readonly int _providerId;
        private readonly string _type;
        private readonly int _weight;

        public RelationCreateAction(IDsmModel model, int consumerId, int providerId, string type, int weight) : base(model)
        {
            _consumerId = consumerId;
            _providerId = providerId;
            _type = type;
            _weight = weight;
        }

        public override void Do()
        {
            _relation = Model.AddRelation(_consumerId, _providerId, _type, _weight);
        }

        public override void Undo()
        {
            if (_relation != null)
            {
                Model.RemoveRelation(_relation);
            }
        }

        public override string Type => "Create relation";
        public override string Details => $"relation={_relation.Id}";
    }
}
