using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Actions.Base;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Base
{
    [TestClass]
    public class ReadOnlyActionAttributesTest
    {
        [TestMethod]
        public void GivenStringValueInDictionaryWhenGetStringIsCalledThenValueIsReturned()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key = "some_key";
            string value = "some_value";
            data[key] = value;

            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            Assert.AreEqual(value, attributes.GetString(key));
        }

        [TestMethod]
        public void GivenIntValueInDictionaryWhenGetIntIsCalledThenValueIsReturned()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key = "some_key";
            int value = 7;
            data[key] = value.ToString();

            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            Assert.AreEqual(7, attributes.GetInt(key));
        }

        [TestMethod]
        public void GivenNullableIntInDictionaryWhenGetNullableIntIsCalledThenValueIsReturned()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key = "some_key";
            int value = 7;
            data[key] = value.ToString();

            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int? readValue = attributes.GetNullableInt(key);
            Assert.IsTrue(readValue.HasValue);
            Assert.AreEqual(7, readValue.Value);
        }

        [TestMethod]
        public void GivenNullableIntNotInDictionaryWhenGetNullableIntIsCalledThenNullValueIsReturned()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key = "some_key";

            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int? readValue = attributes.GetNullableInt(key);
            Assert.IsFalse(readValue.HasValue);
        }

        [TestMethod]
        public void GivenMultipleValuesInDictionaryWhenGetValueIsCalledThenAllAreReturned()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key1 = "some_key1";
            string value1 = "some_value1";
            data[key1] = value1;

            string key2 = "some_key2";
            int value2 = 7;
            data[key2] = value2.ToString();

            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            Assert.AreEqual(value1, attributes.GetString(key1));
            Assert.AreEqual(value2, attributes.GetInt(key2));
        }
    }
}
