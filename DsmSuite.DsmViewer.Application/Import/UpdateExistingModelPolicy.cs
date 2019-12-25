using System.Collections.Generic;
using System.Linq;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Actions.Management;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.Common.Model.Interface;
using DsmSuite.DsmViewer.Application.Actions.Relation;

namespace DsmSuite.DsmViewer.Application.Import
{
    public class UpdateExistingModelPolicy : IImportPolicy
    {
        private readonly IDsmModel _dsmModel;
        private readonly IActionManager _actionManager;
        private readonly Dictionary<int, IDsmElement> _notFoundElements;
        private readonly Dictionary<int, IDsmRelation> _notFoundRelations;

        public UpdateExistingModelPolicy(IDsmModel dsmmodel, string dsmFilename, IActionManager actionManager)
        {
            _dsmModel = dsmmodel;
            _dsmModel.LoadModel(dsmFilename, null);
            _actionManager = actionManager;

            _notFoundElements = _dsmModel.GetElements().ToDictionary(x => x.Id, x => x);
            _notFoundRelations = _dsmModel.GetRelations().ToDictionary(x => x.Id, x => x);
        }

        public IMetaDataItem ImportMetaDataItem(string group, string name, string value)
        {
            return _dsmModel.AddMetaData(group, name, value);
        }

        public IDsmElement ImportElement(string fullname, string name, string type, IDsmElement parent)
        {
            IDsmElement element = _dsmModel.GetElementByFullname(fullname);

            if (element == null)
            {
                ElementCreateAction action = new ElementCreateAction(_dsmModel, name, type, parent);
                _actionManager.Execute(action);
                element = action.CreatedElement;
            }
            else
            {
                _notFoundElements.Remove(element.Id);
            }

            return element;
        }

        public IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            IDsmRelation relation = _dsmModel.FindRelation(consumerId, providerId, type);

            if (relation == null)
            {
                RelationCreateAction action = new RelationCreateAction(_dsmModel, consumerId, providerId, type, weight);
                _actionManager.Execute(action);
            }
            else
            {
                _notFoundRelations.Remove(relation.Id);

                if (relation.Weight != weight)
                {
                    RelationChangeWeightAction action = new RelationChangeWeightAction(_dsmModel, relation, weight);
                    _actionManager.Execute(action);
                }
            }

            return relation;
        }

        public void FinalizeImport()
        {
            RemoveExistingElementsNotFoundAnymore();
            RemoveExistingRelationsNotFoundAnymore();
            _dsmModel.AssignElementOrder();
        }

        private void RemoveExistingElementsNotFoundAnymore()
        {
            foreach (IDsmElement element in _notFoundElements.Values)
            {
                ElementDeleteAction action = new ElementDeleteAction(_dsmModel, element);
                _actionManager.Execute(action);
            }
        }

        private void RemoveExistingRelationsNotFoundAnymore()
        {
            foreach (IDsmRelation relation in _notFoundRelations.Values)
            {
                RelationDeleteAction action = new RelationDeleteAction(_dsmModel, relation);
                _actionManager.Execute(action);
            }
        }
    }
}
