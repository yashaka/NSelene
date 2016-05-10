using NUnit.Framework;
using static NSelene.Utils;
using NSeleneExamples.TodoMVC.Pages;


namespace NSeleneExamples
{
    namespace TodoMVC
    {
        [TestFixture]
        [Parallelizable(ParallelScope.Self)]
        public class Test4 : BaseTest
        {
            [Test]
            [Parallelizable(ParallelScope.Self)]
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

            [Test]
            [Parallelizable(ParallelScope.Self)]
            public void FilterTasks2()
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

            [Test]
            [Parallelizable(ParallelScope.Self)]
            public void FilterTasks3()
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
