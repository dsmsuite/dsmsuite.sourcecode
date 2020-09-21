using System.Collections.Generic;
using System.Linq;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Actions.Management;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.Common.Model.Interface;
using DsmSuite.DsmViewer.Application.Actions.Relation;
using System.Diagnostics;
using DsmSuite.DsmViewer.Application.Actions.Snapshot;
using System;
using DsmSuite.Common.Util;

namespace DsmSuite.DsmViewer.Application.Import.Common
{
    public class UpdateExistingModelPolicy : IImportPolicy
    {
        private readonly IDsmModel _dsmModel;
        private readonly string _dsmFilename;
        private readonly IActionManager _actionManager;
        private readonly Dictionary<int, IDsmElement> _notFoundElements;
        private readonly Dictionary<int, IDsmRelation> _notFoundRelations;

        public UpdateExistingModelPolicy(IDsmModel dsmmodel, string dsmFilename, IActionManager actionManager, IProgress<ProgressInfo> progress)
        {
            _dsmModel = dsmmodel;
            _dsmFilename = dsmFilename;
            _dsmModel.LoadModel(_dsmFilename, progress);
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
                element = _actionManager.Execute(action) as IDsmElement;
                Debug.Assert(element != null);
            }
            else
            {
                _notFoundElements.Remove(element.Id);
            }

            return element;
        }

        public IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            IDsmRelation relation = null;
            IDsmElement consumer = _dsmModel.GetElementById(consumerId);
            IDsmElement provider = _dsmModel.GetElementById(providerId);

            if ((consumer != null) && (provider != null))
            {
                relation = _dsmModel.FindRelation(consumer, provider, type);
                if (relation == null)
                {
                    RelationCreateAction action = new RelationCreateAction(_dsmModel, consumerId, providerId, type, weight);
                    relation = _actionManager.Execute(action) as IDsmRelation;
                    Debug.Assert(relation != null);
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
            }
            
            return relation;
        }

        public void FinalizeImport(IProgress<ProgressInfo> progress)
        {
            // Remove relations before elements to ensure that elements can be resolved 
            // in RelationDeleteElement
            RemoveExistingRelationsNotFoundAnymore();
            RemoveExistingElementsNotFoundAnymore();

            _dsmModel.AssignElementOrder();

            MakeSnapshot();
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

        private void MakeSnapshot()
        {
            string timestamp = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            string description = $"Import file={_dsmFilename} time={timestamp}";
            MakeSnapshotAction action = new MakeSnapshotAction(_dsmModel, description);
            _actionManager.Execute(action);
        }
    }
}
