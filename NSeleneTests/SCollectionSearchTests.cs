using NUnit.Framework;
using NSelene;
using static NSelene.Utils;
using OpenQA.Selenium;

namespace NSeleneTests.WithManagedBrowserBeforAndAfterAllTests
{
    [TestFixture]
    public class SCollectionSearchTests
    {

        [TearDown]
        public void TeardownTest()
        {
            Config.Timeout = 4;
        }
        
        [Test]
        public void SCollectionSearchIsLazyAndDoesNotStartOnCreation()
        {
            var nonExistingCollection = SS(".not-existing"); 
                // TODO: think on improving... actually it does not tell search is not started
                // it would tell if browser is quited at the moment... but it is not...
        }

        [Test]
        public void SCollectionSearchIsPostponedUntilActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = SS(".will-appear");
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear'>Bob</li>
                    <li class='will-appear'>Kate</li>
                </ul>");
            Assert.AreEqual(2, elements.GetCount());
        }

        [Test]
        public void SCollectionSearchIsUpdatedOnNextActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = SS(".will-appear");
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear'>Bob</li>
                    <li class='will-appear'>Kate</li>
                </ul>");
            Assert.AreEqual(2, elements.GetCount());
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear'>Bob</li>
                    <li class='will-appear'>Kate</li>
                    <li class='will-appear'>Joe</li>
                </ul>");
            Assert.AreEqual(3, elements.GetCount());
        }

        [Test]
        public void SCollectionSearchWaitsNothing()
        {
            Given.OpenedEmptyPage();
            var elements = SS(".will-appear");
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear'>Bob</li>
                    <li class='will-appear' style='display:none'>Kate</li>
                </ul>"
            );
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear'>Bob</li>
                    <li class='will-appear' style='display:none'>Kate</li>
                    <li class='will-appear'>Bobik</li>
                </ul>",
                500
            );
            Assert.AreEqual(2, elements.GetCount());
        }
    }
}

