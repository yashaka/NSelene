using NUnit.Framework;
using OpenQA.Selenium.Firefox;
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
    public class BaseTest
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
