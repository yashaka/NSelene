using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
            var options = new ChromeOptions();
            options.AddArguments("headless");
            this._driver = new ChromeDriver(options);

            // explicit resetting defaults
            Configuration.Driver = this._driver;
            Configuration.Timeout = 4.0;
            Configuration.PollDuringWaits = 0.1;
            Configuration.SetValueByJs = false;
            Configuration.TypeByJs = false;
            Configuration.ClickByJs = false;
            Configuration.WaitForNoOverlapFoundByJs = false;
            // TODO: ensure we have tests where it is set to false
            Configuration.LogOuterHtmlOnFailure = true;
            Configuration._HookWaitAction = null;
        }

        [OneTimeTearDown]
        public void disposeDriver()
        {
            this._driver?.Quit();
            this._driver?.Dispose();
        }
    }
}
