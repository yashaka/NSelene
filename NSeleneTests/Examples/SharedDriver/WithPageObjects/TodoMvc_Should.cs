namespace NSelene.Tests.Examples.SharedDriver.WithPageObjects
{
    [TestFixture]
    public class BrowserTest
    {
        [OneTimeSetUp]
        public void InitDriver()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless"); 
            Configuration.Driver = new ChromeDriver(options);
        }

        [OneTimeTearDown]
        public void DisposeDriver()
        {
            Configuration.Driver.Quit();
            Configuration.Driver.Dispose();
        }
    }

    // todo: implement

    // class TodoMvc_Should : BrowserTest
    // {
    //     [Test]
    //     public void Complete_Todo()
    //     {
    //         Open("http://todomvc.com/examples/emberjs/");
    //         S("#new-todo").SetValue("a").PressEnter();
    //         S("#new-todo").SetValue("b").PressEnter();
    //         S("#new-todo").SetValue("c").PressEnter();

    //         SS("#todo-list>li").FindBy(Have.ExactText("b")).Find(".toggle").Click();

    //         SS("#todo-list>li").FilterBy(Have.CssClass("completed")).Should(Have.ExactTexts("b"));
    //         SS("#todo-list>li").FilterBy(Have.No.CssClass("completed")).Should(Have.ExactTexts("a", "c"));
    //     }
    // }
}