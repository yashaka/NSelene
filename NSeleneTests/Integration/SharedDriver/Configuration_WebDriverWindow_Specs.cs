using System;
using System.Drawing;
using System.Reflection;
using NSelene;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace NSeleneTests.Integration.SharedDriver
{
    [TestFixture]
    public class ConfigurationWebDriverWindowSpecs
    {
        private IWebDriver _driver { get; set; }

        private readonly string _emptyPage = new Uri(new Uri(Assembly.GetExecutingAssembly().Location),
            "../../../Resources/empty.html").AbsoluteUri;

        private const int DefaultSeleniumWidth = 800;
        private const int DefaultSeleniumHeight = 600;

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");
            _driver = new ChromeDriver(options);
            Configuration.Driver = _driver;
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
            Configuration.Driver = null;
        }


        [Test]
        public void WindowShouldHaveConfiguredSize()
        {
            ResetSeleneConfiguration();

            Configuration.WindowWidth = 888;
            Configuration.WindowHeight = 666;
            Selene.Open(_emptyPage);

            Assert.AreEqual(new Size(888, 666), Configuration.Driver.Manage().Window.Size);
        }

        [Test]
        public void WindowShouldHaveDefaultSizeIfNotConfigured()
        {
            ResetSeleneConfiguration();

            Selene.Open(_emptyPage);

            Assert.AreEqual(new Size(DefaultSeleniumWidth, DefaultSeleniumHeight),
                Configuration.Driver.Manage().Window.Size);
        }


        [Test]
        public void WindowShouldHaveConfiguredHeightAndDefaultWidth_OnNullInConfigurationWidth()
        {
            ResetSeleneConfiguration();

            Configuration.WindowHeight = 666;
            Selene.Open(_emptyPage);

            Assert.AreEqual(new Size(DefaultSeleniumWidth, 666), Configuration.Driver.Manage().Window.Size);
        }

        [Test]
        public void WindowShouldHaveConfiguredWidthAndDefaultHeight_OnNullInConfigurationHeight()
        {
            ResetSeleneConfiguration();

            Configuration.WindowWidth = 888;
            Selene.Open(_emptyPage);

            Assert.AreEqual(new Size(888, DefaultSeleniumHeight), Configuration.Driver.Manage().Window.Size);
        }

        [TestCase(-888, 666)]
        [TestCase(888, -666)]
        public void WebDriverArgumentExceptionShouldBeThrown_OnNegativeNumberInConfiguration(int width, int height)
        {
            ResetSeleneConfiguration();
            Configuration.WindowWidth = width;
            Configuration.WindowHeight = height;

            try
            {
                Selene.Open(_emptyPage);
                throw new Exception("Should fail when window size configs have negative values.");
            }

            catch (Exception e)
            {
                Assert.IsInstanceOf<WebDriverArgumentException>(e);
            }
        }

        [Test]
        public void WindowSizeShouldNotBeChangedUntilOpenNewUrl()
        {
            ResetSeleneConfiguration();
            Configuration.WindowWidth = 888;
            Configuration.WindowHeight = 666;
            Selene.Open(_emptyPage);

            Configuration.WindowWidth = DefaultSeleniumWidth;
            Configuration.WindowHeight = DefaultSeleniumHeight;

            Assert.AreEqual(new Size(888, 666), Configuration.Driver.Manage().Window.Size);
        }


        private static void ResetSeleneConfiguration()
        {
            Configuration.WindowWidth = null;
            Configuration.WindowHeight = null;
        }
    }
}