using System;
using System.Drawing;
using NSelene;
using NSelene.Tests.Integration.SharedDriver.Harness;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace NSeleneTests.Integration.SharedDriver
{
    [TestFixture]
    public class ConfigurationWebDriverWindowSpecs
    {
        private IWebDriver _driver { get; set; }

        [SetUp]
        public void InitConfiguration()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");
            _driver = new ChromeDriver(options);
            Configuration.Driver = this._driver;
        }


        [TearDown]
        public void ResetConfiguration()
        {
            _driver.Quit();
            Configuration.Driver = null;
        }


        [Test]
        public void WindowShouldHaveConfiguredSize()
        {
            //arrange
            var expectedSize = new Size(888, 666);
            Configuration.WindowWidth = 888;
            Configuration.WindowHeight = 666;

            //act
            Given.OpenedEmptyPage();

            //assert
            Assert.AreEqual(expectedSize, Configuration.Driver.Manage().Window.Size);
        }

        [Test]
        public void WindowShouldHaveDefaultSizeWhenNotConfigured()
        {
            //arrange
            var expectedSize = new Size(800, 600);
            Configuration.WindowWidth = null;
            Configuration.WindowHeight = null;

            //act
            Given.OpenedEmptyPage();

            //assert
            Assert.AreEqual(expectedSize, Configuration.Driver.Manage().Window.Size);
        }


        [Test]
        public void WindowShouldHaveConfiguredHeightAndDefaultWidthWhenConfiguration_WidthIsNull()
        {
            //arrange
            var expectedSize = new Size(800, 666);
            Configuration.WindowWidth = null;
            Configuration.WindowHeight = 666;

            //act
            Given.OpenedEmptyPage();

            //assert
            Assert.AreEqual(expectedSize, Configuration.Driver.Manage().Window.Size);
        }

        [Test]
        public void WindowShouldHaveConfiguredWidthAndDefaultHeightWhenConfiguration_HeightIsNull()
        {
            //arrange
            var expectedSize = new Size(888, 600);
            Configuration.WindowWidth = 888;
            Configuration.WindowHeight = null;

            //act
            Given.OpenedEmptyPage();

            //assert
            Assert.AreEqual(expectedSize, Configuration.Driver.Manage().Window.Size);
        }

        [TestCase(888, -666)]
        [TestCase(-888, 666)]
        public void WebDriverArgumentExceptionShouldBeThrownWhenConfiguredWithNegatives(int width, int height)
        {
            //arrange
            Configuration.WindowWidth = width;
            Configuration.WindowHeight = height;

            try
            {
                //act
                Given.OpenedEmptyPage();
            }
            catch (Exception e)
            {
                //assert
                Assert.IsInstanceOf<WebDriverArgumentException>(e);
            }
        }
    }
}