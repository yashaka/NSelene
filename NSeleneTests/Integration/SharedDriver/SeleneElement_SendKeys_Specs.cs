namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class SeleneElement_SendKeys_Specs : BaseTest
    {
        // TODO: should we cover cases when sendKeys applied to field with cursor in the middle?

        [Test]
        public void SendKeys_WaitsForVisibility_OfInitialyAbsent()
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
                S("input").SendKeys("and after");
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
        public void SendKeys_IsRenderedInError_OnAbsentElementFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedEmptyPage();

            var act = () => {
                S("input").SendKeys("and after");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualWebElement.SendKeys(and after)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"input"}
                """));
        }
        
        [Test]
        public void SendKeys_PassesOnHiddenInputOfTypeFile()
        {
            Given.OpenedPageWithBody(
                @"
                <input type='file' style='display:none'></input>
                "
            );

            S("[type=file]").SendKeys(EmptyHtmlPath);
            // TODO: should we add a test to check rendered error for case of non-existing file, etc.?

            Assert.That(
                Configuration.Driver.FindElement(By.CssSelector("[type=file]")).GetAttribute("value"),
                Does.Contain("empty.html")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.CssSelector("[type=file]")).GetDomProperty("value"),
                Does.Contain("empty.html")
            );
        }

        [Test]
        public void SendKeys_WaitsForVisibility_OfInitialyHidden()
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
                S("input").SendKeys("and after");
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
        public void SendKeys_IsRenderedInError_OnHiddenElementFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <input value='before ' style='display:none'></input>
                "
            );

            var act = () => {
                S("input").SendKeys("and after");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualWebElement.SendKeys(and after)
                Reason:
                    element not interactable
                """));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value"),
                Is.EqualTo("before "));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("input")).GetDomProperty("value"),
                Is.EqualTo("before "));
        }

        [Test]
        [Ignore("Overlay does not fail SendKeys()")]

        public void SendKeys_PassesUnder_Overlay() // TODO: should we fix it? like we do for element.Type(keys)?
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
                S("input").SendKeys("and after");
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
        [Ignore("Overlay does not fail SendKeys()")]

        public void SendKeys_IsRenderedInError_OnOverlappedWithOverlayFailure() // NOT RELEVANT
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

            var act = () =>
            {
                S("input").SendKeys("and after");
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).ActualNotOverlappedWebElement.SendKeys(and after)
                Reason:
                    Element is overlapped by: <div id="overlay"
                """));
        }
    }
}

