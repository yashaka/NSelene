using NUnit.Framework;
using System;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using static NSelene.Utils;


namespace NSeleneExamples
{
    [TestFixture]
    public abstract class BaseTest
    {
        [SetUp]
        public void SetupTest()
        {
            SetDriver(new FirefoxDriver());
        }

        [TearDown]
        public void TeardownTest()
        {
            GetDriver().Quit();
        }
    }
}
