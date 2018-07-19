using NUnit.Framework;
using static NSelene.Selene;
using NSeleneExamples.TodoMVC.WithPages.Pages;


namespace NSeleneExamples.TodoMVC.WithPages
{
    [TestFixture]
    public class TodoMvcTests : BaseTest
    {
        [Test]
        public void FilterTasks()
        {
            Tasks.Visit();

            Tasks.Add("a", "b", "c");
            Tasks.ShouldBe("a", "b", "c");

            Tasks.Toggle("b"); 

            Tasks.FilterActive();
            Tasks.ShouldBe("a", "c");

            Tasks.FilterCompleted();
            Tasks.ShouldBe("b");
        }
    }
}
