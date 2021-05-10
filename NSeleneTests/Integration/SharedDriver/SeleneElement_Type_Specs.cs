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
    public class SeleneElement_Type_Specs : BaseTest
    {
        // TODO: should we cover cases when Type applied to field with cursor in the middle?

        // TODO: move here some TypeByJs tests

        [Test]
        public void Type_WaitsForVisibility_OfInitiialyAbsent()
        {
            Configuration.Timeout = 0.7; // bigger than for other actions, because we simulate typing all keys...
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();
            var beforeCall = DateTime.Now;
            Given.OpenedPageWithBodyTimedOut(
                @"
                <input value='before '></input>
                ",
                300
            );

            S("input").Type("and after");
            var afterCall = DateTime.Now;

            Assert.AreEqual(
                "before and after", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetAttribute("value")
            );
            Assert.AreEqual(
                "before and after", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetProperty("value")
            );
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.7));
        }
        
        [Test]
        public void Type_IsRenderedInError_OnAbsentElementFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();

            try 
            {
                S("input").Type("and after");
            }

            catch (TimeoutException error)
            {
                // TODO: shoud we check timing here too?
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).ActualNotOverlappedWebElement.SendKeys(and after)", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains(
                    "no such element: Unable to locate element: "
                    + "{\"method\":\"css selector\",\"selector\":\"input\"}"
                    , 
                    lines
                );
            }
        }
        
        [Test]
        public void Type_FailsOnHiddenInputOfTypeFile()
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
                S("[type=file]").Type(path);
            }

            catch (TimeoutException error)
            {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains($"Browser.Element([type=file]).ActualNotOverlappedWebElement.SendKeys({path})", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("javascript error: element is not visible", lines);

                Assert.AreEqual(
                    "", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetAttribute("value")
                );
                Assert.AreEqual(
                    "", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetProperty("value")
                );
            }
        }

        [Test]
        public void Type_WaitsForVisibility_OfInitialyHidden()
        {
            Configuration.Timeout = 0.7;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <input value='before ' style='display:none'></input>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementsByTagName('input')[0].style.display = 'block';
                ",
                300
            );

            S("input").Type("and after");
            var afterCall = DateTime.Now;

            Assert.AreEqual(
                "before and after", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetAttribute("value")
            );
            Assert.AreEqual(
                "before and after", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetProperty("value")
            );
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.7));
        }
        
        [Test]
        public void Type_IsRenderedInError_OnHiddenElementFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <input value='before ' style='display:none'></input>
                "
            );

            try 
            {
                S("input").Type("and after");
            }

            catch (TimeoutException error)
            {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).ActualNotOverlappedWebElement.SendKeys(and after)", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("element not interactable", lines);

                Assert.AreEqual(
                    "before ", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetAttribute("value")
                );
                Assert.AreEqual(
                    "before ", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetProperty("value")
                );
            }
        }
        [Test]
        public void Type_IsRenderedInError_OnHiddenElementFailure_WhenCustomizedToWaitByJsForNotOverlapped()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <input value='before ' style='display:none'></input>
                "
            );

            try 
            {
                S("input").With(waitByJsForNotOverlapped: true).Type("and after");
            }

            catch (TimeoutException error)
            {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).ActualNotOverlappedWebElement.SendKeys(and after)", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("javascript error: element is not visible", lines);

                Assert.AreEqual(
                    "before ", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetAttribute("value")
                );
                Assert.AreEqual(
                    "before ", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetProperty("value")
                );
            }
        }

        [Test]
        public void Type_WorksUnderOverlay_ByDefault()
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

                <input value='before '></input>
                "
            );
            var beforeCall = DateTime.Now;

            S("input").Type("and after");

            var afterCall = DateTime.Now;
            Assert.Less(afterCall, beforeCall.AddSeconds(0.5));
            Assert.AreEqual(
                "before and after", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetAttribute("value")
            );
            Assert.AreEqual(
                "before and after", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetProperty("value")
            );
        }

        [Test]
        public void Type_WaitsForNoOverlay_IfExplicitelyCustomized()
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

                <input value='before '></input>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('overlay').style.display = 'none';
                ",
                300
            );

            S("input").With(waitByJsForNotOverlapped: true).Type("and after");

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
            Assert.AreEqual(
                "before and after", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetAttribute("value")
            );
            Assert.AreEqual(
                "before and after", 
                Configuration.Driver
                .FindElement(By.TagName("input")).GetProperty("value")
            );
        }

        [Test]
        public void Type_IsRenderedInError_OnOverlappedWithOverlayFailure()
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

                <input value='before '></input>
                "
            );

            try 
            {
                S("input").With(waitByJsForNotOverlapped: true).Type("and after");
            }

            catch (TimeoutException error)
            {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).ActualNotOverlappedWebElement.SendKeys(and after)", lines);
                Assert.Contains("Reason:", lines);
                Assert.NotNull(lines.Find(item => item.Contains(
                    "Element is overlapped by: <div id=\"overlay\" "
                )));
            }
        }
    }
}

