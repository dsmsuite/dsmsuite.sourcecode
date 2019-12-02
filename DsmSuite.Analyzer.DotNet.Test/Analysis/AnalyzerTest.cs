using System.Collections.Generic;
using System.Reflection;
using DsmSuite.Analyzer.DotNet.Settings;
using DsmSuite.Analyzer.DotNet.Test.Util;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.DotNet.Test.Analysis
{
    [TestClass]
    public class AnalyzerTest
    {
        [TestMethod]
        public void TestAnalyze()
        {
            AnalyzerSettings analyzerSettings = new AnalyzerSettings
            {
                LoggingEnabled = true,
                AssemblyDirectory = TestData.RootDirectory,
                ExternalNames = new List<string>()
            };

            IDsiDataModel model = new DsiDataModel("Test", Assembly.GetExecutingAssembly());
            DotNet.Analysis.Analyzer analyzer = new DotNet.Analysis.Analyzer(model, analyzerSettings);
            analyzer.Analyze();

            // Main elements
            IDsiElement elementMainClient = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MainClient");
            Assert.IsNotNull(elementMainClient);

            IDsiElement elementMainType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MainType");
            Assert.IsNotNull(elementMainType);

            IDsiElement elementMainTypeMyDelegate = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MainType/MyDelegate");
            Assert.IsNotNull(elementMainTypeMyDelegate);

            IDsiElement elementMainTypeAnonymous = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MainType/<>c");
            Assert.IsNotNull(elementMainTypeAnonymous);

            IDsiElement elementInterfaceA = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.InterfaceA");
            Assert.IsNotNull(elementInterfaceA);

            IDsiElement elementBaseType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.BaseType");
            Assert.IsNotNull(elementBaseType);

            IDsiElement elementNestedType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MainType/NestedType");
            Assert.IsNotNull(elementNestedType);

            // Fields
            IDsiElement elementFieldType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.FieldType");
            Assert.IsNotNull(elementFieldType);

            IDsiElement elementGenericFieldType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericFieldType`1");
            Assert.IsNotNull(elementGenericFieldType);

            IDsiElement elementGenericFieldTypeParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericFieldTypeParameter");
            Assert.IsNotNull(elementGenericFieldTypeParameter);

            // Properties
            IDsiElement elementPropertyType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.PropertyType");
            Assert.IsNotNull(elementPropertyType);

            IDsiElement elementPropertyEnum = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.PropertyEnum");
            Assert.IsNotNull(elementPropertyEnum);

            IDsiElement elementGenericPropertyType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericPropertyType`1");
            Assert.IsNotNull(elementGenericPropertyType);

            IDsiElement elementGenericPropertyTypeParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericPropertyTypeParameter");
            Assert.IsNotNull(elementGenericPropertyTypeParameter);

            // Method variables
            IDsiElement elementMethodVariableType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MethodVariableType");
            Assert.IsNotNull(elementMethodVariableType);

            // Parameters
            IDsiElement elementParameterEnum = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.ParameterEnum");
            Assert.IsNotNull(elementParameterEnum);

            IDsiElement elementParameterType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.ParameterType");
            Assert.IsNotNull(elementParameterType);

            IDsiElement elementGenericParameterType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericParameterType`1");
            Assert.IsNotNull(elementGenericParameterType);

            IDsiElement elementGenericParameterTypeParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericParameterTypeParameter"); 
            Assert.IsNotNull(elementGenericParameterTypeParameter);

            // Return types
            IDsiElement elementReturnType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.ReturnType"); 
            Assert.IsNotNull(elementReturnType);

            IDsiElement elementReturnEnum = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.ReturnEnum"); 
            Assert.IsNotNull(elementReturnEnum);

            IDsiElement elementGenericReturnType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericReturnType`1"); 
            Assert.IsNotNull(elementGenericReturnType);

            IDsiElement elementGenericReturnTypeParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericReturnTypeParameter"); 
            Assert.IsNotNull(elementGenericReturnTypeParameter);

            IDsiElement elementDelegateGenericParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.DelegateGenericParameter");
            Assert.IsNotNull(elementDelegateGenericParameter);

            IDsiElement elementEventsArgsGenericParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.EventsArgsGenericParameter"); 
            Assert.IsNotNull(elementEventsArgsGenericParameter);

            // Main relations
            Assert.IsTrue(model.DoesRelationExist(elementMainClient, elementMainType)); 
            Assert.IsTrue(model.DoesRelationExist(elementMainClient, elementEventsArgsGenericParameter));

            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementNestedType));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementMainTypeAnonymous));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementMainTypeMyDelegate));

            Assert.IsTrue(model.DoesRelationExist(elementMainTypeAnonymous, elementMainTypeMyDelegate));

            Assert.IsTrue(model.DoesRelationExist(elementMainTypeAnonymous, elementDelegateGenericParameter));
            Assert.IsTrue(model.DoesRelationExist(elementMainTypeAnonymous, elementDelegateGenericParameter));

            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementInterfaceA)); 
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementBaseType)); 

            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementEventsArgsGenericParameter));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementDelegateGenericParameter));

            // Field relations
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementFieldType)); 
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementGenericFieldType)); 

            // Property relations
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementPropertyType));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementPropertyEnum));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementGenericPropertyType));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementGenericPropertyTypeParameter));

            // Method variable relations
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementMethodVariableType));

            // Parameters type relations
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementParameterEnum));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementParameterType));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementGenericParameterType));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementGenericParameterTypeParameter));

            Assert.IsTrue(model.DoesRelationExist(elementInterfaceA, elementParameterEnum));
            Assert.IsTrue(model.DoesRelationExist(elementInterfaceA, elementParameterType));
            Assert.IsTrue(model.DoesRelationExist(elementInterfaceA, elementGenericParameterType));
            Assert.IsTrue(model.DoesRelationExist(elementInterfaceA, elementGenericParameterTypeParameter));

            // Return type relations
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementReturnType));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementReturnEnum));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementGenericReturnType));
            Assert.IsTrue(model.DoesRelationExist(elementMainType, elementGenericReturnTypeParameter));

            Assert.IsTrue(model.DoesRelationExist(elementInterfaceA, elementReturnType));
            Assert.IsTrue(model.DoesRelationExist(elementInterfaceA, elementReturnEnum));
            Assert.IsTrue(model.DoesRelationExist(elementInterfaceA, elementGenericReturnType));
            Assert.IsTrue(model.DoesRelationExist(elementInterfaceA, elementGenericReturnTypeParameter));
        }
    }
}
