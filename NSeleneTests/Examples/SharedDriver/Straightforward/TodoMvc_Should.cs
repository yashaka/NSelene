using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using static NSelene.Selene;

namespace NSelene.Tests.Examples.SharedDriver.StraightForward
{
    [TestFixture]
    public class BrowserTest
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

    class TodoMvc_Should : BrowserTest
    {
        [Test]
        public void Complete_Todo()
        {
            Open("http://todomvc.com/examples/emberjs/");
            S("#new-todo").SetValue("a").PressEnter();
            S("#new-todo").SetValue("b").PressEnter();
            S("#new-todo").SetValue("c").PressEnter();

            SS("#todo-list>li").FindBy(Have.ExactText("b")).Find(".toggle").Click();

            SS("#todo-list>li").FilterBy(Have.CssClass("completed")).Should(Have.ExactTexts("b"));
            SS("#todo-list>li").FilterBy(Have.No.CssClass("completed")).Should(Have.ExactTexts("a", "c"));
        }
    }
}