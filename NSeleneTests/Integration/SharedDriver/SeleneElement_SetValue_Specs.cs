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
                .FindElement(By.TagName("input")).GetProperty("value")
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
                // TODO: shoud we check timing here too?
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).ActualNotOverlappedWebElement.Clear().SendKeys(overwritten)", lines);
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
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains($"Browser.Element([type=file]).ActualNotOverlappedWebElement.Clear().SendKeys({path})", lines);
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
                .FindElement(By.TagName("input")).GetProperty("value")
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
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).ActualNotOverlappedWebElement.Clear().SendKeys(overwritten)", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("javascript error: element is not visible", lines);

                Assert.AreEqual(
                    "initial", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetAttribute("value")
                );
                Assert.AreEqual(
                    "initial", 
                    Configuration.Driver
                    .FindElement(By.TagName("input")).GetProperty("value")
                );
            }
        }

        [Test]
        public void SetValue_WaitsForNoOverlay()
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
                .FindElement(By.TagName("input")).GetProperty("value")
            );
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
        }

        [Test]
        public void SetValue_IsRenderedInError_OnOverlappedWithOverlayFailure()
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
                S("input").SetValue("overwritten");
            }

            catch (TimeoutException error)
            {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).ActualNotOverlappedWebElement.Clear().SendKeys(overwritten)", lines);
                Assert.Contains("Reason:", lines);
                Assert.NotNull(lines.Find(item => item.Contains(
                    "Element is overlapped by: <div id=\"overlay\" "
                )));
            }
        }
    }
}

