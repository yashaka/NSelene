using NUnit.Framework;
using NSelene;
using static NSelene.Utils;
using OpenQA.Selenium;

namespace NSeleneTests.WithManagedBrowserBeforAndAfterAllTests
{
    [TestFixture]
    public class FoundByConditionSElementSearchTests
    {

        [TearDown]
        public void TeardownTest()
        {
            Config.Timeout = 4;
        }
        
        [Test]
        public void FoundByConditionSElementSearchIsLazyAndDoesNotStartOnCreation()
        {
            Given.OpenedPageWithBody("<p>have no any items nor visible nor hidden</p>");
            var nonExistentElement = SS(".not-existing").FindBy(Be.Visible);
        }

        [Test]
        public void FoundByConditionSElementSearchIsLazyAndDoesNotStartEvenOnFollowingInnerSearch()
        {//TODO: consider testing using FindBy(Have.CssClass("..."))
            Given.OpenedPageWithBody("<p>have no any items nor visible nor hidden</p>");
            var nonExistentElement = SS(".not-existing").FindBy(Be.Visible).Find("#not-existing-inner");
        }

        [Test]
        public void FoundByConditionSElementSearchIsPostponedUntilActualActionLikeQuestioiningValue()
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
            Assert.AreEqual("Good!", element.GetValue());
        }

        [Test]
        public void InnerSElementSearchIsUpdatedOnNextActualActionLikeQuestioiningValue()
        {
            Given.OpenedEmptyPage();
            var element = SS("#will-exist>input").FindBy(Be.Visible);
            When.WithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?' style='display:none'></input>
                    <input id='answer1' type='submit' value='Good!'></input>
                </p>"
            );
            Assert.AreEqual("Good!", element.GetValue());
            Utils.ExecuteScript(@"
                document.getElementById('answer1').value = 'Great!';"
            );
            Assert.AreEqual("Great!", element.GetValue());
        }

        [Test]
        public void FoundSElementSearchWaitsForItsConditionOnActionsLikeClick()
        {
            Given.OpenedPageWithBody(@"
                <a href='#first' style ='display:none'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h1 id='first'>Heading 1</h1>
                <h2 id='second'>Heading 2</h2>"
            );
            Utils.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[1].style = 'display:block';
                    }, 
                    500);"
            );
            SS("a").FindBy(Be.Visible).Click();
            Assert.IsTrue(Utils.GetDriver().Url.Contains("second"));
        }

        [Test]
        public void BothSCollectionAndFoundByConditionSElementSearchWaitsForVisibilityOnActionsLikeClick()
        {
            Given.OpenedEmptyPage();
            When.WithBody(@"
                <a href='#first' style='display:none'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h1 id='first'>Heading 1</h1>
                <h2 id='second'>Heading 2</h2>"
                ,
                500
            );
            Utils.ExecuteScript(@"
               setTimeout(
                    function(){
                        document.getElementsByTagName('a')[1].style = 'display:block';
                    }, 
                    1000);"
            );
            SS("a").FindBy(Be.Visible).Click();
            Assert.IsTrue(Utils.Url().Contains("second"));
        }

        [Test]
        public void FoundByConditionSElementSearchFailsOnTimeoutDuringWaitingForVisibilityOnActionsLikeClick()
        {
            Config.Timeout = 0.5;
            Given.OpenedPageWithBody(@"
                <a href='#first' style='display:none'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h1 id='first'>Heading 1</h1>
                <h2 id='second'>Heading 2</h2>"
            );
            Utils.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('a')[1].style = 'display:block';
                    }, 
                    1000);"
            );
            try {
                SS("a").FindBy(Be.Visible).Click();
                Assert.Fail("should fail on timeout before can be clicked");
            } catch (WebDriverTimeoutException ex) {
                Assert.IsFalse(Utils.GetDriver().Url.Contains("second"));
                //TODO: consider asserting that actually 500ms passed
            }
        }
    }
}

