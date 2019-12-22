using System;
using System.Linq;
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
                    case ElementCreateAction.TypeName:
                        {
                            _actionManager.Add(new ElementCreateAction(_model, action.Data));
                        }
                        break;
                    case ElementDeleteAction.TypeName:
                        {
                            _actionManager.Add(new ElementDeleteAction(_model, action.Data));
                        }
                        break;
                    case ElementEditNameAction.TypeName:
                        {
                            _actionManager.Add(new ElementEditNameAction(_model, action.Data));
                        }
                        break;
                    case ElementEditTypeAction.TypeName:
                        {
                            _actionManager.Add(new ElementEditTypeAction(_model, action.Data));
                        }
                        break;
                    case ElementMoveAction.TypeName:
                        {
                            _actionManager.Add(new ElementMoveAction(_model, action.Data));
                        }
                        break;
                    case ElementMoveDownAction.TypeName:
                        {
                            _actionManager.Add(new ElementMoveDownAction(_model, action.Data));
                        }
                        break;
                    case ElementMoveUpAction.TypeName:
                        {
                            _actionManager.Add(new ElementMoveUpAction(_model, action.Data));
                        }
                        break;
                    case ElementPartitionAction.TypeName:
                        {
                            _actionManager.Add(new ElementPartitionAction(_model, action.Data));
                        }
                        break;
                    case RelationCreateAction.TypeName:
                        {
                            _actionManager.Add(new RelationCreateAction(_model, action.Data));
                        }
                        break;
                    case RelationEditTypeAction.TypeName:
                        {
                            _actionManager.Add(new RelationEditTypeAction(_model, action.Data));
                        }
                        break;
                    case RelationEditWeightAction.TypeName:
                        {
                            _actionManager.Add(new RelationEditWeightAction(_model, action.Data));
                        }
                        break;
                    case RelationDeleteAction.TypeName:
                        {
                            _actionManager.Add(new RelationDeleteAction(_model, action.Data));
                        }
                        break;
                    case MakeSnapshotAction.TypeName:
                        {
                            _actionManager.Add(new MakeSnapshotAction(_model, action.Data));
                        }
                        break;
                }
            }
        }

        public void Save()
        {
            int index= 0;
            foreach (IAction action in _actionManager.GetUndoActions().Reverse())
            {
                index++;
                _model.AddAction(index, action.Type, action.Data);
            }
        }

        public object GetInstance(string strFullyQualifiedName)
        {
            Type t = Type.GetType(strFullyQualifiedName);
            return Activator.CreateInstance(t);
        }
    }
}
