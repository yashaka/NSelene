using NUnit.Framework;
using System;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using static NSelene.Selene;


namespace NSeleneExamples
{
    [TestFixture()]
    public class BaseTest
    {
        [SetUp]
        public void SetupTest()
        {
            SetWebDriver(new FirefoxDriver());
        }

        [TearDown]
        public void TeardownTest()
        {
            GetWebDriver().Quit();
        }
    }
}
