using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using static NSelene.Selene;


namespace NSelene.Tests.Integration.SharedDriver.Harness
{

    [TestFixture]
    public class BaseTest
    {
        [OneTimeSetUp]
        public void initDriver()
        {
            string chromeVersion = "Latest"; // e.g. "83.0.4103.39" or "Latest", see https://chromedriver.chromium.org/downloads
            new DriverManager().SetUpDriver(new ChromeConfig(), version: chromeVersion);

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            SetWebDriver(new ChromeDriver(options));
        }

        [OneTimeTearDown]
        public void disposeDriver()
        {
            GetWebDriver().Quit();
        }
    }
}
