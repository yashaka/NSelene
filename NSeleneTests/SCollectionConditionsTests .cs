using NUnit.Framework;
using NSelene;
using static NSelene.Utils;

namespace NSeleneTests.WithManagedBrowserBeforAndAfterAllTests
{
    [TestFixture]
    public class SCollectionConditionTests
    {

        [Test]
        public void SElementShouldHaveText()
        {
            Given.OpenedPageWithBody("<ul>Hello to:<li>Dear Bob</li><li>Lovly Kate</li></ul>");
            SS("li").Should(Have.Texts("Bob", "Kate"));
        }
    }
}

