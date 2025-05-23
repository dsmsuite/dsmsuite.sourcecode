﻿using DsmSuite.Analyzer.Jdeps.Settings;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using System.Reflection;

namespace DsmSuite.Analyzer.Jdeps.Test.Analysis
{
    [TestClass]
    public class AnalyzerTest
    {
        [TestMethod]
        public void TestAnalyze()
        {
            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            analyzerSettings.Input.DotFileDirectory = ".";

            IDsiModel model = new DsiModel("Test", analyzerSettings.Transformation.IgnoredNames, Assembly.GetExecutingAssembly());
            Jdeps.Analysis.Analyzer analyzer = new Jdeps.Analysis.Analyzer(model, analyzerSettings, null);
            analyzer.Analyze();

            Assert.AreEqual(5, model.CurrentElementCount);

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
