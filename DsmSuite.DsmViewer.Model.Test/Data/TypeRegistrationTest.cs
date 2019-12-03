using DsmSuite.DsmViewer.Model.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.DsmViewer.Model.Test.Data
{
    [TestClass]
    public class TypeRegistrationTest
    {
        [TestMethod]
        public void TestTypeRegistration()
        {
            string typea = "typea";
            string typeb = "typeb";
            string typec = "typec";
            TypeRegistration typeRegistration = new TypeRegistration();
            char ida = typeRegistration.AddTypeName(typea);
            char idb = typeRegistration.AddTypeName(typeb);
            char idc = typeRegistration.AddTypeName(typec);
            Assert.AreEqual(typea, typeRegistration.GetTypeName(ida));
            Assert.AreEqual(typeb, typeRegistration.GetTypeName(idb));
            Assert.AreEqual(typec, typeRegistration.GetTypeName(idc));
        }
    }
}
