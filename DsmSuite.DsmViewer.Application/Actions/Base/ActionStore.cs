using System;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Application.Actions.Relation;
using DsmSuite.DsmViewer.Application.Actions.Snapshot;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public class ActionStore
    {
        private readonly IDsmModel _model;
        private readonly ActionManager _actionManager;

        public ActionStore(IDsmModel model, ActionManager actionManager)
        {
            _model = model;
            _actionManager = actionManager;
        }

        public void Load()
        {
            foreach (IDsmAction action in _model.GetActions())
            {
                switch (action.Type)
                {
                    case nameof(ElementCreateAction):
                        {
                            _actionManager.Add(new ElementCreateAction(_model, action.Data));
                        }
                        break;
                    case nameof(ElementDeleteAction):
                        {
                            _actionManager.Add(new ElementDeleteAction(_model, action.Data));
                        }
                        break;
                    case nameof(ElementEditNameAction):
                        {
                            _actionManager.Add(new ElementEditNameAction(_model, action.Data));
                        }
                        break;
                    case nameof(ElementEditTypeAction):
                        {
                            _actionManager.Add(new ElementEditTypeAction(_model, action.Data));
                        }
                        break;
                    case nameof(ElementMoveAction):
                        {
                            _actionManager.Add(new ElementMoveAction(_model, action.Data));
                        }
                        break;
                    case nameof(ElementMoveDownAction):
                        {
                            _actionManager.Add(new ElementMoveDownAction(_model, action.Data));
                        }
                        break;
                    case nameof(ElementMoveUpAction):
                        {
                            _actionManager.Add(new ElementMoveUpAction(_model, action.Data));
                        }
                        break;
                    case nameof(ElementPartitionAction):
                        {
                            _actionManager.Add(new ElementPartitionAction(_model, action.Data));
                        }
                        break;
                    case nameof(RelationCreateAction):
                        {
                            _actionManager.Add(new RelationCreateAction(_model, action.Data));
                        }
                        break;
                    case nameof(RelationEditTypeAction):
                        {
                            _actionManager.Add(new RelationEditTypeAction(_model, action.Data));
                        }
                        break;
                    case nameof(RelationEditWeightAction):
                        {
                            _actionManager.Add(new RelationEditWeightAction(_model, action.Data));
                        }
                        break;
                    case nameof(RelationDeleteAction):
                        {
                            _actionManager.Add(new RelationDeleteAction(_model, action.Data));
                        }
                        break;
                    case nameof(MakeSnapshotAction):
                        {
                            _actionManager.Add(new MakeSnapshotAction(_model, action.Data));
                        }
                        break;
                }
            }
        }

        public void Save()
        {
            int id = 0;
            foreach (IAction action in _actionManager.GetActions())
            {
                id++;
                _model.AddAction(id, action.ActionName, action.Pack());
            }
        }

        public object GetInstance(string strFullyQualifiedName)
        {
            Type t = Type.GetType(strFullyQualifiedName);
            return Activator.CreateInstance(t);
        }
    }
}
