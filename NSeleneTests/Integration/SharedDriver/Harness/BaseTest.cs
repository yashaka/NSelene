using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using static NSelene.Selene;


namespace NSelene.Tests.Integration.SharedDriver.Harness
{

    [TestFixture]
    public class BaseTest
    {
        [OneTimeSetUp]
        public void initDriver()
        {
            SetWebDriver(new ChromeDriver());
        }

        [OneTimeTearDown]
        public void disposeDriver()
        {
            GetWebDriver().Quit();
        }
    }
}
