using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Common.Util.Test
{
    [TestClass]
    public class ElementNameTest
    {
        [TestMethod]
        public void When_ElementNameIsConstructedWithNoArgument_Then_ItsHasOneNamePartWhichIsAnEmptyString()
        {
            ElementName elementName = new ElementName();
            Assert.AreEqual("", elementName.FullName);
            Assert.AreEqual("", elementName.ParentName);
            Assert.AreEqual("", elementName.LastNamePart);
            Assert.AreEqual(1, elementName.NamePartCount);
        }

        [TestMethod]
        public void When_ElementNameIsConstructedWithSingleMultiPartArgument_Then_ItsHasMutipleNameParts()
        {
            ElementName elementName = new ElementName("a.b.c");
            Assert.AreEqual("a.b.c", elementName.FullName);
            Assert.AreEqual("a.b", elementName.ParentName);
            Assert.AreEqual("c", elementName.LastNamePart);
            Assert.AreEqual(3, elementName.NamePartCount);
            Assert.AreEqual("a", elementName.NameParts[0]);
            Assert.AreEqual("b", elementName.NameParts[1]);
            Assert.AreEqual("c", elementName.NameParts[2]);
        }

        [TestMethod]
        public void When_ElementNameIsConstructedWithTwoArguments_Then_ItsHasTheJoinedMultipleNameParts()
        {
            ElementName elementName = new ElementName("a.b", "c");
            Assert.AreEqual("a.b.c", elementName.FullName);
            Assert.AreEqual("a.b", elementName.ParentName);
            Assert.AreEqual("c", elementName.LastNamePart);
            Assert.AreEqual(3, elementName.NamePartCount);
            Assert.AreEqual("a", elementName.NameParts[0]);
            Assert.AreEqual("b", elementName.NameParts[1]);
            Assert.AreEqual("c", elementName.NameParts[2]);
        }

        [TestMethod]
        public void Given_AnEmptyElementName_When_AddPartIsCalled_Then_ItsHasOneNamePart()
        {
            ElementName elementName = new ElementName();

            elementName.AddNamePart("a");
            Assert.AreEqual("a", elementName.FullName);
            Assert.AreEqual("", elementName.ParentName);
            Assert.AreEqual("a", elementName.LastNamePart);
            Assert.AreEqual(1, elementName.NamePartCount);
            Assert.AreEqual("a", elementName.NameParts[0]);
        }

        [TestMethod]
        public void Given_FilledElementName_When_AddPartIsCalled_Then_ItsHasOneAdditionalNamePart()
        {
            ElementName elementName = new ElementName("a.b");
            elementName.AddNamePart("c");
            Assert.AreEqual("a.b.c", elementName.FullName);
            Assert.AreEqual("a.b", elementName.ParentName);
            Assert.AreEqual("c", elementName.LastNamePart);
            Assert.AreEqual(3, elementName.NamePartCount);
            Assert.AreEqual("a", elementName.NameParts[0]);
            Assert.AreEqual("b", elementName.NameParts[1]);
            Assert.AreEqual("c", elementName.NameParts[2]);
        }
    }
}
