using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using Microsoft.SqlServer.Server;

namespace DsmSuite.Analyzer.Diff
{
    class Comparer
    {
        private readonly IDsiModel _model1;
        private readonly IDsiModel _model2;
        private IProgress<ProgressInfo> _progress;

        private readonly HashSet<string> _foundModel1ElementNames = new HashSet<string>();
        private readonly HashSet<string> _foundModel2ElementNames = new HashSet<string>();
        private readonly HashSet<string> _missingModel1ElementNames = new HashSet<string>();
        private readonly HashSet<string> _missingModel2ElementNames = new HashSet<string>();
        private readonly HashSet<string> _allElementNames = new HashSet<string>();

        private readonly Dictionary<string, HashSet<string>> _foundModel1RelationProviderNames = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _foundModel2RelationProviderNames = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _missingModel1RelationProviderNames = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _missingModel2RelationProviderNames = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _allRelationProviderNames = new Dictionary<string, HashSet<string>>();

        public Comparer(IDsiModel model1, IDsiModel model2, IProgress<ProgressInfo> progress)
        {
            _model1 = model1;
            _model2 = model2;
            _progress = progress;
        }

        public void Compare()
        {
            FindElements();

            foreach (string elementName in _allElementNames)
            {
                CompareElement(elementName);
            }

            foreach (string elementName in _allElementNames)
            {
                CompareElementRelations(elementName);
            }

            if (AreIdentical())
            {
                Logger.LogUserMessage("Model are identical");
            }
            else
            {
                Logger.LogUserMessage("Model are different");
                Logger.LogUserMessage("###################");

                Logger.LogUserMessage("");
                Logger.LogUserMessage("Elements");
                Logger.LogUserMessage("--------");

                bool foundElementDeltas = false;
                if (_missingModel1ElementNames.Count > 0)
                {
                    foundElementDeltas = true;
                    Logger.LogUserMessage("Missing in model 1");
                    foreach (string missingElementName in _missingModel1ElementNames)
                    {
                        Logger.LogUserMessage($" {missingElementName}");
                    }
                    Logger.LogUserMessage("");
                }

                if (_missingModel2ElementNames.Count > 0)
                {
                    foundElementDeltas = true;
                    Logger.LogUserMessage("Missing in model 2");
                    foreach (string missingElementName in _missingModel2ElementNames)
                    {
                        Logger.LogUserMessage($" {missingElementName}");
                    }
                    Logger.LogUserMessage("");
                }

                if (!foundElementDeltas)
                {
                    Logger.LogUserMessage(" none");
                    Logger.LogUserMessage("");
                }

                Logger.LogUserMessage("Relations");
                Logger.LogUserMessage("--------");

                bool foundRelationDeltas = false;
                foreach (string elementName in _allElementNames)
                {
                    if (_missingModel1RelationProviderNames[elementName].Count > 0)
                    {
                        foundRelationDeltas = true;
                        Logger.LogUserMessage($"Missing in model 1 for {elementName}");
                        foreach (string missingTargetName in _missingModel1RelationProviderNames[elementName])
                        {
                            Logger.LogUserMessage($" {missingTargetName}");
                        }
                        Logger.LogUserMessage("");
                    }

                    if (_missingModel2RelationProviderNames[elementName].Count > 0)
                    {
                        foundRelationDeltas = true;
                        Logger.LogUserMessage($"Missing in model 2 for {elementName}");
                        foreach (string missingTargetName in _missingModel2RelationProviderNames[elementName])
                        {
                            Logger.LogUserMessage($" {missingTargetName}");
                        }
                        Logger.LogUserMessage("");
                    }
                }

                if (!foundRelationDeltas)
                {
                    Logger.LogUserMessage(" none");
                    Logger.LogUserMessage("");
                }
            }
        }

        private void FindElements()
        {
            foreach (IDsiElement element in _model1.GetElements())
            {
                _foundModel1ElementNames.Add(element.Name);
                _allElementNames.Add(element.Name);
            }

            foreach (IDsiElement element in _model2.GetElements())
            {
                _foundModel2ElementNames.Add(element.Name);
                _allElementNames.Add(element.Name);
            }
        }

        private void CompareElement(string elementName)
        {
            if (!_foundModel1ElementNames.Contains(elementName))
            {
                _missingModel1ElementNames.Add(elementName);
            }

            if (!_foundModel2ElementNames.Contains(elementName))
            {
                _missingModel2ElementNames.Add(elementName);
            }
        }

        private void CompareElementRelations(string consumerName)
        {
            _foundModel1RelationProviderNames[consumerName] = new HashSet<string>();
            _foundModel2RelationProviderNames[consumerName] = new HashSet<string>();
            _missingModel1RelationProviderNames[consumerName] = new HashSet<string>();
            _missingModel2RelationProviderNames[consumerName] = new HashSet<string>();
            _allRelationProviderNames[consumerName] = new HashSet<string>();

            foreach (IDsiRelation relation in _model1.GetRelations())
            {
                IDsiElement consumer = _model1.FindElementById(relation.ConsumerId);
                IDsiElement provider = _model1.FindElementById(relation.ProviderId);

                if (consumer.Name == consumerName)
                {
                    _foundModel1RelationProviderNames[consumerName].Add(provider.Name);
                    _allRelationProviderNames[consumerName].Add(provider.Name);
                }
            }

            foreach (IDsiRelation relation in _model2.GetRelations())
            {
                IDsiElement consumer = _model1.FindElementById(relation.ConsumerId);
                IDsiElement provider = _model1.FindElementById(relation.ProviderId);

                if (consumer.Name == consumerName)
                {
                    _foundModel2RelationProviderNames[consumerName].Add(provider.Name);
                    _allRelationProviderNames[consumerName].Add(provider.Name);
                }
            }

            foreach (string providerName in _allRelationProviderNames[consumerName])
            {
                if (!_foundModel1RelationProviderNames[consumerName].Contains(providerName))
                {
                    _missingModel1RelationProviderNames[consumerName].Add(providerName);
                }

                if (!_foundModel2RelationProviderNames[consumerName].Contains(providerName))
                {
                    _missingModel2RelationProviderNames[consumerName].Add(providerName);
                }
            }
        }

        private bool AreIdentical()
        {
            bool identical = true;

            if (_missingModel1ElementNames.Count > 0)
            {
                identical = false;
            }

            if (_missingModel2ElementNames.Count > 0)
            {
                identical = false;
            }

            foreach (string elementName in _allElementNames)
            {
                if (_missingModel1RelationProviderNames[elementName].Count > 0)
                {
                    identical = false;
                }

                if (_missingModel1RelationProviderNames[elementName].Count > 0)
                {
                    identical = false;
                }
            }
            return identical;
        }

        private void UpdateProgress(string actionText, int currentItemCount, int totalItemCount, string itemType)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = actionText;
            progressInfo.CurrentItemCount = currentItemCount;
            progressInfo.TotalItemCount = totalItemCount;
            progressInfo.ItemType = itemType;
            progressInfo.Percentage = currentItemCount * 100 / totalItemCount;
            progressInfo.Done = currentItemCount == totalItemCount;
            _progress?.Report(progressInfo);
        }
    }
}
