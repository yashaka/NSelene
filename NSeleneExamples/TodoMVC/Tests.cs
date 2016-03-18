using NUnit.Framework;
using static NSelene.Utils;
using NSeleneTests.Pages;


namespace NSeleneTests
{
    namespace NSeleneTests
    {
        [TestFixture()]
        public class Test : BaseTest
        {
            [Test()]
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
