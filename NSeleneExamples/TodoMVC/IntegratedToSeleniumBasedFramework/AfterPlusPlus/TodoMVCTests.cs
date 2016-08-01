using NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.AfterPlusPlus.Pages;
using NUnit.Framework;
using static NSelene.Selene;


namespace NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.AfterPlusPLus
{
    [TestFixture]
    public class TestTodoMVC : BaseTest
    {
        [Test]
        public void FilterTasks()
        {
            var tasks = new Tasks();
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
