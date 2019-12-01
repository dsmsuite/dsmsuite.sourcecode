using System.Reflection;
using DsmSuite.Analyzer.Jdeps.Settings;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.Jdeps.Test.Analysis
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
                InputFilename = "example.dot",
            };

            IDataModel model = new DataModel("Test", Assembly.GetExecutingAssembly());
            Jdeps.Analysis.Analyzer analyzer = new Jdeps.Analysis.Analyzer(model, analyzerSettings);
            analyzer.Analyze();

            Assert.AreEqual(5, model.TotalElementCount);

            IElement elementJavaxCryptoCipher = model.FindElement("javax.crypto.Cipher");
            Assert.IsNotNull(elementJavaxCryptoCipher);

            IElement elementJavaxCryptoCipherTransform = model.FindElement("javax.crypto.Cipher.Transform");
            Assert.IsNotNull(elementJavaxCryptoCipherTransform);

            IElement elementJavaxCryptoSpecRcsParameterSpec = model.FindElement("javax.crypto.spec.RC5ParameterSpec");
            Assert.IsNotNull(elementJavaxCryptoSpecRcsParameterSpec);

            IElement elementSunSecurityJcaGetInstance = model.FindElement("sun.security.jca.GetInstance");
            Assert.IsNotNull(elementSunSecurityJcaGetInstance);

            IElement elementJavaLangCharSequence = model.FindElement("java.lang.CharSequence");
            Assert.IsNotNull(elementJavaLangCharSequence);

            Assert.AreEqual(3, model.ResolvedRelationCount);
            Assert.IsTrue(model.DoesRelationExist(elementJavaxCryptoCipher, elementJavaxCryptoSpecRcsParameterSpec));
            Assert.IsTrue(model.DoesRelationExist(elementJavaxCryptoCipher, elementSunSecurityJcaGetInstance));
            Assert.IsTrue(model.DoesRelationExist(elementJavaxCryptoCipherTransform, elementJavaLangCharSequence));
        }
    }
}
