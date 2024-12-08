namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class Selene_S_Specs : BaseTest
    {
        [Test]
        public void NotStartActualSearch()
        {
            Given.OpenedEmptyPage();
            var nonExistentElement = S("#not-existing-element-id");
            Assert.That(nonExistentElement.ToString(), Is.Not.Empty); 
        }

        [Test]
        public void PostponeTheSearchUntilActualActionLikeQuestioiningDisplayed()
        {
            Given.OpenedEmptyPage();
            var element = S("#will-be-existing-element-id");
            When.WithBody(@"<h1 id='will-be-existing-element-id'>Hello kitty:*</h1>");
            Assert.That(element.Displayed, Is.True);
        }

        [Test]
        public void UpdateSearchResults_OnNextActualActionLikeQuestioiningDisplayed()
        {
            Given.OpenedEmptyPage();
            var element = S("#will-be-existing-element-id");
            When.WithBody(@"<h1 id='will-be-existing-element-id'>Hello kitty:*</h1>");
            Assert.That(element.Displayed, Is.True);
            When.WithBody(@"<h1 id='will-be-existing-element-id' style='display:none'>Hello kitty:*</h1>");
            Assert.That(element.Displayed, Is.False);
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
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void FailWithTimeout_DuringWaitingForVisibilityOnActionsLikeClick()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(@"
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>"
            );
            Given.ExecuteScriptWithTimeout("""
                document.getElementsByTagName('a')[0].style = 'display:block';
                """,
                1500
            );

            var act = () =>
            {
                S("a").Click();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(a).ActualWebElement.Click()
                Reason:
                    element not interactable
                """, after: Configuration.Timeout));
            Assert.That(Configuration.Driver.Url, Does.Not.Contain("second"));
        }
    }
}

