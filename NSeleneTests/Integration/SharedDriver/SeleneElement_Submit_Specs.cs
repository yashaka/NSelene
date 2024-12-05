namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class SeleneElement_Submit_Specs : BaseTest
    {
        [Test]
        public void Submit_WaitsForVisibility_OfInitialyAbsent()
        {
            Given.OpenedEmptyPage();
            Given.OpenedPageWithBodyTimedOut(
                @"
                <form action='#second'>go to Heading 2</form>
                <h2 id='second'>Heading 2</h2>
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                S("form").Submit();
            };

            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }
        
        [Test]
        public void Submit_IsRenderedInError_OnAbsentElementFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedEmptyPage();

            var act = () => {
                S("form").Submit();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(form).ActualWebElement.Submit()
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"form"}
                """));
        }
        
        [Test]
        public void Submit_IsRenderedInError_OnAbsentElementFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedEmptyPage();

            var act = () => {
                S("form").With(waitForNoOverlapFoundByJs: true).Submit();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(form).ActualNotOverlappedWebElement.Submit()
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"form"}
                """));
        }

        [Test]
        public void Submit_Works_OnHidden_ByDefault() // TODO: but should it?
        // public void Submit_WaitsForVisibility_OfInitialyHidden()
        {
            Given.OpenedPageWithBody(
                @"
                <form action='#second' style='display:none'>go to Heading 2</form>
                <h2 id='second'>Heading 2</h2>
                "
            );
            // Given.ExecuteScriptWithTimeout(
            //     @"
            //     document.getElementsByTagName('form')[0].style.display = 'block';
            //     ",
            //     PollingPeriod
            // );

            var act = () =>
            {
                S("form").Submit();
            };

            Assert.That(act, Does.PassBefore(DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void Submit_WaitsForVisibility_OfInitialyHidden_WhenCustomizedToWaitForNoOverlapFoundByJs()
        {
            Given.OpenedPageWithBody(
                @"
                <form action='#second' style='display:none'>go to Heading 2</form>
                <h2 id='second'>Heading 2</h2>
                "
            );
            Given.ExecuteScriptWithTimeout(
                 @"
                document.getElementsByTagName('form')[0].style.display = 'block';
                ",
                 ShortTimeoutSpan
             );

            var act = () =>
            {
                S("form").With(waitForNoOverlapFoundByJs: true).Submit();
            };

            Assert.That(act, Does.PassBefore(DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }
        
        [Test]
        public void Submit_IsRenderedInError_OnHiddenElementFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <form action='#second' style='display:none'>go to Heading 2</form>
                <h2 id='second'>Heading 2</h2>
                "
            );

            var act = () => {
                S("form").With(waitForNoOverlapFoundByJs: true).Submit();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(form).ActualNotOverlappedWebElement.Submit()
                Reason:
                    javascript error: element is not visible
                """));
        }

        [Test]
        public void Submit_Works_UnderOverlay()
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

                <form action='#second'>go to Heading 2</form>
                <h2 id='second'>Heading 2</h2>
                "
            );
            
            var act = () =>
            {
                S("form").Submit(); // TODO: this overlay works only for "overlayying at center of element", handle the "partial overlay" cases too!
            };
        
            Assert.That(act, Does.PassBefore(DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void Submit_Waits_For_NoOverlay_WhenCustomized()
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

                <form action='#second'>go to Heading 2</form>
                <h2 id='second'>Heading 2</h2>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('overlay').style.display = 'none';
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                S("form").With(waitForNoOverlapFoundByJs: true).Submit(); // TODO: this overlay works only for "overlayying at center of element", handle the "partial overlay" cases too!
            };

            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void Submit_IsRenderedInError_OnOverlappedWithOverlayFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
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

                <form action='#second'>go to H2</form>
                <h2 id='second'>Heading 2</h2>
                "
            );

            var act = () => {
                S("form").With(waitForNoOverlapFoundByJs: true).Submit();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(form).ActualNotOverlappedWebElement.Submit()
                Reason:
                    Element: <form action="#second">go to H2</form>
                    is overlapped by: <div id="overlay"
                """));
        }

        [Test]
        public void Submit_Fails_OnNonFormElement()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <a href='#second'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>
                "
            );

            var act = () => {
                S("a").Submit();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(a).ActualWebElement.Submit()
                Reason:
                    javascript error: Unable to find owning document
                """));
            Assert.That(Configuration.Driver.Url, Does.Not.Contain("second"));
        }
    }
}

