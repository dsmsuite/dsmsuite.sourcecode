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

            IDataModel model = new DataModel("Test", Assembly.GetExecutingAssembly());
            DotNet.Analysis.Analyzer analyzer = new DotNet.Analysis.Analyzer(model, analyzerSettings);
            analyzer.Analyze();

            // Main elements
            IElement elementMainClient = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MainClient");
            Assert.IsNotNull(elementMainClient);

            IElement elementMainType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MainType");
            Assert.IsNotNull(elementMainType);

            IElement elementMainTypeMyDelegate = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MainType/MyDelegate");
            Assert.IsNotNull(elementMainTypeMyDelegate);

            IElement elementMainTypeAnonymous = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MainType/<>c");
            Assert.IsNotNull(elementMainTypeAnonymous);

            IElement elementInterfaceA = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.InterfaceA");
            Assert.IsNotNull(elementInterfaceA);

            IElement elementBaseType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.BaseType");
            Assert.IsNotNull(elementBaseType);

            IElement elementNestedType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MainType/NestedType");
            Assert.IsNotNull(elementNestedType);

            // Fields
            IElement elementFieldType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.FieldType");
            Assert.IsNotNull(elementFieldType);

            IElement elementGenericFieldType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericFieldType`1");
            Assert.IsNotNull(elementGenericFieldType);

            IElement elementGenericFieldTypeParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericFieldTypeParameter");
            Assert.IsNotNull(elementGenericFieldTypeParameter);

            // Properties
            IElement elementPropertyType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.PropertyType");
            Assert.IsNotNull(elementPropertyType);

            IElement elementPropertyEnum = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.PropertyEnum");
            Assert.IsNotNull(elementPropertyEnum);

            IElement elementGenericPropertyType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericPropertyType`1");
            Assert.IsNotNull(elementGenericPropertyType);

            IElement elementGenericPropertyTypeParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericPropertyTypeParameter");
            Assert.IsNotNull(elementGenericPropertyTypeParameter);

            // Method variables
            IElement elementMethodVariableType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.MethodVariableType");
            Assert.IsNotNull(elementMethodVariableType);

            // Parameters
            IElement elementParameterEnum = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.ParameterEnum");
            Assert.IsNotNull(elementParameterEnum);

            IElement elementParameterType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.ParameterType");
            Assert.IsNotNull(elementParameterType);

            IElement elementGenericParameterType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericParameterType`1");
            Assert.IsNotNull(elementGenericParameterType);

            IElement elementGenericParameterTypeParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericParameterTypeParameter"); 
            Assert.IsNotNull(elementGenericParameterTypeParameter);

            // Return types
            IElement elementReturnType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.ReturnType"); 
            Assert.IsNotNull(elementReturnType);

            IElement elementReturnEnum = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.ReturnEnum"); 
            Assert.IsNotNull(elementReturnEnum);

            IElement elementGenericReturnType = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericReturnType`1"); 
            Assert.IsNotNull(elementGenericReturnType);

            IElement elementGenericReturnTypeParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.GenericReturnTypeParameter"); 
            Assert.IsNotNull(elementGenericReturnTypeParameter);

            IElement elementDelegateGenericParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.DelegateGenericParameter");
            Assert.IsNotNull(elementDelegateGenericParameter);

            IElement elementEventsArgsGenericParameter = model.FindElement("DsmSuite.Analyzer.DotNet.Test.Data.EventsArgsGenericParameter"); 
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
