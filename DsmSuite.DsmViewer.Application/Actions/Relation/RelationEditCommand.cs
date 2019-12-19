using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationEditAction : ActionBase
    {
        public int Relation { get; }
        public string OldType { get; }
        public int OldWeight { get; }
        public string NewType { get; }
        public int NewWeight { get; }

        public RelationEditAction(IDsmModel model, IDsmRelation relation, string type, int weight) : base(model)
        {
            Relation = relation.Id;

            OldType = relation.Type;
            OldWeight = relation.Weight;

            NewType = type;
            NewWeight = weight;

            ClassName = nameof(RelationEditAction);
            Title = "Edit relation";
            Details = $"relation={relation.Id}";
        }

        public override void Do()
        {
            IDsmRelation relation = Model.GetRelationById(Relation);
            Debug.Assert(relation != null);
            Model.EditRelation(relation, NewType, NewWeight);
        }

        public override void Undo()
        {
            IDsmRelation relation = Model.GetRelationById(Relation);
            Debug.Assert(relation != null);
            Model.EditRelation(relation, OldType, OldWeight);
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
