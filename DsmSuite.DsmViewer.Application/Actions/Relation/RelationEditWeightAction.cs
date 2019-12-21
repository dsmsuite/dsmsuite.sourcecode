using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationEditWeightAction : ActionBase
    {
        private readonly IDsmRelation _relation;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;
        private readonly int _old;
        private readonly int _new;

        public RelationEditWeightAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_relation));
            _relation = model.GetRelationById(id);
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);

            _old = attributes.GetInt(nameof(_old));
            _new = attributes.GetInt(nameof(_new));
        }

        public RelationEditWeightAction(IDsmModel model, IDsmRelation relation, int weight) : base(model)
        {
            _relation = relation;
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);

            _old = relation.Weight;
            _new = weight;
        }

        public override string ActionName => nameof(RelationEditWeightAction);
        public override string Title => "Edit relation weight";
        public override string Description => $"consumer={_consumer.Fullname} provider={_provider.Fullname} type={_relation.Type} weight={_old}->{_new}";

        public override void Do()
        {
            Model.EditRelationWeight(_relation, _new);
        }

        public override void Undo()
        {
            Model.EditRelationWeight(_relation, _old);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            ActionAttributes attributes = new ActionAttributes();
            attributes.SetInt(nameof(Element), _relation.Id);
            attributes.SetInt(nameof(_old), _old);
            attributes.SetInt(nameof(_new), _new);
            return attributes.GetData();
        }
    }
}
