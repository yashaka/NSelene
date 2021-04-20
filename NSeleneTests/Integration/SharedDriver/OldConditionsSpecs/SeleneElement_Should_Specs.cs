using NUnit.Framework;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Harness;
    using OpenQA.Selenium;

    [TestFixture]
    public class SeleneElement_Should_OldStyleConditions_Specs : BaseTest
    {
        // TODO: imrove coverage and consider breaking down into separate test classes

        [Test]
        public void Should_HaveValue_WaitsForPresenceInDom_OfInitiialyAbsent()
        {
            Configuration.Timeout = 0.6; 
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();
            var beforeCall = DateTime.Now;
            Given.OpenedPageWithBodyTimedOut(
                @"
                <input value='initial' style='display:none'></input>
                ",
                300
            );

            S("input").Should(Have.Value("initial"));
            var afterCall = DateTime.Now;

            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
        }

        [Test]
        public void Should_HaveNoValue_WaitsForAbsenceInDom_OfInitiialyPresent()
        {
            Configuration.Timeout = 0.6; 
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <input value='initial' style='display:none'></input>
                "
            );
            var beforeCall = DateTime.Now;
            Given.WithBodyTimedOut(
                @"
                ",
                300
            );

            S("input").Should(Have.No.Value("initial"));

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
            try
            {
                Configuration.Driver
                .FindElement(By.TagName("input")).GetAttribute("value");
                Assert.Fail("should fail because the element should be already absent");
            }
            catch (WebDriverException error)
            {
                StringAssert.Contains(
                    "no such element: Unable to locate element: ", error.Message
                );
            }
        }
        
        [Test]
        public void Should_HaveValue_IsRenderedInError_OnAbsentElementTimeoutFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();
            var beforeCall = DateTime.Now;

            try 
            {
                S("input").Should(Have.Value("initial"));
            }

            catch (TimeoutException error)
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                var accuracyDelta = 0.1;
                Assert.Less(afterCall, beforeCall.AddSeconds(0.25 + 0.1 + accuracyDelta));

                // TODO: shoud we check timing here too?
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).value='initial'", lines);
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
        public void Should_HaveNoValue_IsRenderedInError_OnInDomElementTimeoutFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <input value='initial' style='display:none'></input>
                "
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("input").Should(Have.No.Value("initial"));
            }

            catch (TimeoutException error)
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                var accuracyDelta = 0.1;
                Assert.Less(afterCall, beforeCall.AddSeconds(0.25 + 0.1 + accuracyDelta));

                // TODO: shoud we check timing here too?
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(input).not value='initial'", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("actual: value='initial'", lines);
            }
        }
        
        [Test]
        public void Should_HaveText_WaitsForVisibility_OfInitialyHidden()
        {
            Configuration.Timeout = 0.6;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <label style='display:none'>initial</label>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementsByTagName('label')[0].style.display = 'block';
                ",
                300
            );

            S("label").Should(Have.Text("initial"));

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
            Assert.AreEqual(
                "initial", 
                Configuration.Driver
                .FindElement(By.TagName("label")).Text
            );
        }

        [Test]
        public void Should_HaveText_WaitsForAskedText_OfInitialyOtherText()
        {
            Configuration.Timeout = 0.6;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <label>initial</label>
                "
            );
            var beforeCall = DateTime.Now;
            Given.OpenedPageWithBodyTimedOut(
                @"
                <label>new</label>
                "
                ,
                300
            );

            S("label").Should(Have.Text("new"));

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
            Assert.AreEqual(
                "new", 
                Configuration.Driver
                .FindElement(By.TagName("label")).Text
            );
            Assert.AreEqual(
                "new", 
                Configuration.Driver
                .FindElement(By.TagName("label")).Text
            );
        }
        
        [Test]
        public void Should_HaveText_IsRenderedInError_OnHiddenElementFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <label style='display:none'>initial</label>
                "
            );

            try 
            {
                S("label").Should(Have.Text("initial"));
            }

            catch (TimeoutException error)
            {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(label).contains initial", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("actual:", lines);

                Assert.AreEqual(
                    "",  Configuration.Driver.FindElement(By.TagName("label")).Text
                );
            }
        }

        [Test]
        public void Should_BeVisible_WaitsForVisibility_OfInitiialyHiddenInDom()
        {
            Configuration.Timeout = 0.6; 
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <label style='display:none'>initial</label>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementsByTagName('label')[0].style.display = 'block';
                ",
                300
            );

            S("label").Should(Be.Visible);

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
        }

        [Test]
        public void Should_BeNotVisible_WaitsForHiddenInDom_FromInitiialyVisible()
        {
            Configuration.Timeout = 0.6; 
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <label style='display:block'>initial</label>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementsByTagName('label')[0].style.display = 'none';
                ",
                300
            );

            S("label").Should(Be.Not.Visible);

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
        }
        
        [Test]
        public void Should_BeVisible_WaitsForVisibility_OfInitiialyAbsentInDom()
        {
            Configuration.Timeout = 0.6; 
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();
            var beforeCall = DateTime.Now;
            Given.WithBodyTimedOut(
                @"
                <label>initial</label>
                "
                ,
                300
            );

            S("label").Should(Be.Visible);

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
        }

        [Test]
        public void Should_BeNotVisible_WaitsForAbsentInDom_FromInitiallyVisibile()
        {
            Configuration.Timeout = 0.6; 
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <label>initial</label>
                "
            );
            var beforeCall = DateTime.Now;
            Given.WithBodyTimedOut(
                @"
                "
                ,
                300
            );

            S("label").Should(Be.Not.Visible);

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
        }

        [Test]
        public void Should_BeVisible_IsRenderedInError_OnHiddenInDomElementFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <label style='display:none'>initial</label>
                "
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("label").Should(Be.Visible);
            }

            catch (TimeoutException error)
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                var accuracyDelta = 0.1;
                Assert.Less(afterCall, beforeCall.AddSeconds(0.25 + 0.1 + accuracyDelta));

                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(label).Visible", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("actual: False", lines);

                Assert.AreEqual(
                    false,  Configuration.Driver.FindElement(By.TagName("label")).Displayed
                );
            }
        }

        [Test]
        public void Should_BeNotVisible_IsRenderedInError_OnVisibleElementFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <label>initial</label>
                "
            );

            try 
            {
                S("label").Should(Be.Not.Visible);
            }

            catch (TimeoutException error)
            {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(label).not Visible", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("actual: False", lines);

                Assert.AreEqual(
                    true,  Configuration.Driver.FindElement(By.TagName("label")).Displayed
                );
            }
        }

        [Test]
        public void Should_BeVisible_IsRenderedInError_OnAbsentInDomElementFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();
            var beforeCall = DateTime.Now;

            try 
            {
                S("label").Should(Be.Visible);
            }

            catch (TimeoutException error)
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                var accuracyDelta = 0.1;
                Assert.Less(afterCall, beforeCall.AddSeconds(0.25 + 0.1 + accuracyDelta));

                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(label).Visible", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains(
                    "no such element: Unable to locate element: "
                    + "{\"method\":\"css selector\",\"selector\":\"label\"}"
                    , 
                    lines
                );
            }
        }

        [Test]
        public void Should_HaveText_IsRenderedInError_OnElementDifferentTextFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <label>initial</label>
                "
            );

            try 
            {
                S("label").Should(Have.Text("new"));
            }

            catch (TimeoutException error)
            {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();

                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(label).contains new", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("actual: initial", lines);

                Assert.AreEqual(
                    "initial",  Configuration.Driver.FindElement(By.TagName("label")).Text
                );
            }
        }

        [Test]
        public void Should_HaveText_DoesNotWaitForNoOverlay() // TODO: but should it?
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

                <label>initial</label>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('overlay').style.display = 'none';
                ",
                300
            );

            S("label").Should(Have.Text("initial"));

            var afterCall = DateTime.Now;
            Assert.Less(afterCall, beforeCall.AddSeconds(0.3));
            Assert.AreEqual(
                "initial", 
                Configuration.Driver
                .FindElement(By.TagName("label")).Text
            );
            // Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            // Assert.Less(afterCall, beforeCall.AddSeconds(0.6));
        }

        // [Test]
        // public void Should_HaveText_IsRenderedInError_OnOverlappedWithOverlayFailure() // IS NOT RELEVANT
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


        //         <label>initial</label>
        //         "
        //     );

        //     try 
        //     {
        //         S("label").Should(Have.Text("initial"));
        //     }

        //     catch (TimeoutException error)
        //     {
        //         var lines = error.Message.Split("\n").Select(
        //             item => item.Trim()
        //         ).ToList();

        //         Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
        //         Assert.Contains("Browser.Element(label).contains initial", lines);
        //         Assert.Contains("Reason:", lines);
        //         Assert.NotNull(lines.Find(item => item.Contains(
        //             "Element is overlapped by: <div id=\"overlay\" "
        //         )));
        //     }
        // }
    }
}

