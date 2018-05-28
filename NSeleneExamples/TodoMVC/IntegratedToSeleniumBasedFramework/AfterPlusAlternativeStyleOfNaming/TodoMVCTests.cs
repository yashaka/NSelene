using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static NSelene.Selene;


namespace NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.AfterPlusAlternativeStyleOfNaming
{
    [TestFixture]
    public class TestTodoMVC
    {
        static IWebDriver driver = new ChromeDriver();

        [TestFixtureTearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test]
        public void FilterTasks()
        {
            var tasks = new Pages.Tasks(driver);
            tasks.Visit();

            tasks.Add("a", "b", "c");
            tasks.ShouldBe("a", "b", "c");

            tasks.Toggle("b"); 

            tasks.FilterActive();
            tasks.ShouldBe("a", "c");

            tasks.FilterCompleted();
            tasks.ShouldBe("b");

            tasks.ClearCompleted();   /* << Added with */
            tasks.ShouldBeEmpty();    /* <<  NSelene   */
        }
    }
}
