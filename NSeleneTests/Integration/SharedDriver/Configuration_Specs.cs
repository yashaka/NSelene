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
            var options = new ChromeOptions();
            options.AddArguments("headless");
            this._driver1 = new ChromeDriver(options);
            this._driver2 = new ChromeDriver(options);

            Configuration.Driver = this._driver1;
            Configuration.Timeout = 4;
            Configuration.PollDuringWaits = 0.1;
            Configuration.SetValueByJs = false;
            Configuration.BaseUrl = "";
        }

        [OneTimeTearDown]
        public void ResetConfiguration()
        {
            this._driver1.Quit();
            this._driver1.Dispose();
            this._driver2.Quit();
            this._driver2.Dispose();

            Configuration.Driver = null;
            Configuration.Timeout = 4;
            Configuration.PollDuringWaits = 0.1;
            Configuration.SetValueByJs = false;
            Configuration.BaseUrl = "";
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
            Configuration.BaseUrl = "test url";

            // WHEN
            var fresh = Configuration._New_();

            // THEN
            Assert.That(fresh.Driver, Is.EqualTo(null));
            Assert.That(fresh.Timeout, Is.EqualTo(4.0));
            Assert.That(fresh.PollDuringWaits, Is.EqualTo(0.1));
            Assert.That(fresh.SetValueByJs, Is.EqualTo(false));
            Assert.That(fresh.BaseUrl, Is.EqualTo(""));
        }
        
        [Test]
        public void New_DoesNotTouchExistingSharedStaticConfiguration()
        {
            // GIVEN some shared static Configuration
            Configuration.Driver = _driver1;
            Configuration.Timeout = 9.9;
            Configuration.PollDuringWaits = 0.9;
            Configuration.SetValueByJs = true;
            Configuration.BaseUrl = "test url";

            // WHEN
            var fresh = Configuration._New_();

            // THEN
            Assert.That(Configuration.Driver, Is.EqualTo(_driver1));
            Assert.That(Configuration.Timeout, Is.EqualTo(9.9));
            Assert.That(Configuration.PollDuringWaits, Is.EqualTo(0.9));
            Assert.That(Configuration.SetValueByJs, Is.EqualTo(true));
            Assert.That(Configuration.BaseUrl, Is.EqualTo("test url"));
        }
        
        [Test]
        public void New_CanCustomizeManySettings()
        {
            var custom = Configuration._New_(
                driver: this._driver1,
                timeout: 2.0,
                pollDuringWaits: 0.2,
                setValueByJs: true,
                baseUrl: "test url"
            );

            Assert.That(custom.Driver, Is.EqualTo(this._driver1));
            Assert.That(custom.Timeout, Is.EqualTo(2.0));
            Assert.That(custom.PollDuringWaits, Is.EqualTo(0.2));
            Assert.That(custom.SetValueByJs, Is.EqualTo(true));
            Assert.That(custom.BaseUrl, Is.EqualTo("test url"));
        }

        [Test]
        public void New_CanCustomizeSetting()
        {
            Configuration.Timeout = 1.0;
            Configuration.BaseUrl = "test url";

            var custom = Configuration._New_(timeout: 2.0, baseUrl: "test2 url");

            Assert.That(custom.Timeout, Is.EqualTo(2.0));
            Assert.That(custom.BaseUrl, Is.EqualTo("test2 url"));
        }

        [Test]
        public void With_CustomizesSetting()
        {
            Configuration.Timeout = 1.0;
            Configuration.BaseUrl = "test url";

            var custom = Configuration._With_(timeout: 2.0, baseUrl: "test2 url");

            Assert.That(custom.Timeout, Is.EqualTo(2.0));
            Assert.That(custom.BaseUrl, Is.EqualTo("test2 url"));
        }
        
        [Test]
        public void With_ReusesLatestSharedSettingsNotExplicitelyOverriden()
        {
            Configuration.Timeout = 2.0;
            Configuration.PollDuringWaits = 0.2;

            var custom = Configuration._With_(timeout: 1.0);

            Assert.That(custom.Timeout, Is.EqualTo(1.0));
            Assert.That(custom.PollDuringWaits, Is.EqualTo(0.2));
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

            Assert.That(custom.Timeout, Is.EqualTo(1.0));
            Assert.That(custom.Driver, Is.SameAs(this._driver2));
            Assert.That(custom.PollDuringWaits, Is.EqualTo(0.25));
        }
        
        [Test]
        public void NewConfig_StaysUnchangedOnFurtherSharedConfigUpdates()
        {
            var custom = Configuration._New_(timeout: 1.0, baseUrl: "test url");

            Configuration.Timeout = 2.0;
            Configuration.PollDuringWaits = 0.2;
            Configuration.BaseUrl = "test2 url";

            Assert.That(custom.Timeout, Is.EqualTo(1.0));
            Assert.That(custom.PollDuringWaits, Is.EqualTo(0.1));
            Assert.That(custom.BaseUrl, Is.EqualTo("test url"));
        }
        
        [Test]
        public void Several_NewConfigs_Are_Different()
        { // TODO: consider broadening coverage
            Configuration.Timeout = 0.5;
            var first = Configuration._New_(timeout: 1.0, baseUrl: "test url");

            var second = Configuration._New_(timeout: 2.0, baseUrl: "test2 url");

            Assert.That(first.Timeout, Is.EqualTo(1.0));
            Assert.That(second.Timeout, Is.EqualTo(2.0));

            Assert.That(first.BaseUrl, Is.EqualTo("test url"));
            Assert.That(second.BaseUrl, Is.EqualTo("test2 url"));
        }

        // TODO: ensure that switching Configuration.driver will work on S, etc.
    }
}