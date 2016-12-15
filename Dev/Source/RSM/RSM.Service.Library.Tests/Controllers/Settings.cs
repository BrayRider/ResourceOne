using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSM.Artifacts;
using RSM.Staging.Library;
using RSM.Support;

namespace RSM.Service.Library.Tests.Controllers
{
    [TestClass]
    public class Settings
    {
        public TestContext TestContext { get; set; }

        public const int BaseId = 10000;

        [TestMethod]
        public void Search()
        {
            const string settingName = "Test Setting";

            var system = Factory.CreateExternalSystem(BaseId + 1, "Test System", ExternalSystemDirection.None);
            var system2 = Factory.CreateExternalSystem(BaseId + 2, "Test System 2", ExternalSystemDirection.None);

            var testSetting = Factory.CreateSetting(BaseId + 1, settingName, "Test Setting Label", "value", 0, true, InputTypes.Text, system);
            var testSetting2 = Factory.CreateSetting(BaseId + 2, settingName + " 2", "Test Setting Label 2", "value", 0, true, InputTypes.Text, system2);
            var testSetting3 = Factory.CreateSetting(BaseId + 3, settingName + " 2", "Test Setting Label 2", "value", 0, true, InputTypes.Text, system);

            using (var context = new RSMDataModelDataContext())
            {
                context.ExternalSystems.InsertOnSubmit(system);
                context.ExternalSystems.InsertOnSubmit(system2);

                context.Settings.InsertOnSubmit(testSetting);
                context.Settings.InsertOnSubmit(testSetting2);
                context.Settings.InsertOnSubmit(testSetting3);

                context.SubmitChanges();
            }

            var controller = new Library.Controllers.Settings();

            var results = controller.Search(o => o.SystemId == system.Id);

            Assert.IsNotNull(results, "Missing results");
            Assert.IsTrue(results.Succeeded, "Call Failed");
            Assert.IsNotNull(results.Entity, "Missing entity");
            Assert.AreEqual(results.Entity.Count, 2, "Incorrect row count returned");
        }

        [TestMethod]
        public void GetByName()
        {
            const string settingName = "Test Setting";
            var system = Factory.CreateExternalSystem(BaseId + 1, "Test System", ExternalSystemDirection.None);
            var testSetting = Factory.CreateSetting(BaseId + 1, settingName, "Test Setting Label", "value", 0, true, InputTypes.Text, system);

            using (var context = new RSMDataModelDataContext())
            {
                context.ExternalSystems.InsertOnSubmit(system);
                context.Settings.InsertOnSubmit(testSetting);
                context.SubmitChanges();
            }

            var controller = new Library.Controllers.Settings();

            var results = controller.Get(system.Name, settingName);

            Assert.IsNotNull(results, "Missing results");
            Assert.IsTrue(results.Succeeded, "Call Failed");
            Assert.IsNotNull(results.Entity, "Missing entity");
            Assert.IsTrue(testSetting.Equals(results.Entity), "Values don't match");
        }

        [TestMethod]
        public void Set()
        {
            const string settingName = "Test Setting";
            var system = Factory.CreateExternalSystem(BaseId + 1, "Test System", ExternalSystemDirection.None);
            var testSetting = Factory.CreateSetting(BaseId + 1, settingName, "Test Setting Label", "value", 0, true, InputTypes.Text, system);

            const string newValue = "new value";

            using (var context = new RSMDataModelDataContext())
            {
                context.ExternalSystems.InsertOnSubmit(system);
                context.Settings.InsertOnSubmit(testSetting);
                context.SubmitChanges();
            }

            var controller = new Library.Controllers.Settings();

            var results = controller.Set(testSetting.Id, newValue);

            Assert.IsNotNull(results, "Missing results");
            Assert.IsTrue(results.Succeeded, "Call Failed");
            Assert.IsNotNull(results.Entity, "Missing entity");
            Assert.IsTrue(newValue.Equals(results.Entity.Value), string.Format("New value isn't correct - {0} | {1}", newValue, results.Entity.Value));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            using (var context = new RSMDataModelDataContext())
            {
                context.Settings.DeleteAllOnSubmit(context.Settings.Where(x => x.Id >= BaseId));
                context.ExternalSystems.DeleteAllOnSubmit(context.ExternalSystems.Where(x => x.Id >= BaseId));
                context.SubmitChanges();
            }
        }
    }
}
