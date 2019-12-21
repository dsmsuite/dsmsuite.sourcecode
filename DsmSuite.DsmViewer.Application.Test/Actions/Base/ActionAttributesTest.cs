using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsmSuite.DsmViewer.Application.Actions.Base;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Base
{
    [TestClass]
    public class ActionAttributesTest
    {
        [TestMethod]
        public void WhenSetStringIsCalledThenValueIsInDictionary()
        {
            ActionAttributes attributes = new ActionAttributes();

            string key = "some_key";
            string value = "some_value";
            attributes.SetString(key, value);

            IReadOnlyDictionary<string, string> data = attributes.GetData();
            Assert.AreEqual(1, data.Count);
            Assert.IsTrue(data.ContainsKey(key));
            Assert.AreEqual(value, data[key]);
        }

        [TestMethod]
        public void WhenSetIntIsCalledThenValueIsInDictionary()
        {
            ActionAttributes attributes = new ActionAttributes();

            string key = "some_key";
            int value = 7;
            attributes.SetInt(key, value);

            IReadOnlyDictionary<string, string> data = attributes.GetData();
            Assert.AreEqual(1, data.Count);
            Assert.IsTrue(data.ContainsKey(key));
            Assert.AreEqual(value.ToString(), data[key]);
        }

        [TestMethod]
        public void WhenSetNullableIntIsCalledWithNotNullThenValueIsInDictionary()
        {
            ActionAttributes attributes = new ActionAttributes();

            string key = "some_key";
            int value = 7;
            attributes.SetNullableInt(key, value);

            IReadOnlyDictionary<string, string> data = attributes.GetData();
            Assert.AreEqual(1, data.Count);
            Assert.IsTrue(data.ContainsKey(key));
            Assert.AreEqual(value.ToString(), data[key]);
        }

        [TestMethod]
        public void WhenSetNullableIntIsCalledWitNullThenValueIsNotInDictionary()
        {
            ActionAttributes attributes = new ActionAttributes();

            string key = "some_key";
            attributes.SetNullableInt(key, null);

            IReadOnlyDictionary<string, string> data = attributes.GetData();
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void WhenSetNullableIntIsCalledWitMultipleValuesThenAllValuesAreInDictionary()
        {
            ActionAttributes attributes = new ActionAttributes();

            string key1 = "some_key1";
            string value1 = "some_value1";
            attributes.SetString(key1, value1);

            string key2 = "some_key2";
            int value2 = 7;
            attributes.SetInt(key2, value2);

            IReadOnlyDictionary<string, string> data = attributes.GetData();
            Assert.AreEqual(2, data.Count);
            Assert.IsTrue(data.ContainsKey(key1));
            Assert.AreEqual(value1, data[key1]);
            Assert.IsTrue(data.ContainsKey(key2));
            Assert.AreEqual(value2.ToString(), data[key2]);
        }
    }
}
