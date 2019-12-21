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
            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_relation));
            _relation = model.GetRelationById(id);
            Debug.Assert(_relation != null);

            int consumerId = attributes.GetInt(nameof(_consumer));
            _consumer = model.GetElementById(consumerId);
            Debug.Assert(_consumer != null);

            int providerId = attributes.GetInt(nameof(_provider));
            _provider = model.GetElementById(providerId);
            Debug.Assert(_provider != null);

            _type = attributes.GetString(nameof(_type));
            _weight = attributes.GetInt(nameof(_weight));
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
            ActionAttributes attributes = new ActionAttributes();
            attributes.SetInt(nameof(_relation), _relation.Id);
            attributes.SetInt(nameof(_consumer), _consumer.Id);
            attributes.SetInt(nameof(_provider), _provider.Id);
            attributes.SetString(nameof(_type), _type);
            attributes.SetInt(nameof(_weight), _weight);
            return attributes.GetData();
        }
    }
}
