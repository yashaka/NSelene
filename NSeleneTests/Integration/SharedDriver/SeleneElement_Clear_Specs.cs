namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class SeleneElement_Clear_Specs : BaseTest
    {
        [Test]
        public void Clear_WaitsForVisibility_OfInitialyAbsent()
        {
            Given.OpenedEmptyPage();
            Given.OpenedPageWithBodyTimedOut(
                @"
                <input value='abracadabra'></input>
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                S("input").Clear();
            };

            Assert.That(act, Does.PassAfter(ShortTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("")
            );
        }
        
        [Test]
        public void Clear_IsRenderedInError_OnAbsentElementFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedEmptyPage();

            var act = () => { 
                S("input").Clear();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualWebElement.Clear()
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"input"}
                """));
        }
        
        [Test]
        public void Clear_IsRenderedInError_OnAbsentElementFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedEmptyPage();

            var act = () => {
                S("input").With(waitForNoOverlapFoundByJs: true).Clear();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.Clear()
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"input"}
                """));
        }

        [Test]
        [Ignore("Hidden input does not fail Clear()")]

        public void Clear_PassesOnHidden_IfAllowed() // NOT IMPLEMENTED
        {
            Given.OpenedPageWithBody(
                @"
                 <input value='abracadabra' style='display:none'></input>
                 "
            );

            var act = () =>
            {
                S("input").Clear();
            };

            Assert.That(act, Does.Pass());
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("")
            );
        }

        [Test]
        public void Clear_WaitsForVisibility_OfInitialyHidden()
        {
            Given.OpenedPageWithBody(
                @"
                <input value='abracadabra' style='display:none'></input>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementsByTagName('input')[0].style.display = 'block';
                ",
                ShortTimeoutSpan);

            var act = () =>
            {
                S("input").Clear();
            };

            Assert.That(act, Does.PassAfter(ShortTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("")
            );
        }
        
        [Test]
        public void Clear_IsRenderedInError_OnHiddenElementFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <input value='abracadabra' style='display:none'></input>
                "
            );

            var act = () => {
                S("input").Clear();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualWebElement.Clear()
                Reason:
                    element not interactable
                """));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("abracadabra")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("abracadabra")
            );
        }

        [Test]
        public void Clear_IsRenderedInError_OnHiddenElementFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <input value='abracadabra' style='display:none'></input>
                "
            );

            var act = () => {
                S("input").With(waitForNoOverlapFoundByJs: true).Clear();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.Clear()
                Reason:
                    javascript error: element is not visible
                """));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("abracadabra")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("abracadabra")
            );
        }

        [Test]
        public void Clear_WorksUnderOverlay_ByDefault()
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

                <input value='abracadabra'></input>
                "
            );
            var act = () =>
            {
                S("input").Clear();
            };

            Assert.That(act, Does.Pass());
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("")
            );
        }

        [Test]
        public void Clear_Waits_For_NoOverlay_WhenCustomized()
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

                <input value='abracadabra'></input>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('overlay').style.display = 'none';
                ",
                ShortTimeoutSpan);

            var act = () =>
            {
                S("input").With(waitForNoOverlapFoundByJs: true).Clear(); // TODO: this overlay works only for "overlayying at center of element", handle the "partial overlay" cases too!
            };

            Assert.That(act, Does.PassAfter(ShortTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("")
            );
        }

        [Test]
        public void Clear_IsRenderedInError_OnOverlappedWithOverlayFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
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

                <input value='abracadabra'></input>
                "
            );

            var act = () => {
                S("input").With(waitForNoOverlapFoundByJs: true).Clear();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.Clear()
                Reason:
                    Element: <input value="abracadabra">
                    is overlapped by: <div id="overlay" style="
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
                                    ">
                                </div>
                """));
        }
    }
}

