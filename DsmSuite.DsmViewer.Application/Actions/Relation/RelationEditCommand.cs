using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationEditAction : ActionBase
    {
        private int? _relationId;
        private readonly IDsmRelation _relation;
        private readonly string _oldType;
        private readonly int _oldWeight;
        private readonly string _newType;
        private readonly int _newWeight;

        public RelationEditAction(IDsmModel model, IDsmRelation relation, string type, int weight) : base(model)
        {
            _relation = relation;

            _oldType = _relation.Type;
            _oldWeight = _relation.Weight;

            _newType = type;
            _newWeight = weight;
        }

        public override void Do()
        {
             Model.EditRelation(_relation, _newType, _newWeight);
        }

        public override void Undo()
        {
             Model.EditRelation(_relation, _oldType, _oldWeight);
        }

        public override string Type => "Edit relation";
        public override string Details => "todo";
    }
}
