namespace NSelene.Tests.Integration.SharedDriver.SeleneCollectionSpec
{
    [TestFixture]
    public class SeleneCollection_FindBy_Specs : BaseTest
    {
        [Test]
        public void NotStartSearch_OnCreation()
        {
            Given.OpenedPageWithBody("<p>have no any items nor visible nor hidden</p>");
            var nonExistentElement = SS(".not-existing").ElementBy(Be.Visible);
            Assert.That(nonExistentElement.ToString(), Is.Not.Empty); 
        }

        [Test]
        public void NotStartSearch_EvenOnFollowingInnerSearch()
        {//TODO: consider testing using FindBy(Have.CssClass("..."))
            Given.OpenedPageWithBody("<p>have no any items nor visible nor hidden</p>");
            var nonExistentElement = SS(".not-existing").ElementBy(Be.Visible).Element("#not-existing-inner");
            Assert.That(nonExistentElement.ToString(), Is.Not.Empty); 
        }

        [Test]
        public void PostponeSearch_UntilActualActionLikeQuestioiningValue()
        {
            Given.OpenedEmptyPage();
            var element = SS("#will-exist>input").ElementBy(Be.Visible);
            When.WithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?' style='display:none'></input>
                    <input id='answer1' type='submit' value='Good!'></input>
                    <input id='answer2' type='submit' value='Great!'></input>
                </p>"
            );
            Assert.That(element.Value, Is.EqualTo("Good!"));
        }

        [Test]
        public void UpdateSearch_OnNextActualActionLikeQuestioiningValue()
        {
            Given.OpenedEmptyPage();
            var element = SS("#will-exist>input").ElementBy(Be.Visible);
            When.WithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?' style='display:none'></input>
                    <input id='answer1' type='submit' value='Good!'></input>
                </p>"
            );
            Assert.That(element.Value, Is.EqualTo("Good!"));
            Selene.ExecuteScript(@"
                document.getElementById('answer1').value = 'Great!';"
            );
            Assert.That(element.Value, Is.EqualTo("Great!"));
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
            SS("a").ElementBy(Be.Visible).Click();
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
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
                TimeSpan.FromMilliseconds(250)
            );
            Selene.ExecuteScript(@"
               setTimeout(
                    function(){
                        document.getElementsByTagName('a')[1].style = 'display:block';
                    }, 
                    500);"
            );
            SS("a").ElementBy(Be.Visible).Click();
            Assert.That(Selene.Url(), Does.Contain("second"));
        }

        [Test]
        public void FailOnTimeoutDuringWaitingForVisibility_OnActionsLikeClick()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(@"
                <a href='#first' style='display:none'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h1 id='first'>Heading 1</h1>
                <h2 id='second'>Heading 2</h2>"
            );

            var act = () =>
            {
                SS("a").ElementBy(Be.Visible).Click();
            };

            Assert.That(act, Does.Timeout($$"""
                    Browser.All(a).FirstBy(Be.Visible).ActualWebElement.Click()
                    Reason:
                        
                      Actual html elements : [<a href="#first" style="display:none">go to Heading 1</a>,<a href="#second" style="display:none">go to Heading 2</a>]
                    """, after: Configuration.Timeout
            ));
            Assert.That(Configuration.Driver.Url, Does.Not.Contain("second"));
        }
    }
}

