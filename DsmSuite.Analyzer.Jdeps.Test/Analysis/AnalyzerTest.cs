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

            IDsiDataModel model = new DsiDataModel("Test", Assembly.GetExecutingAssembly());
            Jdeps.Analysis.Analyzer analyzer = new Jdeps.Analysis.Analyzer(model, analyzerSettings);
            analyzer.Analyze();

            Assert.AreEqual(5, model.TotalElementCount);

            IDsiElement elementJavaxCryptoCipher = model.FindElement("javax.crypto.Cipher");
            Assert.IsNotNull(elementJavaxCryptoCipher);

            IDsiElement elementJavaxCryptoCipherTransform = model.FindElement("javax.crypto.Cipher.Transform");
            Assert.IsNotNull(elementJavaxCryptoCipherTransform);

            IDsiElement elementJavaxCryptoSpecRcsParameterSpec = model.FindElement("javax.crypto.spec.RC5ParameterSpec");
            Assert.IsNotNull(elementJavaxCryptoSpecRcsParameterSpec);

            IDsiElement elementSunSecurityJcaGetInstance = model.FindElement("sun.security.jca.GetInstance");
            Assert.IsNotNull(elementSunSecurityJcaGetInstance);

            IDsiElement elementJavaLangCharSequence = model.FindElement("java.lang.CharSequence");
            Assert.IsNotNull(elementJavaLangCharSequence);

            Assert.AreEqual(3, model.ResolvedRelationCount);
            Assert.IsTrue(model.DoesRelationExist(elementJavaxCryptoCipher, elementJavaxCryptoSpecRcsParameterSpec));
            Assert.IsTrue(model.DoesRelationExist(elementJavaxCryptoCipher, elementSunSecurityJcaGetInstance));
            Assert.IsTrue(model.DoesRelationExist(elementJavaxCryptoCipherTransform, elementJavaLangCharSequence));
        }
    }
}
