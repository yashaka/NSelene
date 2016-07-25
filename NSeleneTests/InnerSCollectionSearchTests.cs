using NUnit.Framework;
using NSelene;
using static NSelene.Utils;
using OpenQA.Selenium;
using System.Threading;

namespace NSeleneTests
{
    [TestFixture]
    public class InnerSCollectionSearchTests : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void InnerSCollectionSearchIsLazyAndDoesNotStartOnCreation()
        {
            Given.OpenedPageWithBody("<p id='#existing'>Hello!</p>");
            var nonExistingCollection = S("#existing").SS(".also-not-existing"); 
            var nonExistingCollection2 = S("#not-existing").SS(".also-not-existing"); 
        }

        [Test]
        public void InnerSCollectionSearchIsPostponedUntilActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = S("div").SS("li");
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear'>Kate</li>
                    </ul>
                </div>"
            ); //TODO: consider simplifying example via removing div and using ul instead
            Assert.AreEqual(2, elements.Count);
        }

        [Test]
        public void InnerSCollectionSearchIsUpdatedOnNextActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = S("div").SS(".will-appear");
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear'>Kate</li>
                    </ul>
                </div>"
            );
            Assert.AreEqual(2, elements.Count);
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear'>Kate</li>
                        <li class='will-appear'>Joe</li>
                    </ul>
                </div>"
            );
            Assert.AreEqual(3, elements.Count);
        }

        [Test]
        public void InnerSCollectionSearchWaitsNothing()
        {
            Given.OpenedEmptyPage();
            var elements = S("div").SS(".will-appear");
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear' style='display:none'>Kate</li>
                    </ul>
                </div>"
            );
            When.WithBodyTimedOut(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear' style='display:none'>Kate</li>
                        <li class='will-appear'>Bobik</li>
                    </ul>
                </div>",
                500
            );
            Assert.AreEqual(2, elements.Count);
        }
    }
}

