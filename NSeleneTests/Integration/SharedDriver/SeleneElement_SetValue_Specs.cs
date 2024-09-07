using NUnit.Framework;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Harness;
    using OpenQA.Selenium;

    [TestFixture]
    public class SeleneElement_SetValue_Specs : BaseTest
    {
        // TODO: move here some SetValueByJs tests

        [Test]
        public void SetValue_WaitsForVisibility_OfInitiialyAbsent()
        {
            Configuration.Timeout = 0.6; 
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();
            var beforeCall = DateTime.Now;
            Given.OpenedPageWithBodyTimedOut(
                @"
                <input value='initial'></input>
                ",
                300
            );

            S("input").SetValue("overwritten");
            var afterCall = DateTime.Now;

            Assert.AreEqual(
                "overwritten", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetAttribute("value")
            );
            Assert.AreEqual(
                "overwritten", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetDomProperty("value")
            );
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
        }
        
        [Test]
        public void SetValue_IsRenderedInError_OnAbsentElementFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();

            try 
            {
                S("input").SetValue("overwritten");
            }

            catch (TimeoutException error)
            {
                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(input).ActualWebElement.Clear().SendKeys(overwritten)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"input"}
                """.Trim()
                ));
            }
        }

        [Test]
        public void SetValue_IsRenderedInError_OnAbsentElementFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();

            try 
            {
                S("input").With(waitForNoOverlapFoundByJs: true).SetValue("overwritten");
            }

            catch (TimeoutException error)
            {
                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(input).ActualNotOverlappedWebElement.Clear().SendKeys(overwritten)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"input"}
                """.Trim()
                ));
            }
        }
        
        [Test]
        public void SetValue_FailsOnHiddenInputOfTypeFile() // TODO: should we allow it here like in send keys? kind of sounds natural... but can we do that without drawing performance?
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <input type='file' style='display:none'></input>
                "
            );

            var path = new Uri(
                new Uri(Assembly.GetExecutingAssembly().Location), 
                "../../../Resources/empty.html" // TODO: use ./empty.html (tune csproj correspondingly)
            ).AbsolutePath;
            
            try 
            {
                S("[type=file]").SetValue(path);
            }

            catch (TimeoutException error)
            {
                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element([type=file]).ActualWebElement.Clear().SendKeys({{path}})
                Reason:
                    element not interactable
                """.Trim()
                ));

                Assert.AreEqual(
                    "", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetAttribute("value")
                );
                Assert.AreEqual(
                    "", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetDomProperty("value")
                );
            }
        }
        
        [Test]
        public void SetValue_FailsOnHiddenInputOfTypeFile_WhenCustomizedToWaitForNoOverlapFoundByJs() // TODO: should we allow it here like in send keys? kind of sounds natural... but can we do that without drawing performance?
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <input type='file' style='display:none'></input>
                "
            );

            var path = new Uri(
                new Uri(Assembly.GetExecutingAssembly().Location), 
                "../../../Resources/empty.html" // TODO: use ./empty.html (tune csproj correspondingly)
            ).AbsolutePath;
            
            try 
            {
                S("[type=file]").With(waitForNoOverlapFoundByJs: true).SetValue(path);
            }

            catch (TimeoutException error)
            {
                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element([type=file]).ActualNotOverlappedWebElement.Clear().SendKeys({{path}})
                Reason:
                    javascript error: element is not visible
                """.Trim()
                ));

                Assert.AreEqual(
                    "", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetAttribute("value")
                );
                Assert.AreEqual(
                    "", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetDomProperty("value")
                );
            }
        }

        [Test]
        public void SetValue_WaitsForVisibility_OfInitialyHidden()
        {
            Configuration.Timeout = 0.6;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <input value='initial' style='display:none'></input>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementsByTagName('input')[0].style.display = 'block';
                ",
                300
            );

            S("input").SetValue("overwritten");
            var afterCall = DateTime.Now;

            Assert.AreEqual(
                "overwritten", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetAttribute("value")
            );
            Assert.AreEqual(
                "overwritten", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetDomProperty("value")
            );
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
        }
        
        [Test]
        public void SetValue_IsRenderedInError_OnHiddenElementFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <input value='initial' style='display:none'></input>
                "
            );

            try 
            {
                S("input").SetValue("overwritten");
            }

            catch (TimeoutException error)
            {
                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(input).ActualWebElement.Clear().SendKeys(overwritten)
                Reason:
                    element not interactable
                """.Trim()
                ));

                Assert.AreEqual(
                    "initial", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetAttribute("value")
                );
                Assert.AreEqual(
                    "initial", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetDomProperty("value")
                );
            }
        }

        [Test]
        public void SetValue_IsRenderedInError_OnHiddenElementFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <input value='initial' style='display:none'></input>
                "
            );

            try 
            {
                S("input").With(waitForNoOverlapFoundByJs: true).SetValue("overwritten");
            }

            catch (TimeoutException error)
            {
                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(input).ActualNotOverlappedWebElement.Clear().SendKeys(overwritten)
                Reason:
                    javascript error: element is not visible
                """.Trim()
                ));

                Assert.AreEqual(
                    "initial", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetAttribute("value")
                );
                Assert.AreEqual(
                    "initial", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetDomProperty("value")
                );
            }
        }

        [Test]
        public void SetValue_WorksUnderOverlay_ByDefault()
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

                <input value='initial'></input>
                "
            );
            var beforeCall = DateTime.Now;

            S("input").SetValue("overwritten");

            var afterCall = DateTime.Now;
            Assert.Less(afterCall, beforeCall.AddSeconds(0.5));
            Assert.AreEqual(
                "overwritten", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetAttribute("value")
            );
            Assert.AreEqual(
                "overwritten", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetDomProperty("value")
            );
        }

        [Test]
        public void SetValue_WaitsForNoOverlay_WhenCustomized()
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

                <input value='initial'></input>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('overlay').style.display = 'none';
                ",
                300
            );

            S("input").With(waitForNoOverlapFoundByJs: true).SetValue("overwritten");

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
            Assert.AreEqual(
                "overwritten", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetAttribute("value")
            );
            Assert.AreEqual(
                "overwritten", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetDomProperty("value")
            );
        }

        [Test]
        public void SetValue_IsRenderedInError_OnOverlappedWithOverlayFailure_WhenCustomizedToWaitForNoOverlapFoundByJs()
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


                <input value='initial'></input>
                "
            );

            try 
            {
                S("input").With(waitForNoOverlapFoundByJs: true).SetValue("overwritten");
            }

            catch (TimeoutException error)
            {
                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(input).ActualNotOverlappedWebElement.Clear().SendKeys(overwritten)
                Reason:
                    Element: <input value="initial">
                    is overlapped by: <div id="overlay"
                """.Trim()
                ));
            }
        }
    }
}

