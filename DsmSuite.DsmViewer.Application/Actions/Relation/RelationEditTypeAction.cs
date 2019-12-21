using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationEditTypeAction : ActionBase
    {
        private readonly IDsmRelation _relation;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;
        private readonly string _old ;
        private readonly string _new ;
        
        public RelationEditTypeAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            int id = GetInt(data, nameof(_relation));
            _relation = model.GetRelationById(id);
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);

            _new = GetString(data, nameof(_new));
            _old = GetString(data, nameof(_old));
        }

        public RelationEditTypeAction(IDsmModel model, IDsmRelation relation, string type) : base(model)
        {
            _relation = relation;
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);

            _old = relation.Type;
            _new = type;
        }

        public override string ActionName => nameof(RelationEditTypeAction);
        public override string Title => "Edit relation type";
        public override string Description => $"consumer={_consumer.Fullname} provider={_provider.Fullname} type={_old}->{_new}";

        public override void Do()
        {
            Model.EditRelationType(_relation, _new);
        }

        public override void Undo()
        {
            Model.EditRelationType(_relation, _old);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(Element), _relation.Id);
            SetString(data, nameof(_new), _new);
            SetString(data, nameof(_old), _old);
            return data;
        }
    }
}
