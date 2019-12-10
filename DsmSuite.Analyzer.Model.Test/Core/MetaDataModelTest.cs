using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DsmSuite.Analyzer.Model.Test.Core
{
    [TestClass]
    public class MetaDataModelTest
    {
        [TestMethod]
        public void When_ModelIsConstructed_Then_OnlyDefaultGroupIsPresent()
        {
            string defaultGroupName = "SomeGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());
            List<string> groups = model.GetMetaDataGroups().ToList();
            Assert.AreEqual(1, groups.Count);
            Assert.AreEqual(defaultGroupName, groups[0]);
        }

        [TestMethod]
        public void When_ModelIsConstructed_Then_OnlyExecutableItemIsPresent()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());
            List<IDsiMetaDataItem> items = model.GetMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(1, items.Count);
            Assert.IsTrue(items[0].Name == "Executable");
            Assert.IsTrue(items[0].Value.Contains("DsmSuite.Analyzer.Model.Test"));
        }

        [TestMethod]
        public void Given_ItemNameNotUsedBefore_When_AddMetaDataItemIsCalled_Then_ItemIsAdded()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());
            List<IDsiMetaDataItem> itemsBefore = model.GetMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(1, itemsBefore.Count);

            string name = "SomeItemName";
            string value = "SomeItemValue";
            model.AddMetaDataItem(name, value);

            List<IDsiMetaDataItem> itemsAfter = model.GetMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(2, itemsAfter.Count);
            Assert.AreEqual(itemsBefore[0].Name, itemsAfter[0].Name);
            Assert.AreEqual(itemsBefore[0].Value, itemsAfter[0].Value);
            Assert.AreEqual(name, itemsAfter[1].Name);
            Assert.AreEqual(value, itemsAfter[1].Value);
        }


        [TestMethod]
        public void Given_ItemNamesNotUsedBefore_When_AddMetaDataItemIsCalledTwice_Then_TwoItemIAreAddedAndOrderIsMaintained()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());
            List<IDsiMetaDataItem> itemsBefore = model.GetMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(1, itemsBefore.Count);

            string name1 = "SomeItemName1";
            string value1 = "SomeItemValue1";
            model.AddMetaDataItem(name1, value1);

            string name2 = "SomeItemName2";
            string value2 = "SomeItemValue2";
            model.AddMetaDataItem(name2, value2);

            List<IDsiMetaDataItem> itemsAfter = model.GetMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(3, itemsAfter.Count);
            Assert.AreEqual(itemsBefore[0].Name, itemsAfter[0].Name);
            Assert.AreEqual(itemsBefore[0].Value, itemsAfter[0].Value);
            Assert.AreEqual(name1, itemsAfter[1].Name);
            Assert.AreEqual(value1, itemsAfter[1].Value);
            Assert.AreEqual(name2, itemsAfter[2].Name);
            Assert.AreEqual(value2, itemsAfter[2].Value);
        }

        [TestMethod]
        public void Given_ItemNameUsedBefore_When_AddMetaDataItemIsCalled_Then_ItemIsUpdated()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());
            List<IDsiMetaDataItem> itemsBefore = model.GetMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(1, itemsBefore.Count);

            string name = "SomeItemName";
            string value1 = "SomeItemValue1";
            model.AddMetaDataItem(name, value1);

            string value2 = "SomeItemValue2";
            model.AddMetaDataItem(name, value2);

            List<IDsiMetaDataItem> itemsAfter = model.GetMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(2, itemsAfter.Count);
            Assert.AreEqual(itemsBefore[0].Name, itemsAfter[0].Name);
            Assert.AreEqual(itemsBefore[0].Value, itemsAfter[0].Value);
            Assert.AreEqual(name, itemsAfter[1].Name);
            Assert.AreEqual(value2, itemsAfter[1].Value);
        }

        [TestMethod]
        public void When_ImportMetaDataItemIsCalled_Then_GroupIsAdded()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());
            List<string> groupsBefore = model.GetMetaDataGroups().ToList();
            Assert.AreEqual(1, groupsBefore.Count);

            string importedGroupName = "ImportedGroupName";
            string name = "SomeItemName";
            string value = "SomeItemValue1";
            model.ImportMetaDataItem(importedGroupName, name, value);

            List<string> groupsAfter = model.GetMetaDataGroups().ToList();
            Assert.AreEqual(2, groupsAfter.Count);
            Assert.AreEqual(groupsBefore[0], groupsAfter[0]);
            Assert.AreEqual(importedGroupName, groupsAfter[1]);
        }

        [TestMethod]
        public void When_ImportMetaDataItemIsCalled_Then_ItemIsAdded()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());

            string importedGroupName = "ImportedGroupName";
            string name = "SomeItemName";
            string value = "SomeItemValue1";
            model.ImportMetaDataItem(importedGroupName, name, value);

            List<IDsiMetaDataItem> itemsAfter = model.GetMetaDataGroupItems(importedGroupName).ToList();
            Assert.AreEqual(1, itemsAfter.Count);
            Assert.AreEqual(name, itemsAfter[0].Name);
            Assert.AreEqual(value, itemsAfter[0].Value);
         }


        [TestMethod]
        public void When_ImportMetaDataItemIsCalledTwice_Then_TwoItemIAreAddedAndOrderIsMaintained()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());

            string importedGroupName = "ImportedGroupName";
            string name1 = "SomeItemName1";
            string value1 = "SomeItemValue2";
            model.ImportMetaDataItem(importedGroupName, name1, value1);
            string name2 = "SomeItemName";
            string value2 = "SomeItemValue1";
            model.ImportMetaDataItem(importedGroupName, name2, value2);

            List<IDsiMetaDataItem> itemsAfter = model.GetMetaDataGroupItems(importedGroupName).ToList();
            Assert.AreEqual(2, itemsAfter.Count);
            Assert.AreEqual(name1, itemsAfter[0].Name);
            Assert.AreEqual(value1, itemsAfter[0].Value);
            Assert.AreEqual(name2, itemsAfter[1].Name);
            Assert.AreEqual(value2, itemsAfter[1].Value);
        }
    }
}

