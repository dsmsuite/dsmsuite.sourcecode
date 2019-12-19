using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationCreateAction : ActionBase
    {
        public int Relation { get; private set; }
        public int Consumer { get; }
        public int Provider { get; }
        public string Type { get; }
        public int Weight { get; }

        public RelationCreateAction(IDsmModel model, int consumerId, int providerId, string type, int weight) : base(model)
        {
            Consumer = consumerId;
            Provider = providerId;
            Type = type;
            Weight = weight;

            ClassName = nameof(RelationCreateAction);
            Title = "Create relation";
            Details = $"?";
        }

        public override void Do()
        {
            IDsmRelation relation = Model.AddRelation(Consumer, Provider, Type, Weight);
            Debug.Assert(relation != null);
            Relation = relation.Id;
        }

        public override void Undo()
        {
            Model.RemoveRelation(Relation);
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
