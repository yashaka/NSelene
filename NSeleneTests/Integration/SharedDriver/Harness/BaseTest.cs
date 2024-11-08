using System.Reflection;

namespace NSelene.Tests.Integration.SharedDriver.Harness
{

    [TestFixture]
    public class BaseTest
    {
        public const double ShortTimeout = 0.25;
        public const double MediumTimeout = 0.6;
        public const double LongTimeout = 4.0;
        public const double PollDuringWaits = 0.1;
        public static readonly TimeSpan ShortTimeoutSpan = TimeSpan.FromSeconds(ShortTimeout);

        internal static readonly string EmptyHtmlPath = new Uri(
                    new Uri(Assembly.GetExecutingAssembly().Location),
                    "../../../Resources/empty.html"
                ).AbsolutePath;
        internal static readonly string EmptyHtmlUri = new Uri(
                    new Uri(Assembly.GetExecutingAssembly().Location),
                    "../../../Resources/empty.html"
                ).AbsoluteUri;

        IWebDriver _driver;

        [OneTimeSetUp]
        public void InitDriver()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");
            this._driver = new ChromeDriver(options);
        }
        [SetUp]
        public void InitDefaults()
        { 
            // explicit resetting defaults
            Configuration.Driver = this._driver;
            Configuration.Timeout = LongTimeout;
            Configuration.PollDuringWaits = PollDuringWaits;
            Configuration.SetValueByJs = false;
            Configuration.TypeByJs = false;
            Configuration.ClickByJs = false;
            Configuration.WaitForNoOverlapFoundByJs = false;
            // TODO: ensure we have tests where it is set to false
            Configuration.LogOuterHtmlOnFailure = true;
            Configuration._HookWaitAction = null;
        }

        [OneTimeTearDown]
        public void DisposeDriver()
        {
            this._driver?.Quit();
            this._driver?.Dispose();
        }
    }
}
