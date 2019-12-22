using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationDeleteAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmRelation _relation;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;

        public const string TypeName = "rdelete";

        public RelationDeleteAction(IDsmModel model, IReadOnlyDictionary<string, string> data)
        {
            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_relation));
            _relation = model.GetDeletedRelationById(id);
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);
        }

        public RelationDeleteAction(IDsmModel model, IDsmRelation relation)
        {
            _model = model;

            _relation = relation;
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);
        }

        public string Type => TypeName;
        public string Title => "Delete relation";
        public string Description => $"consumer={_consumer.Fullname} provider={_provider.Fullname} type={_relation.Type}";

        public void Do()
        {
            _model.RemoveRelation(_relation.Id);
        }

        public void Undo()
        {
            _model.UnremoveRelation(_relation.Id);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_relation), _relation.Id);
                return attributes.Data;
            }
        }
    }
}
