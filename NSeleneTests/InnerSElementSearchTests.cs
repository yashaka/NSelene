using NUnit.Framework;
using NSelene;
using static NSelene.Utils;
using OpenQA.Selenium;
using System;

namespace NSeleneTests
{
    [TestFixture]
    public class InnerSElementSearchTests : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void InnerSElementSearchIsLazyAndDoesNotStartOnCreation()
        {
            Given.OpenedPageWithBody("<p id='#existing'>Hello!</p>");
            var nonExistentElement = S("#existing").Find("#not-existing-inner");
            Assert.IsNotEmpty(nonExistentElement.ToString());
        }

        [Test]
        public void SElementSearchIsLazyAndDoesNotStartEvenOnFollowingInnerSearch()
        {
            Given.OpenedEmptyPage();
            var nonExistentElement = S("#not-existing").Find("#not-existing-inner");
            Assert.IsNotEmpty(nonExistentElement.ToString());
        }

        [Test]
        public void InnerSElementSearchIsPostponedUntilActualActionLikeQuestioiningValue()
        {
            Given.OpenedPageWithBody("<p id='existing'>Hello!</p>");
            var element = S("#existing").Find("#will-exist");
            When.WithBody(@"
                <p id='existing'>Hello! 
                    <input id='will-exist' type='submit' value='How r u?'></input>
                </p>"
            );
            Assert.AreEqual("How r u?", element.GetValue());
        }

        [Test]
        public void InnerSElementSearchIsUpdatedOnNextActualActionLikeQuestioiningValue()
        {
            Given.OpenedPageWithBody("<p id='existing'>Hello!</p>");
            var element = S("#existing").Find("#will-exist");
            When.WithBody(@"
                <p id='existing'>Hello! 
                    <input id='will-exist' type='submit' value='How r u?'></input>
                </p>"
            );
            Assert.AreEqual("How r u?", element.GetValue());
            When.WithBody(@"
                <p id='existing'>Hello! 
                    <input id='will-exist' type='submit' value='R u Ok?'></input>
                </p>"
            );
            Assert.AreEqual("R u Ok?", element.GetValue());
        }

        [Test]
        public void InnerSElementSearchFindsExactlyInsideParentElement()
        {
            Given.OpenedPageWithBody(@"
                <a href='#first' style='display:none'>go to Heading 2</a>
                <p>
                    <a href='#second'>go to Heading 2</a>
                    <h1 id='first'>Heading 1</h1>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            S("p").S("a").Click();
            Assert.IsTrue(Utils.GetWebDriver().Url.Contains("second"));
        }

        [Test]
        public void InnerSElementSearchWaitsForVisibilityOnActionsLikeClick()
        {
            Given.OpenedPageWithBody(@"
                <p>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            Utils.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[0].style = 'display:block';
                    }, 
                    1000);"
            );
            S("p").S("a").Click();
            Assert.IsTrue(Utils.GetWebDriver().Url.Contains("second"));
        }

        [Test]
        public void BothNormalAndInnerSElementSearchWaitsForVisibilityOnActionsLikeClick()
        {
            Given.OpenedPageWithBody(@"
                <p style='display:none'>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            Utils.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('p')[0].style = 'display:block';
                    }, 
                    500);
               setTimeout(
                    function(){
                        document.getElementsByTagName('a')[0].style = 'display:block';
                    }, 
                    1000);"
            );
            S("p").S("a").Click();
            Assert.IsTrue(Utils.GetWebDriver().Url.Contains("second"));
        }

        [Test]
        public void InnerSElementSearchFailsOnTimeoutDuringWaitingForVisibilityOnActionsLikeClick()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <p>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            Utils.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[0].style = 'display:block';
                    }, 
                    500);"
            );
            try {
                S("p").S("a").Click();
                Assert.Fail("should fail on timeout before can be clicked");
            } catch (WebDriverTimeoutException) {
                Assert.IsFalse(Utils.GetWebDriver().Url.Contains("second"));
            }
        }
    }
}

