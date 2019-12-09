using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationDeleteAction : ActionBase
    {
        private readonly IDsmRelation _relation;
        public RelationDeleteAction(IDsmModel model, IDsmRelation relation) : base(model)
        {
            _relation = relation;
        }

        public override void Do()
        {
            Model.RemoveRelation(_relation.Id);
        }

        public override void Undo()
        {
            Model.UnremoveRelation(_relation.Id);
        }

        public override string Type => "Delete relation";
        public override string Details => "todo";
    }
}
