using System;
using NUnit.Framework;
using static NSelene.Selene;
using NSelene;

namespace NSeleneTests
{
    [TestFixture]
    public class SCollectionActionsTests : BaseTest
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

