using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Compare
{
    class Comparer
    {
        private readonly IDsiModel _model1;
        private readonly IDsiModel _model2;
        private readonly IProgress<ProgressInfo> _progress;

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

            int totalItemCount = _allElementNames.Count;

            int currentItemCount = 0;
            foreach (string elementName in _allElementNames)
            {
                CompareElement(elementName);

                currentItemCount++;
                UpdateProgress("Comparing elements", currentItemCount, totalItemCount, "elements");
            }

            currentItemCount = 0;
            foreach (string elementName in _allElementNames)
            {
                CompareElementRelations(elementName);

                currentItemCount++;
                UpdateProgress("Comparing element relations", currentItemCount, totalItemCount, "elements");
            }

            if (AreIdentical())
            {
                Logger.LogUserMessage("Model are identical");
            }
            else
            {
                Logger.LogUserMessage("Model are different");

                WriteMissingElementsToFile("elementDeltas1.txt", $"Missing elements in {_model1.Filename}", _missingModel1ElementNames);
                WriteMissingElementsToFile("elementDeltas2.txt", $"Missing elements in {_model2.Filename}", _missingModel2ElementNames);

                WriteMissingRelationsToFile("relationDeltas1.txt", $"Missing relations in {_model1.Filename}", _missingModel1RelationProviderNames);
                WriteMissingRelationsToFile("relationDeltas2.txt", $"Missing relations in {_model2.Filename}", _missingModel2RelationProviderNames);
            }
        }

        private void WriteMissingElementsToFile(string filename, string title, HashSet<string> missingModelElementNames)
        {
            FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
            using (StreamWriter writetext = new StreamWriter(fs))
            {
                writetext.WriteLine(title);
                writetext.WriteLine("");

                int deltaCount = missingModelElementNames.Count;

                foreach (string missingElementName in missingModelElementNames)
                {
                    writetext.WriteLine($" {missingElementName}");
                }

                writetext.WriteLine($" {deltaCount} missing elements found");
            }
        }

        private void WriteMissingRelationsToFile(string filename, string title, Dictionary<string, HashSet<string>> missingModelRelationProviderNames)
        {
            FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
            using (StreamWriter writetext = new StreamWriter(fs))
            {
                writetext.WriteLine(title);
                writetext.WriteLine("");

                int deltaCount = 0;
                foreach (string elementName in _allElementNames)
                {
                    deltaCount += missingModelRelationProviderNames[elementName].Count;

                    if (missingModelRelationProviderNames[elementName].Count > 0)
                    {
                        writetext.WriteLine($"For {elementName}:");
                        foreach (string missingTargetName in missingModelRelationProviderNames[elementName])
                        {
                            writetext.WriteLine($" {missingTargetName}");
                        }
                        writetext.WriteLine("");
                    }
                }

                writetext.WriteLine($" {deltaCount} missing relations found");
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

                if ((consumer != null) && (provider != null) && (consumer.Name == consumerName))
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
