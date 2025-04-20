using DsmSuite.DsmViewer.Model.Core;

namespace DsmSuite.DsmViewer.Model.Test.Core
{
    [TestClass]
    public class NameRegistrationTest
    {
        [TestMethod]
        public void TestNameRegistration()
        {
            string typeA = "type1";
            string typeB = "type2";
            string typeC = "type3";
            NameRegistration typeRegistration = new NameRegistration();
            char ida = typeRegistration.RegisterName(typeA);
            char idb = typeRegistration.RegisterName(typeB);
            char idc = typeRegistration.RegisterName(typeC);
            Assert.AreEqual(typeA, typeRegistration.GetRegisteredName(ida));
            Assert.AreEqual(typeB, typeRegistration.GetRegisteredName(idb));
            Assert.AreEqual(typeC, typeRegistration.GetRegisteredName(idc));
        }
    }
}
