using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationChangeTypeAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmRelation _relation;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;
        private readonly string _old ;
        private readonly string _new ;

        public const string TypeName = "rchangetype";

        public RelationChangeTypeAction(object[] args)
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

            _consumer = _model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = _model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);

            _new = attributes.GetString(nameof(_new));
            _old = attributes.GetString(nameof(_old));
        }

        public RelationChangeTypeAction(IDsmModel model, IDsmRelation relation, string type)
        {
            _model = model;
            Debug.Assert(_model != null);

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
        public string Title => "Change relation type";
        public string Description => $"consumer={_consumer.Fullname} provider={_provider.Fullname} type={_old}->{_new}";

        public void Do()
        {
            _model.ChangeRelationType(_relation, _new);
        }

        public void Undo()
        {
            _model.ChangeRelationType(_relation, _old);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_relation), _relation.Id);
                attributes.SetString(nameof(_new), _new);
                attributes.SetString(nameof(_old), _old);
                return attributes.Data;
            }
        }
    }
}
