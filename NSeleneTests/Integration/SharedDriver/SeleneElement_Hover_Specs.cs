using NUnit.Framework;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    using System;
    using System.Linq;
    using Harness;

    [TestFixture]
    public class SeleneElement_Hover_Specs : BaseTest
    {
        // TODO: move here error messages tests, and at least some ClickByJs tests...
        
        [Test]
        public void Hover_WaitsForVisibility_OfInitiialyAbsent()
        {
            Configuration.Timeout = 1.0;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();
            var beforeCall = DateTime.Now;
            Given.OpenedPageWithBodyTimedOut(
                @"
                <span onmouseover='window.location=this.href + ""#second""'>to h2</span>
                <h2 id='second'>Heading 2</h2>
                ",
                300
            );

            S("span").Hover();

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void Hover_WaitsForVisibility_OfInitiialyHidden()
        {
            Configuration.Timeout = 1.0;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <span 
                  id='link' 
                  onmouseover='window.location=this.href + ""#second""' 
                  style='display:none'
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('link').style.display = 'block';
                ",
                300
            );

            S("span").Hover();

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
            StringAssert.Contains("second", Configuration.Driver.Url);
        }

        [Test]
        public void Hover_IsRenderedInError_OnHiddenFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <span 
                  id='link' 
                  onmouseover='window.location=this.href + ""#second""' 
                  style='display:none'
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("span").Hover();
                Assert.Fail("should fail with exception");
            }

            catch (TimeoutException error)
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                Assert.Less(afterCall, beforeCall.AddSeconds(1.0));

                Assert.That(error.Message.Trim(), Does.Contain("""
                Timed out after 0.25s, while waiting for:
                	Browser.Element(span).Actions.MoveToElement(self.ActualWebElement).Perform()
                Reason:
                	javascript error: {"status":60,"value":"[object HTMLSpanElement] has no size and location"}
                """.Trim()
                ));

                StringAssert.DoesNotContain("second", Configuration.Driver.Url);
            }
        }

        [Test]
        public void Hover_IsRenderedInError_OnHiddenFailure_WhenCustomizedToWaitForNoOverlap()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <span 
                  id='link' 
                  onmouseover='window.location=this.href + ""#second""' 
                  style='display:none'
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("span").With(waitForNoOverlapFoundByJs: true).Hover();
                Assert.Fail("should fail with exception");
            }

            catch (TimeoutException error)
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                Assert.Less(afterCall, beforeCall.AddSeconds(1.0));

                Assert.That(error.Message.Trim(), Does.Contain("""
                Timed out after 0.25s, while waiting for:
                	Browser.Element(span).Actions.MoveToElement(self.ActualNotOverlappedWebElement).Perform()
                Reason:
                	javascript error: element is not visible
                """.Trim()
                ));

                StringAssert.DoesNotContain("second", Configuration.Driver.Url);
            }
        }

        [Test]
        public void Hover_PassesWithoutEffect_UnderOverlay()
        {
            Configuration.Timeout = 1.0;
            Configuration.PollDuringWaits = 0.1;
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

                <span 
                  id='link' 
                  onmouseover='window.location=this.href + ""#second""' 
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;

            S("span").Hover();

            var afterCall = DateTime.Now;
            Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
            StringAssert.DoesNotContain("second", Configuration.Driver.Url);
        }

        [Test]
        public void Hover_Waits_For_NoOverlay_IfCustomized()
        {
            Configuration.Timeout = 1.0;
            Configuration.PollDuringWaits = 0.1;
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

                <span 
                  id='link' 
                  onmouseover='window.location=this.href + ""#second""' 
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('overlay').style.display = 'none';
                ",
                300
            );

            S("span").With(waitForNoOverlapFoundByJs: true).Hover();

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
            StringAssert.Contains("second", Configuration.Driver.Url);
        }

        [Test]
        public void Hover_IsRenderedInError_OnOverlappedWithOverlayFailure_IfCustomizedToWaitForNoOverlayByJs()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
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

                <span 
                  id='link' 
                  onmouseover='window.location=this.href + ""#second""' 
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("span").With(waitForNoOverlapFoundByJs: true).Hover();
                
                Assert.Fail("should fail with exception");
            }

            catch (TimeoutException error)
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                Assert.Less(afterCall, beforeCall.AddSeconds(1.25));
                
                StringAssert.DoesNotContain("second", Configuration.Driver.Url);

                Assert.That(error.Message.Trim(), Does.Contain("""
                Timed out after 0.25s, while waiting for:
                	Browser.Element(span).Actions.MoveToElement(self.ActualNotOverlappedWebElement).Perform()
                Reason:
                	Element: <span id="link" onmouseover="window.location=this.href + &quot;#second&quot;">to h2</span>
                	is overlapped by: <div id="overlay" style="
                """.Trim()
                ));
            }
        }
    }
}

