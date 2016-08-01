using NUnit.Framework;
using NSelene;
using static NSelene.Selene;
using OpenQA.Selenium;
using System.Threading;

namespace NSeleneTests
{
    [TestFixture]
    public class FilteredSCollectionSearchTests : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void FilteredSCollectionSearchIsLazyAndDoesNotStartOnCreation()
        {
            var nonExistingCollection = SS(".will-exist").FilterBy(Be.Visible);
            Assert.IsNotEmpty(nonExistingCollection.ToString()); 
        }

        [Test]
        public void FilteredSCollectionSearchIsPostponedUntilActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = SS("li").FilterBy(Be.Visible);
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear' style='display:none'>Bob</li>
                    <li class='will-appear'>Kate</li>
                </ul>"
            ); 
            Assert.AreEqual(1, elements.Count);
        }

        [Test]
        public void FilteredSCollectionSearchIsUpdatedOnNextActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = SS("li").FilterBy(Be.Visible);
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear' style='display:none'>Miller</li>
                    <li class='will-appear' style='display:none'>Julie Mao</li>
                </ul>"
            );
            Assert.AreEqual(0, elements.Count);
            Selene.ExecuteScript(@"
                document.getElementsByTagName('li')[0].style = 'display:block';"
            );
            Assert.AreEqual(1, elements.Count);
        }

        [Test]
        public void FilteredSCollectionSearchWaitsNothing()
        {
            Given.OpenedEmptyPage();
            var elements = SS("li").FilterBy(Be.Visible);
            When.WithBodyTimedOut(@"
                <ul>Hello to:
                    <li class='will-appear' style='display:none'>Miller</li>
                    <li class='will-appear' style='display:none'>Julie Mao</li>
                </ul>"
                , 
                500
            );
            When.ExecuteScriptWithTimeout(@"
                document.getElementsByTagName('a')[1].style = 'display:block';
                ", 1000
            );
            Assert.AreEqual(0, elements.Count);
        }
    }
}

