namespace NSelene.Tests.Examples.SharedDriver.StraightForward
{
    [TestFixture]
    public class BrowserTest
    {
        IWebDriver driver;

        [OneTimeSetUp]
        public void InitDriver()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");
            this.driver = new ChromeDriver(options);
            Configuration.Driver = driver;
            Configuration.BaseUrl = "http://todomvc-emberjs-app.autotest.how/";
        }

        [OneTimeTearDown]
        public void DisposeDriver()
        {
            Configuration.BaseUrl = "";
            this.driver.Quit();
            this.driver.Dispose();
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