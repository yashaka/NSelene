using NUnit.Framework;
using NSelene;
using static NSelene.Utils;

namespace NSeleneTests.WithManagedBrowserBeforAndAfterAllTests
{
    [TestFixture]
    public class SCollectionConditionTests
    {

        [Test]
        public void SCollectionShouldHaveTextsAndExactTexts()
        { 
            Given.OpenedPageWithBody("<ul>Hello to:<li>Dear Bob</li><li>Lovely Kate</li></ul>");
            SS("li").ShouldNot(Have.Texts("Kate", "Bob"));
            SS("li").ShouldNot(Have.Texts("Bob"));
            SS("li").ShouldNot(Have.Texts("Bob", "Kate", "Joe"));
            SS("li").Should(Have.Texts("Bob", "Kate"));
            SS("li").ShouldNot(Have.ExactTexts("Bob", "Kate"));
            SS("li").ShouldNot(Have.ExactTexts("Lovely Kate", "Dear Bob"));
            SS("li").ShouldNot(Have.ExactTexts("Dear Bob"));
            SS("li").ShouldNot(Have.ExactTexts("Dear Bob", "Lovely Kate", "Funny Joe"));
            SS("li").Should(Have.ExactTexts("Dear Bob", "Lovely Kate"));
        }

        [Test]
        public void SCollectionShouldHaveCountAtLeastAndCount()
        {
            Given.OpenedEmptyPage();
            SS("li").ShouldNot(Have.Count(2));
            When.WithBody("<ul>Hello to:<li>Dear Bob</li><li>Lovely Kate</li></ul>");
            SS("li").ShouldNot(Have.CountAtLeast(3));
            SS("li").Should(Have.Count(2));
            SS("li").Should(Have.CountAtLeast(1));
        }

        [Test]
        public void SCollectionShouldBeEmpty()
        {
            Given.OpenedEmptyPage();
            SS("li").Should(Be.Empty());
            When.WithBody("<ul>Hello to:<li>Dear Bob</li><li>Lovely Kate</li></ul>");
            SS("li").ShouldNot(Be.Empty());
        }
    }
}

