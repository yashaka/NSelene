using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneCollectionSpec
{
    using System;
    using Harness;

    [TestFixture]
    public class SeleneCollection_Indexer_Specs : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void NotStartSearch_OnCreation()
        {
            Given.OpenedPageWithBody("<p>have no any items</p>");
            var nonExistentElement = SS(".not-existing")[10];
        }

        [Test]
        public void NotStartSearch_EvenOnFollowingInnerSearch()
        {
            Given.OpenedPageWithBody("<p>have no any items</p>");
            var nonExistentElement = SS(".not-existing")[10].Find("#not-existing-inner");
            Assert.IsNotEmpty(nonExistentElement.ToString()); 
        }

        [Test]
        public void PostponeSearch_UntilActualActionLikeQuestioiningValue()
        {
            Given.OpenedEmptyPage();
            var element = SS("#will-exist>input")[1];
            When.WithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?'></input>
                    <input id='answer' type='submit' value='Good!'></input>
                </p>"
            );
            Assert.AreEqual("Good!", element.Value);
        }

        [Test]
        public void UpdateSearch_OnNextActualActionLikeQuestioiningValue()
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
        public void WaitForVisibility_OnActionsLikeClick()
        {
            Given.OpenedPageWithBody(@"
                <a href='#first'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h1 id='first'>Heading 1</h1>
                <h2 id='second'>Heading 2</h2>"
            );
            Selene.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[1].style = 'display:block';
                    }, 
                    250);"
            );
            SS("a")[1].Click();
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void WaitForAppearance_OnActionsLikeClick()
        {
            Given.OpenedPageWithBody(@"
                <a href='#first'>go to Heading 1</a>
                <h1 id='first'>Heading 1</h1>
                <h2 id='second'>Heading 2</h2>"
                                    );
            When.WithBodyTimedOut(@"
                <a href='#first'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h1 id='first'>Heading 1</h1>
                <h2 id='second'>Heading 2</h2>"
                , 250
            );
            Selene.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[1].style = 'display:block';
                    }, 
                    500);"
            );
            SS("a")[1].Click();
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void WaitForVisibility_OnActionsLikeClick_AfterSeleneSSWaiting()
        {// TODO: think on breaking down this test into two, or add one more explicitly testing implicit wait in get
            // todo: SS can't wait;) something is wrong with the test name;)
            Given.OpenedEmptyPage();
            When.WithBodyTimedOut(@"
                <a href='#first'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h1 id='first'>Heading 1</h1>
                <h2 id='second'>Heading 2</h2>"
                ,
                250
            );
            Selene.ExecuteScript(@"
               setTimeout(
                    function(){
                        document.getElementsByTagName('a')[1].style = 'display:block';
                    }, 
                    500);"
            );
            SS("a")[1].Click();
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void FailOnTimeout_DuringWaitingForVisibilityOnActionsLikeClick()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <a href='#first'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h1 id='first'>Heading 1</h1>
                <h2 id='second'>Heading 2</h2>"
            );
            When.ExecuteScriptWithTimeout(@"
                document.getElementsByTagName('a')[1].style = 'display:block';"
                ,
                500
            );
            try {
                SS("a")[1].Click();
                Assert.Fail("should fail on timeout before can be clicked");
            } catch (TimeoutException) {
                Assert.IsFalse(Configuration.Driver.Url.Contains("second"));
            }
        }
    }
}

