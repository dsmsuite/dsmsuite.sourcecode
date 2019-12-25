using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationCreateAction : IAction
    {
        private readonly IDsmModel _model;
        private IDsmRelation _relation;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;
        private readonly string _type;
        private readonly int _weight;

        public const string TypeName = "rcreate";

        public RelationCreateAction(object[] args)
        {
            Debug.Assert(args.Length == 2);
            _model = args[0] as IDsmModel;
            Debug.Assert(_model != null);
            IReadOnlyDictionary<string, string> data = args[1] as IReadOnlyDictionary<string, string>;
            Debug.Assert(data != null);

            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            int id = attributes.GetInt(nameof(_relation));
            _relation = _model.GetRelationById(id);
            Debug.Assert(_relation != null);

            int consumerId = attributes.GetInt(nameof(_consumer));
            _consumer = _model.GetElementById(consumerId);
            Debug.Assert(_consumer != null);

            int providerId = attributes.GetInt(nameof(_provider));
            _provider = _model.GetElementById(providerId);
            Debug.Assert(_provider != null);

            _type = attributes.GetString(nameof(_type));
            _weight = attributes.GetInt(nameof(_weight));
        }

        public RelationCreateAction(IDsmModel model, int consumerId, int providerId, string type, int weight)
        {
            _model = model;
            Debug.Assert(_model != null);

            _consumer = model.GetElementById(consumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(providerId);
            Debug.Assert(_provider != null);

            _type = type;
            _weight = weight;
        }

        public string Type => TypeName;
        public string Title => "Create relation";
        public string Description => $"consumer={_consumer.Fullname} provider={_provider.Fullname} type={_type} weight={_weight}";
        public virtual IDsmRelation CreatedRelation => _relation; // Virtual for testabiity with Moq mocking framework

        public void Do()
        {
            _relation = _model.AddRelation(_consumer.Id, _provider.Id, _type, _weight);
            Debug.Assert(_relation != null);
        }

        public void Undo()
        {
            _model.RemoveRelation(_relation.Id);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_relation), _relation.Id);
                attributes.SetInt(nameof(_consumer), _consumer.Id);
                attributes.SetInt(nameof(_provider), _provider.Id);
                attributes.SetString(nameof(_type), _type);
                attributes.SetInt(nameof(_weight), _weight);
                return attributes.Data;
            }
        }
    }
}
