using System;
using NUnit.Framework;
using static NSelene.Utils;
using NSelene;

namespace NSeleneTests.WithManagedBrowserBeforAndAfterAllTests
{
    [TestFixture]
    public class SCollectionActionsTests
    {

        [Test]
        public void SCollectionGetCountCountsInvisible()
        {
            Given.OpenedEmptyPage();
            var elements = SS(".will-appear");
            When.WithBody(@"
                <p>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear' style='display:none'>Kate</li>
                    </ul>
                </p>"
            );
            Assert.AreEqual(2, elements.Count);
        }

        // TODO: TBD
    }
}

