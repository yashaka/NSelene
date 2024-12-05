namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    [TestFixture]
    public class SeleneElement_Find_Specs : BaseTest
    {
        [Test]
        public void NotStartSearch_OnCreation()
        {
            Given.OpenedPageWithBody("<p id='#existing'>Hello!</p>");
            var nonExistentElement = S("#existing").Element("#not-existing-inner");
            Assert.That(nonExistentElement.ToString(), Is.Not.Empty);
        }

        [Test]
        public void NotStartSearch_EvenOnFollowingInnerSearch()
        {
            Given.OpenedEmptyPage();
            var nonExistentElement = S("#not-existing").Element("#not-existing-inner");
            Assert.That(nonExistentElement.ToString(), Is.Not.Empty);
        }

        [Test]
        public void PostponeSearch_UntilActualActionLikeQuestioiningValue()
        {
            Given.OpenedPageWithBody("<p id='existing'>Hello!</p>");
            var element = S("#existing").Element("#will-exist");
            When.WithBody(@"
                <p id='existing'>Hello! 
                    <input id='will-exist' type='submit' value='How r u?'></input>
                </p>"
            );
            Assert.That(element.Value, Is.EqualTo("How r u?"));
        }

        [Test]
        public void UpdateTheSearch_OnNextActualActionLikeQuestioiningValue()
        {
            Given.OpenedPageWithBody("<p id='existing'>Hello!</p>");
            var element = S("#existing").Element("#will-exist");
            When.WithBody(@"
                <p id='existing'>Hello! 
                    <input id='will-exist' type='submit' value='How r u?'></input>
                </p>"
            );
            Assert.That(element.Value, Is.EqualTo("How r u?"));
            When.WithBody(@"
                <p id='existing'>Hello! 
                    <input id='will-exist' type='submit' value='R u Ok?'></input>
                </p>"
            );
            Assert.That(element.Value, Is.EqualTo("R u Ok?"));
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

            S("p").Element("a").Click();

            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
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
            Given.ExecuteScriptWithTimeout($$"""
                document.getElementsByTagName('a')[0].style = 'display:block';
                """,
                1000);

            S("p").Element("a").Click();

            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void WaitForVisibility_OnActionsLikeClick_AfterFirstWaitingForParentToAppear()
        {
            Given.OpenedPageWithBody(@"
                <p id='not-ready'>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            Given.ExecuteScriptWithTimeout($$"""
                document.getElementsByTagName('p')[0].id = 'parent';
                """,
                400);
            Given.ExecuteScriptWithTimeout($$"""
                document.getElementsByTagName('a')[0].style = 'display:block';
                """,
                700);

            var act = () => {
                S("#parent").Element("a").Click();
            };

            Assert.That(act, Does.PassWithin(TimeSpan.FromMilliseconds(700), DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void WaitForVisibility_OnActionsLikeClick_AfterFirstWaitingForParentToBeDisplaid()
        {
            Given.OpenedPageWithBody(@"
                <p style='display:none'>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            Given.ExecuteScriptWithTimeout($$"""
                document.getElementsByTagName('p')[0].style = 'display:block';
                """,
                400);
            Given.ExecuteScriptWithTimeout($$"""
                document.getElementsByTagName('a')[0].style = 'display:block';
                """,
                700);

            var act = () =>
            {
                S("p").Element("a").Click();
            };
            
            Assert.That(act, Does.PassWithin(TimeSpan.FromMilliseconds(700), DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void FailOnTimeout_DuringWaitingForExistingOfParentOnActionsLikeClick()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(@"
                <p id='not-ready' style='display:none'>
                    <a href='#second'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );
            
            var act = () => { 
                S("#parent").Element("a").Click();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(#parent).Element(a).ActualWebElement.Click()
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"#parent"}
                """));
            Assert.That(Configuration.Driver.Url, Does.Not.Contain("second"));
        }

        [Test]
        public void FailOnTimeout_DuringWaitingForVisibilityOfParentOnActionsLikeClick()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(@"
                <p style='display:none'>
                    <a href='#second'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );

            var act = () => {
                S("p").Element("a").Click();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(p).Element(a).ActualWebElement.Click()
                Reason:
                    Element not visible:
                <p style="display:none">
                                    <a href="#second">go to Heading 2</a>
                                    </p>
                """));
            Assert.That(Configuration.Driver.Url, Does.Not.Contain("second"));
        }

        [Test]
        public void FailOnTimeout_DuringWaitingForVisibilityOfChildOnActionsLikeClick()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(@"
                <p>
                    <a href='#second' style='display:none'>go to Heading 2</a>
                    <h2 id='second'>Heading 2</h2>
                </p>"
            );

            var act = () => {
                S("p").Element("a").Click();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(p).Element(a).ActualWebElement.Click()
                Reason:
                    element not interactable
                """));
            Assert.That(Configuration.Driver.Url, Does.Not.Contain("second"));
        }
    }
}

