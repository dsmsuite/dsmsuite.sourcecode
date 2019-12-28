using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Actions.Base;

namespace DsmSuite.DsmViewer.Application.Test.Actions.Base
{
    [TestClass]
    public class ActionReadOnlyAttributesTest
    {
        [TestMethod]
        public void GivenStringValueInDictionaryWhenGetStringIsCalledThenValueIsReturned()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key = "name";
            string memberValue = "value";
            data[key] = memberValue;

            string memberName = "_name";
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            Assert.AreEqual(memberValue, attributes.GetString(memberName));
        }

        [TestMethod]
        public void GivenIntValueInDictionaryWhenGetIntIsCalledThenValueIsReturned()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key = "name";
            int memberValue = 7;
            data[key] = memberValue.ToString();

            string memberName = "_name";
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            Assert.AreEqual(7, attributes.GetInt(memberName));
        }

        [TestMethod]
        public void GivenNullableIntInDictionaryWhenGetNullableIntIsCalledThenValueIsReturned()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key = "name";
            int memberValue = 7;
            data[key] = memberValue.ToString();

            string memberName = "_name";
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            int? readValue = attributes.GetNullableInt(memberName);
            Assert.IsTrue(readValue.HasValue);
            Assert.AreEqual(7, readValue.Value);
        }

        [TestMethod]
        public void GivenNullableIntNotInDictionaryWhenGetNullableIntIsCalledThenNullValueIsReturned()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string memberName = "_name";
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            int? readValue = attributes.GetNullableInt(memberName);
            Assert.IsFalse(readValue.HasValue);
        }

        [TestMethod]
        public void GivenMultipleValuesInDictionaryWhenGetValueIsCalledThenAllAreReturned()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string key1 = "name1";
            string memberValue1 = "some_value1";
            data[key1] = memberValue1;

            string key2 = "name2";
            int memberValue2 = 7;
            data[key2] = memberValue2.ToString();

            string memberName1 = "_name1";
            string memberName2 = "_name2";
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            Assert.AreEqual(memberValue1, attributes.GetString(memberName1));
            Assert.AreEqual(memberValue2, attributes.GetInt(memberName2));
        }
    }
}
