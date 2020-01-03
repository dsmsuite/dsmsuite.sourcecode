using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationChangeWeightAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmRelation _relation;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;
        private readonly int _old;
        private readonly int _new;

        public const string TypeName = "rchangeweight";

        public RelationChangeWeightAction(object[] args)
        {
            Debug.Assert(args.Length == 2);
            _model = args[0] as IDsmModel;
            Debug.Assert(_model != null);
            IReadOnlyDictionary<string, string> data = args[1] as IReadOnlyDictionary<string, string>;
            Debug.Assert(data != null);

            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(_model, data);
            _relation = attributes.GetRelation(nameof(_relation));
            Debug.Assert(_relation != null);

            _consumer = attributes.GetRelationConsumer(nameof(_relation));
            Debug.Assert(_consumer != null);

            _provider = attributes.GetRelationProvider(nameof(_relation));
            Debug.Assert(_provider != null);

            _old = attributes.GetInt(nameof(_old));
            _new = attributes.GetInt(nameof(_new));
        }

        public RelationChangeWeightAction(IDsmModel model, IDsmRelation relation, int weight)
        {
            _model = model;
            Debug.Assert(_model != null);

            _relation = relation;
            Debug.Assert(_relation != null);

            _consumer = model.GetElementById(_relation.ConsumerId);
            Debug.Assert(_consumer != null);

            _provider = model.GetElementById(_relation.ProviderId);
            Debug.Assert(_provider != null);

            _old = relation.Weight;
            _new = weight;
        }

        public string Type => TypeName;
        public string Title => "Change relation weight";
        public string Description => $"consumer={_consumer.Fullname} provider={_provider.Fullname} type={_relation.Type} weight={_old}->{_new}";

        public object Do()
        {
            _model.ChangeRelationWeight(_relation, _new);
            return null;
        }

        public void Undo()
        {
            _model.ChangeRelationWeight(_relation, _old);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_relation), _relation.Id);
                attributes.SetInt(nameof(_old), _old);
                attributes.SetInt(nameof(_new), _new);
                return attributes.Data;
            }
        }
    }
}
