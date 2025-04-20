using DsmSuite.Common.Model.Core;
using DsmSuite.Common.Model.Interface;
using System.Reflection;

namespace DsmSuite.Common.Model.Test.Core
{
    [TestClass]
    public class MetaDataModelTest
    {
        [TestMethod]
        public void When_ModelIsConstructed_Then_OnlyDefaultGroupAndItemsArePresent()
        {
            string defaultGroupName = "SomeGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());

            List<string> groups = model.GetExportedMetaDataGroups().ToList();
            Assert.AreEqual(1, groups.Count);
            Assert.AreEqual(defaultGroupName, groups[0]);

            List<IMetaDataItem> items = model.GetExportedMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(1, items.Count);
            Assert.IsTrue(items[0].Name == "Executable");
            Assert.IsTrue(items[0].Value.Contains("DsmSuite.Common.Model.Test"));
        }

        [TestMethod]
        public void GivenItemNameNotUsedBeforeWhenAddMetaDataItemIsCalledThenItemIsAdded()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());
            List<IMetaDataItem> itemsBefore = model.GetExportedMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(1, itemsBefore.Count);

            string name = "SomeItemName";
            string value = "SomeItemValue";
            model.AddMetaDataItemToDefaultGroup(name, value);

            List<IMetaDataItem> itemsAfter = model.GetExportedMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(2, itemsAfter.Count);
            Assert.AreEqual(itemsBefore[0].Name, itemsAfter[0].Name);
            Assert.AreEqual(itemsBefore[0].Value, itemsAfter[0].Value);
            Assert.AreEqual(name, itemsAfter[1].Name);
            Assert.AreEqual(value, itemsAfter[1].Value);
        }

        [TestMethod]
        public void GivenItemNamesNotUsedBeforeWhenAddMetaDataItemToDefaultGroupIsCalledTwiceThenTwoItemIAreAddedAndOrderIsMaintained()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());
            List<IMetaDataItem> itemsBefore = model.GetExportedMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(1, itemsBefore.Count);

            string name1 = "SomeItemName1";
            string value1 = "SomeItemValue1";
            model.AddMetaDataItemToDefaultGroup(name1, value1);

            string name2 = "SomeItemName2";
            string value2 = "SomeItemValue2";
            model.AddMetaDataItemToDefaultGroup(name2, value2);

            List<IMetaDataItem> itemsAfter = model.GetExportedMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(3, itemsAfter.Count);
            Assert.AreEqual(itemsBefore[0].Name, itemsAfter[0].Name);
            Assert.AreEqual(itemsBefore[0].Value, itemsAfter[0].Value);
            Assert.AreEqual(name1, itemsAfter[1].Name);
            Assert.AreEqual(value1, itemsAfter[1].Value);
            Assert.AreEqual(name2, itemsAfter[2].Name);
            Assert.AreEqual(value2, itemsAfter[2].Value);
        }

        [TestMethod]
        public void GivenItemNameUsedBeforeWhenAddMetaDataItemToDefaultGroupIsCalledThenItemIsUpdated()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());
            List<IMetaDataItem> itemsBefore = model.GetExportedMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(1, itemsBefore.Count);

            string name = "SomeItemName";
            string value1 = "SomeItemValue1";
            model.AddMetaDataItemToDefaultGroup(name, value1);

            string value2 = "SomeItemValue2";
            model.AddMetaDataItemToDefaultGroup(name, value2);

            List<IMetaDataItem> itemsAfter = model.GetExportedMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(2, itemsAfter.Count);
            Assert.AreEqual(itemsBefore[0].Name, itemsAfter[0].Name);
            Assert.AreEqual(itemsBefore[0].Value, itemsAfter[0].Value);
            Assert.AreEqual(name, itemsAfter[1].Name);
            Assert.AreEqual(value2, itemsAfter[1].Value);
        }

        [TestMethod]
        public void WhenAddMetaDataItemIsCalledThenGroupIsAdded()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());
            List<string> groupsBefore = model.GetExportedMetaDataGroups().ToList();
            Assert.AreEqual(1, groupsBefore.Count);

            string groupName = "ImportedGroupName";
            string name = "SomeItemName";
            string value = "SomeItemValue1";
            model.AddMetaDataItem(groupName, name, value);

            List<string> groupsAfter = model.GetExportedMetaDataGroups().ToList();
            Assert.AreEqual(2, groupsAfter.Count);
            Assert.AreEqual(groupsBefore[0], groupsAfter[0]);
            Assert.AreEqual(groupName, groupsAfter[1]);
        }

        [TestMethod]
        public void WhenAddMetaDataItemIsCalledThenItemIsAdded()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());

            string groupName = "ImportedGroupName";
            string name = "SomeItemName";
            string value = "SomeItemValue1";
            model.AddMetaDataItem(groupName, name, value);

            List<IMetaDataItem> itemsAfter = model.GetExportedMetaDataGroupItems(groupName).ToList();
            Assert.AreEqual(1, itemsAfter.Count);
            Assert.AreEqual(name, itemsAfter[0].Name);
            Assert.AreEqual(value, itemsAfter[0].Value);
        }


        [TestMethod]
        public void WhenAddMetaDataItemIsCalledTwiceThenTwoItemIAreAddedAndOrderIsMaintained()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());

            string groupName = "ImportedGroupName";
            string name1 = "SomeItemName1";
            string value1 = "SomeItemValue2";
            model.AddMetaDataItem(groupName, name1, value1);
            string name2 = "SomeItemName";
            string value2 = "SomeItemValue1";
            model.AddMetaDataItem(groupName, name2, value2);

            List<IMetaDataItem> itemsAfter = model.GetExportedMetaDataGroupItems(groupName).ToList();
            Assert.AreEqual(2, itemsAfter.Count);
            Assert.AreEqual(name1, itemsAfter[0].Name);
            Assert.AreEqual(value1, itemsAfter[0].Value);
            Assert.AreEqual(name2, itemsAfter[1].Name);
            Assert.AreEqual(value2, itemsAfter[1].Value);
        }

        [TestMethod]
        public void Given_MetaDataIsPresent_When_ModelIsCleared_Then_OnlyDefaultGroupAndItemsArePresent()
        {
            string defaultGroupName = "DefaultGroupName";
            MetaDataModel model = new MetaDataModel(defaultGroupName, Assembly.GetExecutingAssembly());

            string groupName = "ImportedGroupName";
            string name1 = "SomeItemName1";
            string value1 = "SomeItemValue2";
            model.AddMetaDataItem(groupName, name1, value1);
            string name2 = "SomeItemName";
            string value2 = "SomeItemValue1";
            model.AddMetaDataItem(groupName, name2, value2);

            List<string> groupsBefore = model.GetExportedMetaDataGroups().ToList();
            Assert.AreEqual(2, groupsBefore.Count);

            model.Clear();

            List<string> groupsAfter = model.GetExportedMetaDataGroups().ToList();
            Assert.AreEqual(1, groupsAfter.Count);

            List<IMetaDataItem> items = model.GetExportedMetaDataGroupItems(defaultGroupName).ToList();
            Assert.AreEqual(1, items.Count);
            Assert.IsTrue(items[0].Name == "Executable");
            Assert.IsTrue(items[0].Value.Contains("DsmSuite.Common.Model.Test"));
        }
    }
}

