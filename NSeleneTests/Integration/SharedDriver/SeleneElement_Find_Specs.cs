using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    using System;
    using System.Linq;
    using Harness;

    [TestFixture]
    public class SeleneElement_Find_Specs : BaseTest
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
        public void WaitForVisibility_OnActionsLikeClick_AfterFirstWaitingForParentToAppear()
        {
            Configuration.Timeout = 1.0;
            Configuration.PollDuringWaits = 0.05;
            Given.OpenedPageWithBody(@"
                <p id='not-ready'>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            var beforeCall = DateTime.Now;
            Selene.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('p')[0].id = 'parent';
                    }, 
                    400);
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[0].style = 'display:block';
                    }, 
                    700);
                "
            );

            S("#parent").Find("a").Click();

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.7));
            Assert.Less(afterCall, beforeCall.AddSeconds(1.5));
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void WaitForVisibility_OnActionsLikeClick_AfterFirstWaitingForParentToBeDisplaid()
        {
            Configuration.Timeout = 1.0;
            Configuration.PollDuringWaits = 0.05;
            Given.OpenedPageWithBody(@"
                <p style='display:none'>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            var beforeCall = DateTime.Now;
            Selene.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('p')[0].style = 'display:block';
                    }, 
                    400);
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[0].style = 'display:block';
                    }, 
                    700);
                "
            );

            S("p").Find("a").Click();

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.7));
            Assert.Less(afterCall, beforeCall.AddSeconds(1.5));
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void FailOnTimeout_DuringWaitingForExistingOfParentOnActionsLikeClick()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <p id='not-ready' style='display:none'>
                    <a href='#second'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("#parent").Find("a").Click();
                Assert.Fail("should fail on timeout before can be clicked");
            }

            catch (TimeoutException error) 
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
                
                Assert.IsFalse(Configuration.Driver.Url.Contains("second"));

                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(#parent).Element(a).ActualWebElement.Click()", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains(
                    "no such element: Unable to locate element: "
                    + "{\"method\":\"css selector\",\"selector\":\"#parent\"}", lines);
            }
        }

        [Test]
        public void FailOnTimeout_DuringWaitingForVisibilityOfParentOnActionsLikeClick()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <p style='display:none'>
                    <a href='#second'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("p").Find("a").Click();
                Assert.Fail("should fail on timeout before can be clicked");
            }

            catch (TimeoutException error) 
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
                
                Assert.IsFalse(Configuration.Driver.Url.Contains("second"));

                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(p).Element(a).ActualWebElement.Click()", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("Element not visible:", lines);
                Assert.Contains("<p style=\"display:none\">", lines);
                Assert.Contains("<a href=\"#second\">go to Heading 2</a>", lines);
                Assert.Contains("</p>", lines);
            }
        }

        [Test]
        public void FailOnTimeout_DuringWaitingForVisibilityOfChildOnActionsLikeClick()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <p>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("p").Find("a").Click();
                Assert.Fail("should fail on timeout before can be clicked");
            }

            catch (TimeoutException error) 
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
                
                Assert.IsFalse(Configuration.Driver.Url.Contains("second"));

                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(p).Element(a).ActualWebElement.Click()", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("element not interactable", lines);
            }
        }
    }
}

