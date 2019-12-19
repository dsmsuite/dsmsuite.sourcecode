using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationCreateAction : ActionBase
    {
        private IDsmRelation _relation;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;
        private readonly string _type;
        private readonly int _weight;

        public RelationCreateAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            int id = GetInt(data, nameof(_relation));
            _relation = model.GetRelationById(id);
            Debug.Assert(_relation != null);

            int consumerId = GetInt(data, nameof(_consumer));
            _consumer = model.GetElementById(consumerId);
            Debug.Assert(_consumer != null);

            int providerId = GetInt(data, nameof(_provider));
            _provider = model.GetElementById(providerId);
            Debug.Assert(_provider != null);

            _type = GetString(data, nameof(_type));
            _weight = GetInt(data, nameof(_weight));
        }

        public RelationCreateAction(IDsmModel model, int consumerId, int providerId, string type, int weight) : base(model)
        {
            _consumer = model.GetElementById(consumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(providerId);
            Debug.Assert(_provider != null);

            _type = type;
            _weight = weight;
        }

        public override string ActionName => nameof(RelationCreateAction);
        public override string Title => "Create relation";
        public override string Description => $"consumer={_consumer.Fullname} provider={_provider.Fullname} type={_type} weight={_weight}";

        public override void Do()
        {
            _relation = Model.AddRelation(_consumer.Id, _provider.Id, _type, _weight);
            Debug.Assert(_relation != null);
        }

        public override void Undo()
        {
            Model.RemoveRelation(_relation.Id);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(_relation), _relation.Id);
            SetInt(data, nameof(_consumer), _consumer.Id);
            SetInt(data, nameof(_provider), _provider.Id);
            SetString(data, nameof(_type), _type);
            SetInt(data, nameof(_weight), _weight);
            return data;
        }
    }
}
