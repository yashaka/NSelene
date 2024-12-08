namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class SeleneElement_Click_Specs : BaseTest
    {
        // TODO: move here error messages tests, and at least some ClickByJs tests...
        
        [Test]
        public void Click_WaitsForVisibility_OfInitialyAbsent()
        {
            Given.OpenedEmptyPage();
            Given.OpenedPageWithBodyTimedOut(
                @"
                <a href='#second'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>
                ",
                ShortTimeoutSpan);

            var act = () =>
            {
                S("a").Click();
            };

            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void Click_WaitsForVisibility_OfInitialyHidden()
        {
            Given.OpenedPageWithBody(
                @"
                <a id='link' href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('link').style.display = 'block';
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                S("a").Click();
            };

            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }
        [Test]
        public void Click_IsRenderedInError_OnHiddenFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <a id='link' href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>
                "
            );

            var act = () => {
                S("a").Click();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(a).ActualWebElement.Click()
                Reason:
                    element not interactable
                """,
                after: Configuration.Timeout
            ));
        }

        [Test]
        public void Click_Waits_For_NoOverlay()
        {
            Given.OpenedPageWithBody(
                @"
                <div 
                    id='overlay' 
                    style='
                        display:block;
                        position: fixed;
                        display: block;
                        width: 100%;
                        height: 100%;
                        top: 0;
                        left: 0;
                        right: 0;
                        bottom: 0;
                        background-color: rgba(0,0,0,0.1);
                        z-index: 2;
                        cursor: pointer;
                    '
                >
                </div>

                <a id='link' href='#second' style='display:block'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('overlay').style.display = 'none';
                ",
                ShortTimeoutSpan);

            var act = () =>
            {
                S("a").Click();
            };
            
            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void Click_IsRenderedInError_OnOverlappedWithOverlayFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <div 
                    id='overlay' 
                    style='
                        display: block;
                        position: fixed;
                        display: block;
                        width: 100%;
                        height: 100%;
                        top: 0;
                        left: 0;
                        right: 0;
                        bottom: 0;
                        background-color: rgba(0,0,0,0.1);
                        z-index: 2;
                        cursor: pointer;
                    '
                >
                </div>

                <a id='link' href='#second'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>
                "
            );

            var act = () => {
                S("a").Click();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(a).ActualWebElement.Click()
                Reason:
                    element click intercepted: Element <a id="link" href="#second">...</a> is not clickable at point 
                """,
                after: Configuration.Timeout
            ));
        }
    }
}

