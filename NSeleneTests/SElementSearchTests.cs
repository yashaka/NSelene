using NUnit.Framework;
using NSelene;
using static NSelene.Utils;
using OpenQA.Selenium;

namespace NSeleneTests.WithManagedBrowserBeforAndAfterAllTests
{
    [TestFixture]
    public class SElementSearchTests
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void SElementSearchIsLazyAndDoesNotStartOnCreation()
        {
            Given.OpenedEmptyPage();
            var nonExistentElement = S("#not-existing-element-id");
        }

        [Test]
        public void SElementSearchIsPostponedUntilActualActionLikeQuestioiningDisplayed()
        {
            Given.OpenedEmptyPage();
            var element = S("#will-be-existing-element-id");
            When.WithBody(@"<h1 id='will-be-existing-element-id'>Hello kitty:*</h1>");
            Assert.IsTrue(element.Displayed);
        }

        [Test]
        public void SElementSearchIsUpdatedOnNextActualActionLikeQuestioiningDisplayed()
        {
            Given.OpenedEmptyPage();
            var element = S("#will-be-existing-element-id");
            When.WithBody(@"<h1 id='will-be-existing-element-id'>Hello kitty:*</h1>");
            Assert.IsTrue(element.Displayed);
            When.WithBody(@"<h1 id='will-be-existing-element-id' style='display:none'>Hello kitty:*</h1>");
            Assert.IsFalse(element.Displayed);
        }

        [Test]
        public void SElementSearchWaitsForVisibilityOnActionsLikeClick()
        {
            Given.OpenedPageWithBody(@"
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>"
            );
            Utils.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[0].style = 'display:block';
                    }, 
                    500);"
            );
            S("a").Click();
            Assert.IsTrue(Utils.GetDriver().Url.Contains("second"));
        }

        [Test]
        public void SElementSearchFailsOnTimeoutDuringWaitingForVisibilityOnActionsLikeClick()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>"
            );
            Utils.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[0].style = 'display:block';
                    }, 
                    500);"
            );

            // TODO: consider using Assert.Throws<WebDriverTimeoutException>(() => { ... })
            try {
                S("a").Click();
                Assert.Fail("should fail on timeout before can be clicked");
            } catch (WebDriverTimeoutException ex) {
                Assert.IsFalse(Utils.GetDriver().Url.Contains("second"));
            }
        }
    }
}

