using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationDeleteAction : ActionBase
    {
        public int Relation { get; private set; }

        public RelationDeleteAction(IDsmModel model, IDsmRelation relation) : base(model)
        {
            Relation = relation.Id;

            ClassName = nameof(RelationDeleteAction);
            Title = "Delete relation";
            Details = $"relation={relation.Id}";
        }

        public override void Do()
        {
            Model.RemoveRelation(Relation);
        }

        public override void Undo()
        {
            Model.UnremoveRelation(Relation);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            return null;
        }

        public override IAction Unpack(IReadOnlyDictionary<string, string> data)
        {
            return null;
        }
    }
}
