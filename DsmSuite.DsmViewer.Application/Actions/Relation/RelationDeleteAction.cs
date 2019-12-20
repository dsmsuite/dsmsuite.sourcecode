using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationDeleteAction : ActionBase
    {
        private readonly int _id;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;
        private readonly string _type;
        private readonly int _weight;

        public RelationDeleteAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            _id = GetInt(data, nameof(_id));

            int consumerId = GetInt(data, nameof(_consumer));
            _consumer = model.GetElementById(consumerId);
            Debug.Assert(_consumer != null);

            int providerId = GetInt(data, nameof(_provider));
            _provider = model.GetElementById(providerId);
            Debug.Assert(_provider != null);

            _type = GetString(data, nameof(_type));
            _weight = GetInt(data, nameof(_weight));
        }

        public RelationDeleteAction(IDsmModel model, IDsmRelation relation) : base(model)
        {
            _id = relation.Id;
            _consumer = model.GetElementById(relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(relation.ProviderId);
            Debug.Assert(_provider != null);

            _type = relation.Type;
            _weight = relation.Weight;
        }

        public override string ActionName => nameof(RelationDeleteAction);
        public override string Title => "Delete relation";
        public override string Description => $"consumer={_consumer.Fullname} provider={_provider.Fullname} type={_type}";

        public override void Do()
        {
            Model.RemoveRelation(_id);
        }

        public override void Undo()
        {
            Model.ImportRelation(_id, _consumer.Id, _provider.Id, _type, _weight);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(_id), _id);
            SetInt(data, nameof(_consumer), _consumer.Id);
            SetInt(data, nameof(_provider), _provider.Id);
            SetString(data, nameof(_type), _type);
            SetInt(data, nameof(_weight), _weight);
            return data;
        }
    }
}
