using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationDeleteAction : ActionBase
    {
        private readonly IDsmRelation _relation;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;

        public RelationDeleteAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            int id = GetInt(data, nameof(_relation));
            _relation = model.GetDeletedRelationById(id);
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);
        }

        public RelationDeleteAction(IDsmModel model, IDsmRelation relation) : base(model)
        {
            _relation = relation;
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);
        }

        public override string ActionName => nameof(RelationDeleteAction);
        public override string Title => "Delete relation";
        public override string Description => $"consumer={_consumer.Fullname} provider={_provider.Fullname} type={_relation.Type}";

        public override void Do()
        {
            Model.RemoveRelation(_relation.Id);
        }

        public override void Undo()
        {
            Model.UnremoveRelation(_relation.Id);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(_relation), _relation.Id);
            return data;
        }
    }
}
