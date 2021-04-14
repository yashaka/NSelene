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
    public class SeleneElement_SendKeys_Specs : BaseTest
    {
        // TODO: should we cover cases when sendKeys applied to field with cursor in the middle?

        [Test]
        public void SendKeys_WaitsForVisibility_OfInitiialyAbsent()
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

            S("input").SendKeys("and after");
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
        public void SendKeys_IsRenderedInError_OnAbsentElementFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();

            try 
            {
                S("input").SendKeys("and after");
            }

            catch (TimeoutException error)
            {
                // TODO: shoud we check timing here too?
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).ActualWebElement.SendKeys(and after)", lines);
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
        public void SendKeys_PassesOnHiddenInputOfTypeFile()
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
            S("[type=file]").SendKeys(path);
            // TODO: should we add a test to check rendered error for case of non-existing file, etc.?
            
            StringAssert.Contains(
                "empty.html", 
                Configuration.Driver
                .FindElement(By.CssSelector("[type=file]")).GetAttribute("value")
            );
            StringAssert.Contains(
                "empty.html", 
                Configuration.Driver
                .FindElement(By.CssSelector("[type=file]")).GetProperty("value")
            );
        }

        [Test]
        public void SendKeys_WaitsForVisibility_OfInitialyHidden()
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

            S("input").SendKeys("and after");
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
        public void SendKeys_IsRenderedInError_OnHiddenElementFailure()
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
                S("input").SendKeys("and after");
            }

            catch (TimeoutException error)
            {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).ActualWebElement.SendKeys(and after)", lines);
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
        public void SendKeys_PassesUnder_Overlay() // TODO: should we fix it? like we do for element.Type(keys)?
        {
            Configuration.Timeout = 0.6;
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

            S("input").SendKeys("and after");
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
            Assert.Less(afterCall, beforeCall.AddSeconds(0.3));
        }

        // [Test]
        // public void SendKeys_IsRenderedInError_OnOverlappedWithOverlayFailure() // NOT RELEVANT
        // {
        //     Configuration.Timeout = 0.25;
        //     Configuration.PollDuringWaits = 0.1;
        //     Given.OpenedPageWithBody(
        //         @"
        //         <div 
        //             id='overlay' 
        //             style='
        //                 display: block;
        //                 position: fixed;
        //                 display: block;
        //                 width: 100%;
        //                 height: 100%;
        //                 top: 0;
        //                 left: 0;
        //                 right: 0;
        //                 bottom: 0;
        //                 background-color: rgba(0,0,0,0.1);
        //                 z-index: 2;
        //                 cursor: pointer;
        //             '
        //         >
        //         </div>


        //         <input value='before '></input>
        //         "
        //     );

        //     try 
        //     {
        //         S("input").SendKeys("and after");
        //     }

        //     catch (TimeoutException error)
        //     {
        //         var lines = error.Message.Split("\n").Select(
        //             item => item.Trim()
        //         ).ToList();

        //         Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
        //         Assert.Contains("Browser.Element(input).ActualNotOverlappedWebElement.SendKeys(and after)", lines);
        //         Assert.Contains("Reason:", lines);
        //         Assert.NotNull(lines.Find(item => item.Contains(
        //             "Element is overlapped by: <div id=\"overlay\" "
        //         )));
        //     }
        // }
    }
}

