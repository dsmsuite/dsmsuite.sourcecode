﻿using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;
using DsmSuite.Transformer.Settings;

namespace DsmSuite.Transformer.Transformation
{
    public class MoveElementsAction : Action
    {
        private const string ActionName = "Move elements according specified rules";
        private readonly IDsiModel _model;
        private readonly List<MoveElementRule> _transformationRules;

        public MoveElementsAction(IDsiModel model, MoveElementsSettings moveElementsSettings) :
            base(ActionName, moveElementsSettings.Enabled)
        {
            _model = model;
            _transformationRules = moveElementsSettings.Rules;
        }

        protected override void ExecuteImpl()
        {
            int transformedElements = 0;

            IDsiElement[] clonedElements = _model.GetElements().ToArray(); // Because elements in collection change during iteration
            foreach (IDsiElement element in clonedElements)
            {
                MoveElement(element);

                transformedElements++;
                Console.Write("\r progress elements={0}", transformedElements);
            }
            Console.WriteLine("\r progress elements={0}", transformedElements);
        }

        private void MoveElement(IDsiElement element)
        {
            foreach (MoveElementRule rule in _transformationRules)
            {
                if (element.Name.Contains(rule.From))
                {
                    string elementName = element.Name;
                    string newElementName = element.Name.ReplaceIgnoreCase(rule.From, rule.To);

                    _model.RenameElement(element, newElementName);

                    string description = elementName + "->" + newElementName;
                    AnalyzerLogger.LogTransformation(ActionName, description);
                }
            }
        }
    }
}
