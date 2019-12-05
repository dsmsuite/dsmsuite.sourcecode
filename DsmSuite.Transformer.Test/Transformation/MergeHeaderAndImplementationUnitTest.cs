﻿using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Transformer.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Transformer.Test.Transformation
{
    [TestClass]
    public class MergeHeaderTransformationUnitTest
    {
        [TestMethod]
        public void MergeWhenImplementationDependsOnHeaderFileWithSameName()
        {
            DsiDataModel dataModel = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

            IDsiElement elementCpp = dataModel.CreateElement("namespace1.namespace1.element1Name.cpp", "class", "");
            IDsiElement elementH = dataModel.CreateElement("namespace3.namespace4.element1Name.h", "class", "");

            dataModel.AddRelation(elementCpp.Name, elementH.Name, "", 1, "context");

            Assert.IsNotNull(dataModel.FindElement("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNotNull(dataModel.FindElement("namespace3.namespace4.element1Name.h"));
            Assert.IsNull(dataModel.FindElement("namespace1.namespace1.element1Name.h"));
            
            MoveHeaderElementsAction transformation = new MoveHeaderElementsAction(dataModel, true);
            transformation.Execute();

            Assert.IsNotNull(dataModel.FindElement("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNull(dataModel.FindElement("namespace3.namespace4.element1Name.h"));
            Assert.IsNotNull(dataModel.FindElement("namespace1.namespace1.element1Name.h"));
        }

        [TestMethod]
        public void DoNotMergeWhenImplementationDependsOnHeaderFileWithOtherName()
        {
            DsiDataModel dataModel = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

            IDsiElement elementCpp = dataModel.CreateElement("namespace1.namespace1.element1Name.cpp", "class", "");
            IDsiElement elementH = dataModel.CreateElement("namespace3.namespace4.ELEMENT1NAME.h", "class", "");

            dataModel.AddRelation(elementCpp.Name, elementH.Name, "", 1, "context");

            Assert.IsNotNull(dataModel.FindElement("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNotNull(dataModel.FindElement("namespace3.namespace4.ELEMENT1NAME.h"));
            Assert.IsNull(dataModel.FindElement("namespace1.namespace1.element1Name.h"));

            MoveHeaderElementsAction transformation = new MoveHeaderElementsAction(dataModel, true);
            transformation.Execute();

            Assert.IsNotNull(dataModel.FindElement("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNotNull(dataModel.FindElement("namespace3.namespace4.ELEMENT1NAME.h"));
            Assert.IsNull(dataModel.FindElement("namespace1.namespace1.element1Name.h"));
        }

        [TestMethod]
        public void DoNotMergeWhenImplementationDoesNotDependOnHeaderFileWithSameName()
        {
            DsiDataModel dataModel = new DsiDataModel("Test", Assembly.GetExecutingAssembly());

            dataModel.CreateElement("namespace1.namespace1.element1Name.cpp", "class", "");
            dataModel.CreateElement("namespace3.namespace4.element1Name.h", "class", "");

            Assert.IsNotNull(dataModel.FindElement("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNotNull(dataModel.FindElement("namespace3.namespace4.element1Name.h"));
            Assert.IsNull(dataModel.FindElement("namespace1.namespace1.element1Name.h"));

            MoveHeaderElementsAction transformation = new MoveHeaderElementsAction(dataModel, true);
            transformation.Execute();

            Assert.IsNotNull(dataModel.FindElement("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNotNull(dataModel.FindElement("namespace3.namespace4.element1Name.h"));
            Assert.IsNull(dataModel.FindElement("namespace1.namespace1.element1Name.h"));
        }
    }
}
