using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{

    [TestFixture]
    public class Configuration_Specs
    {
        IWebDriver _driver1 { get; set;}
        IWebDriver _driver2 { get; set;}

        [OneTimeSetUp]
        public void InitConfiguration()
        {            
            new DriverManager().SetUpDriver(
                // new ChromeConfig(), version: "Latest"
                new ChromeConfig(), version: "89.0.4389.23"
            );

            var options = new ChromeOptions();
            options.AddArguments("headless");
            this._driver1 = new ChromeDriver(options);
            this._driver2 = new ChromeDriver(options);

            Configuration.Driver = this._driver1;
            Configuration.Timeout = 4;
            Configuration.PollDuringWaits = 0.1;
            Configuration.SetValueByJs = false;
        }

        [OneTimeTearDown]
        public void ResetConfiguration()
        {
            this._driver1.Quit();
            this._driver2.Quit();

            Configuration.Driver = null;
            Configuration.Timeout = 4;
            Configuration.PollDuringWaits = 0.1;
            Configuration.SetValueByJs = false;
        }
        
        [Test]
        public void ConfigurationAlreadyHasDefaults()
        {
            // TODO: check Configuration defaults somehow... maybe in a new thread/sub-process?
            // is it's even possible?:) 
        }
        
        [Test]
        public void New_CreatesNewConfigWithAllSettingsDefaults()
        {
            // GIVEN some shared static Configuration
            Configuration.Driver = _driver1;
            Configuration.Timeout = 9.9;
            Configuration.PollDuringWaits = 0.9;
            Configuration.SetValueByJs = true;

            // WHEN
            var fresh = Configuration._New_();

            // THEN
            Assert.AreEqual(null, fresh.Driver);
            Assert.AreEqual(4.0, fresh.Timeout);
            Assert.AreEqual(0.1, fresh.PollDuringWaits);
            Assert.AreEqual(false, fresh.SetValueByJs);
        }
        
        [Test]
        public void New_DoesNotTouchExistingSharedStaticConfiguration()
        {
            // GIVEN some shared static Configuration
            Configuration.Driver = _driver1;
            Configuration.Timeout = 9.9;
            Configuration.PollDuringWaits = 0.9;
            Configuration.SetValueByJs = true;

            // WHEN
            var fresh = Configuration._New_();

            // THEN
            Assert.AreEqual(_driver1, Configuration.Driver);
            Assert.AreEqual(9.9, Configuration.Timeout);
            Assert.AreEqual(0.9, Configuration.PollDuringWaits);
            Assert.AreEqual(true, Configuration.SetValueByJs);
        }
        
        [Test]
        public void New_CanCustomizeManySettings()
        {
            var custom = Configuration._New_(
                driver: this._driver1,
                timeout: 2.0,
                pollDuringWaits: 0.2,
                setValueByJs: true
            );
            
            Assert.AreEqual(this._driver1, custom.Driver);
            Assert.AreEqual(2.0, custom.Timeout);
            Assert.AreEqual(0.2, custom.PollDuringWaits);
            Assert.AreEqual(true, custom.SetValueByJs);
        }
        
        [Test]
        public void New_CanCustomizeSetting()
        {
            Configuration.Timeout= 1.0;

            var custom = Configuration._New_(timeout: 2.0);

            Assert.AreEqual(2.0, custom.Timeout);
        }
        
        [Test]
        public void With_CustomizesSetting()
        {
            Configuration.Timeout= 1.0;

            var custom = Configuration._With_(timeout: 2.0);

            Assert.AreEqual(2.0, custom.Timeout);
        }
        
        [Test]
        public void With_ReusesLatestSharedSettingsNotExplicitelyOverriden()
        {
            Configuration.Timeout = 2.0;
            Configuration.PollDuringWaits = 0.2;

            var custom = Configuration._With_(timeout: 1.0);

            Assert.AreEqual(1.0, custom.Timeout);
            Assert.AreEqual(0.2, custom.PollDuringWaits);
        }
        
        [Test]
        public void WithConfig_TracksChangesOnlyInReusedSharedConfigSettings()
        {
            Configuration.Driver = this._driver1;
            Configuration.Timeout = 2.0;
            Configuration.PollDuringWaits = 0.20;
            var custom = Configuration._With_(timeout: 1.0);

            Configuration.Driver = this._driver2;
            Configuration.Timeout = 2.0;
            Configuration.PollDuringWaits = 0.25;

            Assert.AreEqual(1.0, custom.Timeout);
            Assert.AreSame(this._driver2, custom.Driver);
            Assert.AreEqual(0.25, custom.PollDuringWaits);
        }
        
        [Test]
        public void NewConfig_StaysUnchangedOnFurtherSharedConfigUpdates()
        {
            var custom = Configuration._New_(timeout: 1.0);

            Configuration.Timeout = 2.0;
            Configuration.PollDuringWaits = 0.2;

            Assert.AreEqual(1.0, custom.Timeout);
            Assert.AreEqual(0.1, custom.PollDuringWaits);
        }
        
        [Test]
        public void Several_NewConfigs_Are_Different()
        { // TODO: consider broadening coverage
            Configuration.Timeout = 0.5;
            var first = Configuration._New_(timeout: 1.0);

            var second = Configuration._New_(timeout: 2.0);

            Assert.AreEqual(1.0, first.Timeout);
            Assert.AreEqual(2.0, second.Timeout);
        }

        // TODO: ensure that switching Configuration.driver will work on S, etc.
    }
}

