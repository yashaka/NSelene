namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class SeleneElement_SetValue_Specs : BaseTest
    {
        // TODO: move here some SetValueByJs tests

        [Test]
        public void SetValue_WaitsForVisibility_OfInitialyAbsent()
        {
            Given.OpenedEmptyPage();
            Given.OpenedPageWithBodyTimedOut(
                @"
                <input value='initial'></input>
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                S("input").SetValue("overwritten");
            };
            
            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("overwritten")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("overwritten")
            );
        }
        
        [Test]
        public void SetValue_IsRenderedInError_OnAbsentElementFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedEmptyPage();

            var act = () => {
                S("input").SetValue("overwritten");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualWebElement.Clear().SendKeys(overwritten)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"input"}
                """,
                after: Configuration.Timeout
            ));
        }

        [Test]
        public void SetValue_IsRenderedInError_OnAbsentElementFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedEmptyPage();

            var act = () => {
                S("input").With(waitForNoOverlapFoundByJs: true).SetValue("overwritten");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.Clear().SendKeys(overwritten)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"input"}
                """,
                after: Configuration.Timeout
            ));
        }
        
        [Test]
        public void SetValue_FailsOnHiddenInputOfTypeFile() // TODO: should we allow it here like in send keys? kind of sounds natural... but can we do that without drawing performance?
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <input type='file' style='display:none'></input>
                "
            );

            var act = () => {
                S("[type=file]").SetValue(EmptyHtmlPath);
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element([type=file]).ActualWebElement.Clear().SendKeys({{EmptyHtmlPath}})
                Reason:
                    element not interactable
                """,
                after: Configuration.Timeout
            ));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomAttribute("value"),
                Is.EqualTo(null)
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("")
            );
        }
        
        [Test]
        public void SetValue_FailsOnHiddenInputOfTypeFile_WhenCustomizedToWaitForNoOverlapFoundByJs() // TODO: should we allow it here like in send keys? kind of sounds natural... but can we do that without drawing performance?
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <input type='file' style='display:none'></input>
                "
            );

            var act = () => {
                S("[type=file]").With(waitForNoOverlapFoundByJs: true).SetValue(EmptyHtmlPath);
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element([type=file]).ActualNotOverlappedWebElement.Clear().SendKeys({{EmptyHtmlPath}})
                Reason:
                    javascript error: element is not visible
                """,
                after: Configuration.Timeout
            ));

            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomAttribute("value"),
                Is.EqualTo(null)
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("")
            );
        }

        [Test]
        public void SetValue_WaitsForVisibility_OfInitialyHidden()
        {
            Given.OpenedPageWithBody(
                @"
                <input value='initial' style='display:none'></input>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementsByTagName('input')[0].style.display = 'block';
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                S("input").SetValue("overwritten");
            };

            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("overwritten")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("overwritten")
            );
        }
        
        [Test]
        public void SetValue_IsRenderedInError_OnHiddenElementFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <input value='initial' style='display:none'></input>
                "
            );

            var act = () => {
                S("input").SetValue("overwritten");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualWebElement.Clear().SendKeys(overwritten)
                Reason:
                    element not interactable
                """));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomAttribute("value"),
                Is.EqualTo("initial")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("initial")
            );
        }

        [Test]
        public void SetValue_IsRenderedInError_OnHiddenElementFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <input value='initial' style='display:none'></input>
                "
            );

            var act = () => {
                S("input").With(waitForNoOverlapFoundByJs: true).SetValue("overwritten");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.Clear().SendKeys(overwritten)
                Reason:
                    javascript error: element is not visible
                """,
                after: Configuration.Timeout
            ));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomAttribute("value"),
                Is.EqualTo("initial")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("initial")
            );
        }

        [Test]
        public void SetValue_WorksUnderOverlay_ByDefault()
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

                <input value='initial'></input>
                "
            );
            
            var act = () =>
            {
                S("input").SetValue("overwritten");
            };

            Assert.That(act, Does.PassBefore(DefaultTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("overwritten")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("overwritten")
            );
        }

        [Test]
        public void SetValue_WaitsForNoOverlay_WhenCustomized()
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

                <input value='initial'></input>
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
                S("input").With(waitForNoOverlapFoundByJs: true).SetValue("overwritten");
            };

            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("overwritten")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("overwritten")
            );
        }

        [Test]
        public void SetValue_IsRenderedInError_OnOverlappedWithOverlayFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
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


                <input value='initial'></input>
                "
            );

            var act = () => {
                S("input").With(waitForNoOverlapFoundByJs: true).SetValue("overwritten");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.Clear().SendKeys(overwritten)
                Reason:
                    Element: <input value="initial">
                    is overlapped by: <div id="overlay"
                """,
                after: Configuration.Timeout
            ));
        }
    }
}

