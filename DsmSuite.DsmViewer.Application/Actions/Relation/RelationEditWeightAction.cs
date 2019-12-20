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
            int id = GetInt(data, nameof(_relation));
            _relation = model.GetRelationById(id);
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);

            _old = GetInt(data, nameof(_old));
            _new = GetInt(data, nameof(_new));
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
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(Element), _relation.Id);
            SetInt(data, nameof(_old), _old);
            SetInt(data, nameof(_new), _new);
            return data;
        }
    }
}
