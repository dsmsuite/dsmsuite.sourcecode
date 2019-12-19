using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Application.Actions.Relation;
using DsmSuite.DsmViewer.Application.Actions.Snapshot;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsmSuite.DsmViewer.Application.Actions
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
                            string elementId = action.Data[nameof(ElementCreateAction.Element)];
                            string name = action.Data[nameof(ElementCreateAction.Name)];
                            string type = action.Data[nameof(ElementCreateAction.Type)];
                            string parentId = action.Data[nameof(ElementCreateAction.Parent)];
                        }
                        break;
                    case nameof(ElementDeleteAction):
                        {
                            string elementId = action.Data[nameof(ElementDeleteAction.Element)];
                        }
                        break;
                    case nameof(ElementEditAction):
                        {
                            string elementId = action.Data[nameof(ElementEditAction.Element)];
                            string oldname = action.Data[nameof(ElementEditAction.OldName)];
                            string newname = action.Data[nameof(ElementEditAction.NewName)];
                            string oldtype = action.Data[nameof(ElementEditAction.OldType)];
                            string newtype = action.Data[nameof(ElementEditAction.NewType)];
                        }
                        break;
                    case nameof(ElementMoveAction):
                        {
                            string elementId = action.Data[nameof(ElementMoveAction.Element)];
                            string oldparent = action.Data[nameof(ElementMoveAction.OldParent)];
                            string newparent = action.Data[nameof(ElementMoveAction.NewParent)];
                        }
                        break;
                    case nameof(ElementMoveDownAction):
                        {
                            string elementId = action.Data[nameof(ElementMoveDownAction.Element)];
                        }
                        break;
                    case nameof(ElementMoveUpAction):
                        {
                            string elementId = action.Data[nameof(ElementMoveUpAction.Element)];
                        }
                        break;
                    case nameof(ElementPartitionAction):
                        {
                            string elementId = action.Data[nameof(ElementMoveAction.Element)];
                            string oldparent = action.Data[nameof(ElementMoveAction.OldParent)];
                            string newparent = action.Data[nameof(ElementMoveAction.NewParent)];
                        }
                        break;
                    case nameof(RelationCreateAction):
                        {
                            string relationId = action.Data[nameof(RelationCreateAction.Relation)];
                            string consumerId = action.Data[nameof(RelationCreateAction.Consumer)];
                            string providerId = action.Data[nameof(RelationCreateAction.Provider)];
                            string type = action.Data[nameof(RelationCreateAction.Type)];
                            string weight = action.Data[nameof(RelationCreateAction.Weight)];
                        }
                        break;
                    case nameof(RelationEditAction):
                        {
                            string relationId = action.Data[nameof(RelationEditAction.Relation)];
                            string oldType = action.Data[nameof(RelationEditAction.OldType)];
                            string newType = action.Data[nameof(RelationEditAction.NewType)];
                            string oldWeight = action.Data[nameof(RelationEditAction.OldWeight)];
                            string newWeight = action.Data[nameof(RelationEditAction.NewWeight)];
                        }
                        break;
                    case nameof(RelationDeleteAction):
                        {
                            string relationId = action.Data[nameof(RelationDeleteAction.Relation)];
                        }
                        break;
                    case nameof(MakeSnapshotAction):
                        {
                            string name = action.Data[nameof(MakeSnapshotAction.Name)];
                        }
                        break;
                }

                //_actionManager.AddAction(action.Id, action.Type, action.Data);
            }
        }

        public void Save()
        {
            int id = 0;
            foreach (IAction action in _actionManager.GetActions())
            {
                id++;

                Dictionary<string, string> data = new Dictionary<string, string>();
                switch (action.ClassName)
                {
                    case nameof(ElementCreateAction):
                        {
                            string elementId = data[nameof(ElementCreateAction.Element)];
                            string name = data[nameof(ElementCreateAction.Name)];
                            string type = data[nameof(ElementCreateAction.Type)];
                            string parentId = data[nameof(ElementCreateAction.Parent)];
                        }
                        break;
                    case nameof(ElementDeleteAction):
                        {
                            string elementId = data[nameof(ElementDeleteAction.Element)];
                        }
                        break;
                    case nameof(ElementEditAction):
                        {
                            string elementId = data[nameof(ElementEditAction.Element)];
                            string oldname = data[nameof(ElementEditAction.OldName)];
                            string newname = data[nameof(ElementEditAction.NewName)];
                            string oldtype = data[nameof(ElementEditAction.OldType)];
                            string newtype = data[nameof(ElementEditAction.NewType)];
                        }
                        break;
                    case nameof(ElementMoveAction):
                        {
                            string elementId = data[nameof(ElementMoveAction.Element)];
                            string oldparent = data[nameof(ElementMoveAction.OldParent)];
                            string newparent = data[nameof(ElementMoveAction.NewParent)];
                        }
                        break;
                    case nameof(ElementMoveDownAction):
                        {
                            string elementId = data[nameof(ElementMoveDownAction.Element)];
                        }
                        break;
                    case nameof(ElementMoveUpAction):
                        {
                            string elementId = data[nameof(ElementMoveUpAction.Element)];
                        }
                        break;
                    case nameof(ElementPartitionAction):
                        {
                            string elementId = data[nameof(ElementMoveAction.Element)];
                            string oldparent = data[nameof(ElementMoveAction.OldParent)];
                            string newparent = data[nameof(ElementMoveAction.NewParent)];
                        }
                        break;
                    case nameof(RelationCreateAction):
                        {
                            string relationId = data[nameof(RelationCreateAction.Relation)];
                            string consumerId = data[nameof(RelationCreateAction.Consumer)];
                            string providerId = data[nameof(RelationCreateAction.Provider)];
                            string type = data[nameof(RelationCreateAction.Type)];
                            string weight = data[nameof(RelationCreateAction.Weight)];
                        }
                        break;
                    case nameof(RelationEditAction):
                        {
                            string relationId = data[nameof(RelationEditAction.Relation)];
                            string oldType = data[nameof(RelationEditAction.OldType)];
                            string newType = data[nameof(RelationEditAction.NewType)];
                            string oldWeight = data[nameof(RelationEditAction.OldWeight)];
                            string newWeight = data[nameof(RelationEditAction.NewWeight)];
                        }
                        break;
                    case nameof(RelationDeleteAction):
                        {
                            string relationId = data[nameof(RelationDeleteAction.Relation)];
                        }
                        break;
                    case nameof(MakeSnapshotAction):
                        {
                            string name = data[nameof(MakeSnapshotAction.Name)];
                        }
                        break;
                }

                _model.AddAction(id, action.ClassName, data);
            }
        }

        public object GetInstance(string strFullyQualifiedName)
        {
            Type t = Type.GetType(strFullyQualifiedName);
            return Activator.CreateInstance(t);
        }
    }
}
