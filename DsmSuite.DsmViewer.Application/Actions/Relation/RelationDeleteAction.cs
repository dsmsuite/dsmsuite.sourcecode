using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationDeleteAction : ActionBase
    {
        private int _relationId;

        public RelationDeleteAction(IDsmModel model, IDsmRelation relation) : base(model)
        {
            _relationId = relation.Id;

            Type = "Delete relation";
            Details = $"relation={relation.Id}";
        }

        public override void Do()
        {
            Model.RemoveRelation(_relationId);
        }

        public override void Undo()
        {
            Model.UnremoveRelation(_relationId);
        }
    }
}
