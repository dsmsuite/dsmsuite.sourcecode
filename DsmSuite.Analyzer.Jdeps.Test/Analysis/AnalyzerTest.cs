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

            IDsiModel model = new DsiModel("Test", Assembly.GetExecutingAssembly());
            Jdeps.Analysis.Analyzer analyzer = new Jdeps.Analysis.Analyzer(model, analyzerSettings);
            analyzer.Analyze(null);

            Assert.AreEqual(5, model.TotalElementCount);

            IDsiElement elementJavaxCryptoCipher = model.FindElementByName("javax.crypto.Cipher");
            Assert.IsNotNull(elementJavaxCryptoCipher);

            IDsiElement elementJavaxCryptoCipherTransform = model.FindElementByName("javax.crypto.Cipher.Transform");
            Assert.IsNotNull(elementJavaxCryptoCipherTransform);

            IDsiElement elementJavaxCryptoSpecRcsParameterSpec = model.FindElementByName("javax.crypto.spec.RC5ParameterSpec");
            Assert.IsNotNull(elementJavaxCryptoSpecRcsParameterSpec);

            IDsiElement elementSunSecurityJcaGetInstance = model.FindElementByName("sun.security.jca.GetInstance");
            Assert.IsNotNull(elementSunSecurityJcaGetInstance);

            IDsiElement elementJavaLangCharSequence = model.FindElementByName("java.lang.CharSequence");
            Assert.IsNotNull(elementJavaLangCharSequence);

            Assert.AreEqual(3, model.ResolvedRelationCount);
            Assert.IsTrue(model.DoesRelationExist(elementJavaxCryptoCipher.Id, elementJavaxCryptoSpecRcsParameterSpec.Id));
            Assert.IsTrue(model.DoesRelationExist(elementJavaxCryptoCipher.Id, elementSunSecurityJcaGetInstance.Id));
            Assert.IsTrue(model.DoesRelationExist(elementJavaxCryptoCipherTransform.Id, elementJavaLangCharSequence.Id));
        }
    }
}
