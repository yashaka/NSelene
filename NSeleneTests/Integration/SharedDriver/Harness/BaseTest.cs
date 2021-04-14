using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver.Harness
{

    [TestFixture]
    public class BaseTest
    {
        IWebDriver _driver; 

        [OneTimeSetUp]
        public void initDriver()
        {
            new DriverManager().SetUpDriver(
                // new ChromeConfig(), version: "Latest"
                new ChromeConfig(), version: "89.0.4389.23"
            );

            var options = new ChromeOptions();
            options.AddArguments("headless");
            this._driver = new ChromeDriver(options);

            Configuration.Driver = this._driver;
            Configuration.Timeout = 4.0;
            Configuration.PollDuringWaits = 0.1;
            Configuration.SetValueByJs = false;
            Configuration.TypeByJs = false;
            Configuration.ClickByJs = false;
        }

        [OneTimeTearDown]
        public void disposeDriver()
        {
            this._driver?.Quit();
            this._driver?.Dispose();
        }
    }
}
