using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationEditTypeAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmRelation _relation;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;
        private readonly string _old ;
        private readonly string _new ;

        public const string TypeName = "redittype";

        public RelationEditTypeAction(IDsmModel model, IReadOnlyDictionary<string, string> data)
        {
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            int id = attributes.GetInt(nameof(_relation));
            _relation = model.GetRelationById(id);
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);

            _new = attributes.GetString(nameof(_new));
            _old = attributes.GetString(nameof(_old));
        }

        public RelationEditTypeAction(IDsmModel model, IDsmRelation relation, string type)
        {
            _model = model;

            _relation = relation;
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);

            _old = relation.Type;
            _new = type;
        }

        public string Type => TypeName;
        public string Title => "Edit relation type";
        public string Description => $"consumer={_consumer.Fullname} provider={_provider.Fullname} type={_old}->{_new}";

        public void Do()
        {
            _model.EditRelationType(_relation, _new);
        }

        public void Undo()
        {
            _model.EditRelationType(_relation, _old);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(Element), _relation.Id);
                attributes.SetString(nameof(_new), _new);
                attributes.SetString(nameof(_old), _old);
                return attributes.Data;
            }
        }
    }
}
