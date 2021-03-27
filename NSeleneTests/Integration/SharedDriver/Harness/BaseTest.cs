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
            string chromeVersion = "Latest";
            new DriverManager().SetUpDriver(new ChromeConfig(), version: chromeVersion);

            var options = new ChromeOptions();
            options.AddArguments("headless");
            SetWebDriver(new ChromeDriver(options));
        }

        [OneTimeTearDown]
        public void disposeDriver()
        {
            GetWebDriver().Quit();
        }
    }
}
