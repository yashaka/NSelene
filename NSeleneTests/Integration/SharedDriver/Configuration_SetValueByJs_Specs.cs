namespace NSelene.Tests.Integration.SharedDriver.ConfigurationSpec
{
    [TestFixture]
    public class Configuration_SetValueByJs_Specs : BaseTest
    {

        [Test]
        public void SetValue_ViaElementCustomizedToJs_SetsItFasterThanNormalSetValue()
        {
            Configuration.SetValueByJs = false;
            Given.OpenedPageWithBody(@"
                <input id='field1' value='should be cleared'>
                <input id='field2' value='should be cleared'>
            ");
            var beforeType = DateTime.Now;
            S("#field1").SetValue(new string('*', 200));
            var afterType = DateTime.Now;
            var setValueTime = afterType - beforeType;

            var beforeJsSetValue = afterType;
            S("#field2").With(setValueByJs: true).SetValue(new string('*', 200));
            var afterJsSetValue = DateTime.Now;
            
            // THEN
            S("#field1").Should(Have.Value(new string('*', 200)));
            S("#field2").Should(Have.Value(new string('*', 200)));
            
            var jsTime = afterJsSetValue - beforeJsSetValue;
            Assert.That(jsTime, Is.LessThan(setValueTime / 2));
        }

        [Test]
        public void SetValue_ViaConfiguredGloballyToJs_SetsItFasterThanNormalSetValue()
        {
            Configuration.SetValueByJs = false;
            Given.OpenedPageWithBody(@"
                <input id='field1' value='should be cleared'>
                <input id='field2' value='should be cleared'>
            ");
            var beforeType = DateTime.Now;
            S("#field1").SetValue(new string('*', 200));
            var afterType = DateTime.Now;
            var setValueTime = afterType - beforeType;

            var beforeJsSetValue = afterType;
            Configuration.SetValueByJs = true;
            S("#field2").SetValue(new string('*', 200));
            var afterJsSetValue = DateTime.Now;
            
            // THEN
            S("#field1").Should(Have.Value(new string('*', 200)));
            S("#field2").Should(Have.Value(new string('*', 200)));
            
            var jsTime = afterJsSetValue - beforeJsSetValue;
            Assert.That(jsTime, Is.LessThan(setValueTime / 2));
        }
    }
}

