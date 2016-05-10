using NUnit.Framework;
using static NSelene.Utils;
using NSeleneExamples.TodoMVC.Pages;


namespace NSeleneExamples
{
    namespace TodoMVC
    {
        [TestFixture]
        [Parallelizable(ParallelScope.Fixtures)]
        public class Test3 : BaseTest
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

}
