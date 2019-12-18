using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationEditAction : ActionBase
    {
        private int _relationId;
        private readonly string _oldType;
        private readonly int _oldWeight;
        private readonly string _newType;
        private readonly int _newWeight;

        public RelationEditAction(IDsmModel model, IDsmRelation relation, string type, int weight) : base(model)
        {
            _relationId = relation.Id;

            _oldType = relation.Type;
            _oldWeight = relation.Weight;

            _newType = type;
            _newWeight = weight;

            Type = "Edit relation";
            Details = $"relation={relation.Id}";
        }

        public override void Do()
        {
            IDsmRelation relation = Model.GetRelationById(_relationId);
            if (relation != null)
            {
                Model.EditRelation(relation, _newType, _newWeight);
            }
        }

        public override void Undo()
        {
            IDsmRelation relation = Model.GetRelationById(_relationId);
            if (relation != null)
            {
                Model.EditRelation(relation, _oldType, _oldWeight);
            }
        }
    }
}
