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
            var options = new ChromeOptions();
            options.AddArguments("headless");
            Configuration.Driver = new ChromeDriver(options);
            Configuration.BaseUrl = "http://todomvc-emberjs-app.autotest.how/";
        }

        [OneTimeTearDown]
        public void disposeDriver()
        {
            Configuration.Driver.Quit();
            Configuration.BaseUrl = "";
        }
    }

    class TodoMvc_Should : BrowserTest
    {
        [Test]
        public void Complete_Todo()
        {
            Open("/");
            S("#new-todo").SetValue("a").PressEnter();
            S("#new-todo").SetValue("b").PressEnter();
            S("#new-todo").SetValue("c").PressEnter();

            SS("#todo-list>li").ElementBy(Have.ExactText("b")).Element(".toggle").Click();

            SS("#todo-list>li").By(Have.CssClass("completed")).Should(Have.ExactTexts("b"));
            SS("#todo-list>li").By(Have.No.CssClass("completed")).Should(Have.ExactTexts("a", "c"));
        }
    }
}