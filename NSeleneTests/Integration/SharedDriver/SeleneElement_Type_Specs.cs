namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class SeleneElement_Type_Specs : BaseTest
    {
        // TODO: should we cover cases when Type applied to field with cursor in the middle?

        // TODO: move here some TypeByJs tests

        [Test]
        public void Type_WaitsForVisibility_OfInitialyAbsent()
        {
            Given.OpenedEmptyPage();
            Given.OpenedPageWithBodyTimedOut(
                @"
                <input value='before '></input>
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                S("input").Type("and after");
            };
            
            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("before and after")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("before and after")
            );
        }
        
        [Test]
        public void Type_IsRenderedInError_OnAbsentElementFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedEmptyPage();

            var act = () => {
                S("input").Type("and after");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.SendKeys(and after)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"input"}
                """));
        }

        [Test]
        [Ignore("Overlay does not fail <file input>.Type()")]

        public void Type_FailsOnHiddenInputOfTypeFile()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <input type='file' style='display:none'></input>
                "
            );

            var act = () =>
            {
                S("[type=file]").Type(EmptyHtmlPath);
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element([type=file]).ActualNotOverlappedWebElement.SendKeys({path})
                Reason:
                    javascript error: element is not visible
                """));
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
        public void Type_WaitsForVisibility_OfInitialyHidden()
        {
            Given.OpenedPageWithBody(
                @"
                <input value='before ' style='display:none'></input>
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
                S("input").Type("and after");
            };
            
            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("before and after")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("before and after")
            );
        }
        
        [Test]
        public void Type_IsRenderedInError_OnHiddenElementFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <input value='before ' style='display:none'></input>
                "
            );

            var act = () => {
                S("input").Type("and after");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.SendKeys(and after)
                Reason:
                    element not interactable
                """));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("before ")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("before ")
            );
        }
        [Test]
        public void Type_IsRenderedInError_OnHiddenElementFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <input value='before ' style='display:none'></input>
                "
            );

            var act = () => {
                S("input").With(waitForNoOverlapFoundByJs: true).Type("and after");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.SendKeys(and after)
                Reason:
                    javascript error: element is not visible
                """));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("before ")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("before ")
            );
        }

        [Test]
        public void Type_WorksUnderOverlay_ByDefault()
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

                <input value='before '></input>
                "
            );

            var act = () =>
            {
                S("input").Type("and after");
            };

            Assert.That(act, Does.PassBefore(DefaultTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("before and after")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("before and after")
            );
        }

        [Test]
        public void Type_WaitsForNoOverlay_IfExplicitelyCustomized()
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

                <input value='before '></input>
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
                S("input").With(waitForNoOverlapFoundByJs: true).Type("and after");
            };

            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("before and after")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("before and after")
            );
        }

        [Test]
        public void Type_IsRenderedInError_OnOverlappedWithOverlayFailure()
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

                <input value='before '></input>
                "
            );

            var act = () => {
                S("input").With(waitForNoOverlapFoundByJs: true).Type("and after");
            };


            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.SendKeys(and after)
                Reason:
                    Element: <input value="before ">
                    is overlapped by: <div id="overlay"
                """));
        }
    }
}

