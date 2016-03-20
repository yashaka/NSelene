using NUnit.Framework;
using NSelene;
using static NSelene.Utils;

namespace NSeleneTests.WithManagedBrowserBeforAndAfterAllTests
{
    [TestFixture]
    public class SCollectionConditionTests
    {

        [Test]
        public void SCollectionShouldHaveTexts()
        { 
            Given.OpenedPageWithBody("<ul>Hello to:<li>Dear Bob</li><li>Lovly Kate</li></ul>");
            SS("li").ShouldNot(Have.Texts("Kate", "Bob"));
            SS("li").ShouldNot(Have.Texts("Bob"));
            SS("li").ShouldNot(Have.Texts("Bob", "Kate", "Joe"));
            SS("li").Should(Have.Texts("Bob", "Kate"));
        }

        [Test]
        public void SCollectionShouldHaveCountAtLeastAndCount()
        {
            Given.OpenedEmptyPage();
            SS("li").ShouldNot(Have.Count(2));
            When.WithBody("<ul>Hello to:<li>Dear Bob</li><li>Lovly Kate</li></ul>");
            SS("li").ShouldNot(Have.CountAtLeast(3));
            SS("li").Should(Have.Count(2));
            SS("li").Should(Have.CountAtLeast(1));
        }
    }
}

