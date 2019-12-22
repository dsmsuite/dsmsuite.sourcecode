using System;
using System.Linq;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Application.Actions.Relation;
using DsmSuite.DsmViewer.Application.Actions.Snapshot;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Management
{
    public class ActionStore
    {
        private readonly IDsmModel _model;
        private readonly IActionManager _actionManager;
        private readonly Dictionary<string, Type> _types;

        public ActionStore(IDsmModel model, IActionManager actionManager)
        {
            _model = model;
            _actionManager = actionManager;
            _types = new Dictionary<string, Type>();

            RegisterActionTypes();
        }

        public void Load()
        {
            foreach (IDsmAction action in _model.GetActions())
            {
                if (_types.ContainsKey(action.Type))
                {
                    Type type = _types[action.Type];
                    object[] args = { _model, action.Data };
                    object argumentList = args;
                    IAction instance = Activator.CreateInstance(type, argumentList) as IAction;
                    if (instance != null)
                    {
                        _actionManager.Add(instance);
                    }
                }
            }
        }

        public void Save()
        {
            int index= 0;
            foreach (IAction action in _actionManager.GetActionsInChronologicalOrder())
            {
                index++;
                _model.AddAction(index, action.Type, action.Data);
            }
        }

        private void RegisterActionTypes()
        {
            _types[ElementCreateAction.TypeName] = typeof(ElementCreateAction);
            _types[ElementDeleteAction.TypeName] = typeof(ElementDeleteAction);
            _types[ElementEditNameAction.TypeName] = typeof(ElementEditNameAction);
            _types[ElementEditTypeAction.TypeName] = typeof(ElementEditTypeAction);
            _types[ElementMoveAction.TypeName] = typeof(ElementMoveAction);
            _types[ElementMoveDownAction.TypeName] = typeof(ElementMoveDownAction);
            _types[ElementMoveUpAction.TypeName] = typeof(ElementMoveUpAction);
            _types[ElementPartitionAction.TypeName] = typeof(ElementPartitionAction);
            _types[RelationCreateAction.TypeName] = typeof(RelationCreateAction);
            _types[RelationEditTypeAction.TypeName] = typeof(RelationEditTypeAction);
            _types[RelationEditWeightAction.TypeName] = typeof(RelationEditWeightAction);
            _types[RelationDeleteAction.TypeName] = typeof(RelationDeleteAction);
            _types[ElementCreateAction.TypeName] = typeof(ElementCreateAction);
            _types[MakeSnapshotAction.TypeName] = typeof(MakeSnapshotAction);
        }
    }
}
