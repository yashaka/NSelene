using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    using System;
    using System.Linq;
    using Harness;

    [TestFixture]
    public class Selene_S_Should : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void NotStartActualSearch()
        {
            Given.OpenedEmptyPage();
            var nonExistentElement = S("#not-existing-element-id");
            Assert.IsNotEmpty(nonExistentElement.ToString()); 
        }

        [Test]
        public void PostponeTheSearchUntilActualActionLikeQuestioiningDisplayed()
        {
            Given.OpenedEmptyPage();
            var element = S("#will-be-existing-element-id");
            When.WithBody(@"<h1 id='will-be-existing-element-id'>Hello kitty:*</h1>");
            Assert.IsTrue(element.Displayed);
        }

        [Test]
        public void UpdateSearchResults_OnNextActualActionLikeQuestioiningDisplayed()
        {
            Given.OpenedEmptyPage();
            var element = S("#will-be-existing-element-id");
            When.WithBody(@"<h1 id='will-be-existing-element-id'>Hello kitty:*</h1>");
            Assert.IsTrue(element.Displayed);
            When.WithBody(@"<h1 id='will-be-existing-element-id' style='display:none'>Hello kitty:*</h1>");
            Assert.IsFalse(element.Displayed);
        }

        [Test]
        public void WaitForVisibility_OnActionsLikeClick()
        {
            Given.OpenedPageWithBody(@"
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>"
            );
            Selene.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[0].style = 'display:block';
                    }, 
                    500);"
            );
            S("a").Click();
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void FailWithTimeout_DuringWaitingForVisibilityOnActionsLikeClick()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>"
            );
            Selene.ExecuteScript(@"
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
            } catch (TimeoutException) {
                Assert.IsFalse(Configuration.Driver.Url.Contains("second"));
            }
        }
    }
}

