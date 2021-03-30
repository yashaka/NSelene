using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    using Harness;

    [TestFixture]
    public class SeleneElement_Find_Should : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void NotStartSearch_OnCreation()
        {
            Given.OpenedPageWithBody("<p id='#existing'>Hello!</p>");
            var nonExistentElement = S("#existing").Find("#not-existing-inner");
            Assert.IsNotEmpty(nonExistentElement.ToString());
        }

        [Test]
        public void NotStartSearch_EvenOnFollowingInnerSearch()
        {
            Given.OpenedEmptyPage();
            var nonExistentElement = S("#not-existing").Find("#not-existing-inner");
            Assert.IsNotEmpty(nonExistentElement.ToString());
        }

        [Test]
        public void PostponeSearch_UntilActualActionLikeQuestioiningValue()
        {
            Given.OpenedPageWithBody("<p id='existing'>Hello!</p>");
            var element = S("#existing").Find("#will-exist");
            When.WithBody(@"
                <p id='existing'>Hello! 
                    <input id='will-exist' type='submit' value='How r u?'></input>
                </p>"
            );
            Assert.AreEqual("How r u?", element.Value);
        }

        [Test]
        public void UpdateTheSearch_OnNextActualActionLikeQuestioiningValue()
        {
            Given.OpenedPageWithBody("<p id='existing'>Hello!</p>");
            var element = S("#existing").Find("#will-exist");
            When.WithBody(@"
                <p id='existing'>Hello! 
                    <input id='will-exist' type='submit' value='How r u?'></input>
                </p>"
            );
            Assert.AreEqual("How r u?", element.Value);
            When.WithBody(@"
                <p id='existing'>Hello! 
                    <input id='will-exist' type='submit' value='R u Ok?'></input>
                </p>"
            );
            Assert.AreEqual("R u Ok?", element.Value);
        }

        [Test]
        public void FindExactlyInsideParentElement()
        {
            Given.OpenedPageWithBody(@"
                <a href='#first' style='display:none'>go to Heading 2</a>
                <p>
                    <a href='#second'>go to Heading 2</a>
                    <h1 id='first'>Heading 1</h1>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            S("p").Find("a").Click();
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void WaitForVisibility_OnActionsLikeClick()
        {
            Given.OpenedPageWithBody(@"
                <p>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            Selene.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[0].style = 'display:block';
                    }, 
                    1000);"
            );
            S("p").Find("a").Click();
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void WaitForVisibility_OnActionsLikeClick_AfterFirstWaitingInSeleneS()
        {
            Given.OpenedPageWithBody(@"
                <p style='display:none'>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            Selene.ExecuteScript(@"
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
            S("p").Find("a").Click();
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void FailOnTimeout_DuringWaitingForVisibilityOnActionsLikeClick()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <p>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            Selene.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[0].style = 'display:block';
                    }, 
                    500);"
            );
            try {
                S("p").Find("a").Click();
                Assert.Fail("should fail on timeout before can be clicked");
            } catch (WebDriverTimeoutException) {
                Assert.IsFalse(Configuration.Driver.Url.Contains("second"));
            }
        }
    }
}

