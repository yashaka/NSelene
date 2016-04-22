using NUnit.Framework;
using System;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using static NSelene.Utils;


namespace NSeleneTests
{
    namespace WithManagedBrowserBeforAndAfterAllTests {
        [SetUpFixture]
        public class BrowserSetup
        {
            public BrowserSetup()
            {
            }

            [SetUp]
            public void SetUp()
            {
                SetDriver(new FirefoxDriver());
            }

            [TearDown]
            public void TearDown()
            {
                GetDriver().Quit();
            }
        }
    }

    [TestFixture()]
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
