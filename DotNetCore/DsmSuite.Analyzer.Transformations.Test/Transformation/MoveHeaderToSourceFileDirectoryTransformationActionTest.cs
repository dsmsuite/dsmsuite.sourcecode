using System.Collections.Generic;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Transformations.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Transformations.Test.Transformation
{
    [TestClass]
    public class MergeHeaderTransformationUnitTest
    {
        [TestMethod]
        public void MergeWhenImplementationDependsOnHeaderFileWithSameName()
        {
            DsiModel dataModel = new DsiModel("Test", new List<string>(), Assembly.GetExecutingAssembly());

            IDsiElement elementCpp = dataModel.AddElement("namespace1.namespace1.element1Name.cpp", "class", null);
            IDsiElement elementH = dataModel.AddElement("namespace3.namespace4.element1Name.h", "class", null);

            dataModel.AddRelation(elementCpp.Name, elementH.Name, "", 1, null);

            Assert.IsNotNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNotNull(dataModel.FindElementByName("namespace3.namespace4.element1Name.h"));
            Assert.IsNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.h"));
            
            MoveHeaderToSourceFileDirectoryTransformationAction transformation = new MoveHeaderToSourceFileDirectoryTransformationAction(dataModel, null);
            transformation.Execute();

            Assert.IsNotNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNull(dataModel.FindElementByName("namespace3.namespace4.element1Name.h"));
            Assert.IsNotNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.h"));
        }

        [TestMethod]
        public void DoNotMergeWhenImplementationDependsOnHeaderFileWithOtherName()
        {
            DsiModel dataModel = new DsiModel("Test", new List<string>(), Assembly.GetExecutingAssembly());

            IDsiElement elementCpp = dataModel.AddElement("namespace1.namespace1.element1Name.cpp", "class", null);
            IDsiElement elementH = dataModel.AddElement("namespace3.namespace4.ELEMENT1NAME.h", "class", null);

            dataModel.AddRelation(elementCpp.Name, elementH.Name, "", 1, null);

            Assert.IsNotNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNotNull(dataModel.FindElementByName("namespace3.namespace4.ELEMENT1NAME.h"));
            Assert.IsNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.h"));

            MoveHeaderToSourceFileDirectoryTransformationAction transformation = new MoveHeaderToSourceFileDirectoryTransformationAction(dataModel, null);
            transformation.Execute();

            Assert.IsNotNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNotNull(dataModel.FindElementByName("namespace3.namespace4.ELEMENT1NAME.h"));
            Assert.IsNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.h"));
        }

        [TestMethod]
        public void DoNotMergeWhenImplementationDoesNotDependOnHeaderFileWithSameName()
        {
            DsiModel dataModel = new DsiModel("Test", new List<string>(), Assembly.GetExecutingAssembly());

            dataModel.AddElement("namespace1.namespace1.element1Name.cpp", "class", null);
            dataModel.AddElement("namespace3.namespace4.element1Name.h", "class", null);

            Assert.IsNotNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNotNull(dataModel.FindElementByName("namespace3.namespace4.element1Name.h"));
            Assert.IsNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.h"));

            MoveHeaderToSourceFileDirectoryTransformationAction transformation = new MoveHeaderToSourceFileDirectoryTransformationAction(dataModel, null);
            transformation.Execute();

            Assert.IsNotNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.cpp"));
            Assert.IsNotNull(dataModel.FindElementByName("namespace3.namespace4.element1Name.h"));
            Assert.IsNull(dataModel.FindElementByName("namespace1.namespace1.element1Name.h"));
        }
    }
}
