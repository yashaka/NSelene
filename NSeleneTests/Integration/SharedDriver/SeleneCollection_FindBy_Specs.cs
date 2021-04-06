using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneCollectionSpec
{
    using System;
    using Harness;

    [TestFixture]
    public class SeleneCollection_FindBy_Specs : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void NotStartSearch_OnCreation()
        {
            Given.OpenedPageWithBody("<p>have no any items nor visible nor hidden</p>");
            var nonExistentElement = SS(".not-existing").FindBy(Be.Visible);
            Assert.IsNotEmpty(nonExistentElement.ToString()); 
        }

        [Test]
        public void NotStartSearch_EvenOnFollowingInnerSearch()
        {//TODO: consider testing using FindBy(Have.CssClass("..."))
            Given.OpenedPageWithBody("<p>have no any items nor visible nor hidden</p>");
            var nonExistentElement = SS(".not-existing").FindBy(Be.Visible).Find("#not-existing-inner");
            Assert.IsNotEmpty(nonExistentElement.ToString()); 
        }

        [Test]
        public void PostponeSearch_UntilActualActionLikeQuestioiningValue()
        {
            Given.OpenedEmptyPage();
            var element = SS("#will-exist>input").FindBy(Be.Visible);
            When.WithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?' style='display:none'></input>
                    <input id='answer1' type='submit' value='Good!'></input>
                    <input id='answer2' type='submit' value='Great!'></input>
                </p>"
            );
            Assert.AreEqual("Good!", element.Value);
        }

        [Test]
        public void UpdateSearch_OnNextActualActionLikeQuestioiningValue()
        {
            Given.OpenedEmptyPage();
            var element = SS("#will-exist>input").FindBy(Be.Visible);
            When.WithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?' style='display:none'></input>
                    <input id='answer1' type='submit' value='Good!'></input>
                </p>"
            );
            Assert.AreEqual("Good!", element.Value);
            Selene.ExecuteScript(@"
                document.getElementById('answer1').value = 'Great!';"
            );
            Assert.AreEqual("Great!", element.Value);
        }

        [Test]
        public void WaitForItsCondition_OnActionsLikeClick()
        {
            Given.OpenedPageWithBody(@"
                <a href='#first' style ='display:none'>go to Heading 1</a>
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
            SS("a").FindBy(Be.Visible).Click();
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void WaitForVisibility_OnActionsLikeClick_AlsoAfterSeleneSSWaiting()
        {
            Given.OpenedEmptyPage();
            When.WithBodyTimedOut(@"
                <a href='#first' style='display:none'>go to Heading 1</a>
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
            SS("a").FindBy(Be.Visible).Click();
            Assert.IsTrue(Selene.Url().Contains("second"));
        }

        [Test]
        public void FailOnTimeoutDuringWaitingForVisibility_OnActionsLikeClick()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <a href='#first' style='display:none'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h1 id='first'>Heading 1</h1>
                <h2 id='second'>Heading 2</h2>"
            );
            Selene.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[1].style = 'display:block';
                    }, 
                    500);"
            );
            try {
                SS("a").FindBy(Be.Visible).Click();
                Assert.Fail("should fail on timeout before can be clicked");
            } catch (TimeoutException) {
                Assert.IsFalse(Configuration.Driver.Url.Contains("second"));
                //TODO: consider asserting that actually 250ms passed
            }
        }
    }
}

