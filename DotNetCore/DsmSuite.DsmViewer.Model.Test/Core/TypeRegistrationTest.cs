using DsmSuite.DsmViewer.Model.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Core
{
    [TestClass]
    public class TypeRegistrationTest
    {
        [TestMethod]
        public void TestTypeRegistration()
        {
            string typeA = "type1";
            string typeB = "type2";
            string typeC = "type3";
            TypeRegistration typeRegistration = new TypeRegistration();
            char ida = typeRegistration.AddTypeName(typeA);
            char idb = typeRegistration.AddTypeName(typeB);
            char idc = typeRegistration.AddTypeName(typeC);
            Assert.AreEqual(typeA, typeRegistration.GetTypeName(ida));
            Assert.AreEqual(typeB, typeRegistration.GetTypeName(idb));
            Assert.AreEqual(typeC, typeRegistration.GetTypeName(idc));
        }
    }
}
