using NSelene;
using static NSelene.Selene;
using NUnit.Framework;
using OpenQA.Selenium;
using NSelene.Support.SeleneElementJsExtensions;

namespace NSelene.Tests.Integration.SharedDriver
{
    using System;
    using System.Linq;
    using Harness;

    [TestFixture]
    public class ErrorMessages_Specs : BaseTest
    {
        // todo: make it "condition tests"

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }

        [Test]
        public void SeleneElement_ShouldNot_FailsWhenAssertingNotVisibleForVisibleElement()
        {
            Configuration.Timeout = 0.2;
            Given.OpenedPageWithBody("<input id='new-text' type='text' value='ku ku'/>");
            Assert.Throws(Is.TypeOf(typeof(WebDriverTimeoutException))
                          .And.Message.Contains($"for condition: not {Be.Visible.GetType().Name}")
                          .And.Message.Contains("Actual: False"), () => {
                S("#new-text").Should(Be.Not.Visible);
            });
        }

        [Test]
        public void SeleneElement_Should_FailsWhenAssertingVisibleForHiddenElement()
        {
            Configuration.Timeout = 0.2;
            Given.OpenedPageWithBody("<input id='new-text' type='text' value='ku ku' style='display:none'/>");
            Assert.Throws(Is.TypeOf(typeof(WebDriverTimeoutException))
                          .And.Message.Not.Contains("not " + Be.Visible.GetType().Name)
                          .And.Message.Contains($": {Be.Visible.GetType().Name}"), () => {
                S("#new-text").Should(Be.Visible);
            });
        }

        [Test]
        public void SeleneElement_Should_FailsWhenAssertingWrongValueForElement()
        {
            Configuration.Timeout = 0.2;
            Given.OpenedPageWithBody("<input id='new-text' type='text' value='ku ku' style='display:none'/>");
            Assert.Throws(Is.TypeOf(typeof(WebDriverTimeoutException))
                          .And.Message.Contains("for condition: value='ka ku'")
                          .And.Message.Contains("Actual: value='ku ku'"), () => {
                S("#new-text").Should(Have.Value("ka ku"));
            });
        }

        [Test]
        public void SeleneElement_Should_FailsWhenAssertingNoValueForElementWithSuchValue()
        {
            Configuration.Timeout = 0.2;
            Given.OpenedPageWithBody("<input id='new-text' type='text' value='ku ku' style='display:none'/>");
            Assert.Throws(Is.TypeOf(typeof(WebDriverTimeoutException))
                          .And.Message.Contains("for condition: not value='ku ku'")
                          .And.Message.Contains("Actual: value='ku ku'"), () => {
                S("#new-text").Should(Have.No.Value("ku ku"));
            });
        }

        [Test]
        public void SeleneElement_SetValue_FailsForHiddenElement()
        {
            Configuration.Timeout = 0.2;
            Given.OpenedPageWithBody("<input id='new-text' type='text' value='ku ku' style='display:none'/>");
            Assert.Throws(Is.TypeOf(typeof(WebDriverTimeoutException))
                          .And.Message.Not.Contains("not " + Be.Visible.GetType().Name)
                          .And.Message.Contains(Be.Visible.GetType().Name), () => {
                S("#new-text").SetValue("will not happen");
            });
        }

        [Test]
        public void SeleneElement_Click_Failure_OnHiddenElement()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>"
            );

            // TODO: consider using Assert.Throws<WebDriverTimeoutException>(() => { ... })
            try {
                S("a").Click();
                Assert.Fail("should fail on timeout before can be clicked");
            } catch (TimeoutException error) {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();
                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                // TODO: is it ok to have here .ActualWebElement
                //       and don't have it below in JsClick case?
                Assert.Contains("Browser.Element(a).ActualWebElement.Click()", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains("element not interactable", lines);
            }
        }

        [Test]
        public void SeleneElement_CustomizedToJsClick_Click_Failure_OnAbsentInDomElement()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <h2 id='second'>Heading 2</h2>
            ");

            // TODO: consider using Assert.Throws<WebDriverTimeoutException>(() => { ... })
            try {
                S("a").With(clickByJs: true).Click();
                Assert.Fail("should fail on timeout before can be clicked");
            } catch (TimeoutException error) {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();
                Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                Assert.Contains("Browser.Element(a).JsClick(0, 0)", lines);
                Assert.Contains("Reason:", lines);
                Assert.Contains(
                    "no such element: Unable to locate element: "
                    + "{\"method\":\"css selector\",\"selector\":\"a\"}", lines);
            }
        }

        /// TODO: a raw JsClick test and see what will come... it will fail without waiting
        ///       is this what we want?
        ///       should we make JsClick extension impl at least an alias to .With(clickByJs: true).Click()
        ///       at least then it would be more consistent
        ///       but will somebody want one day run js commands without any waitings?
        ///       should we wrap wait inside ExecuteScript ?
        ///       should we make this configurable?
        [Test]
        public void SeleneElement_JsClick_Failure_OnAbsentInDomElement()
        {
            Configuration.Timeout = 0.25;
            Given.OpenedPageWithBody(@"
                <h2 id='second'>Heading 2</h2>
            ");

            // TODO: consider using Assert.Throws<WebDriverTimeoutException>(() => { ... })
            try {
                S("a").JsClick();
                Assert.Fail("should fail before can be js-clicked");
            } catch (NoSuchElementException error) {
                var lines = error.Message.Split("\n").Select(
                    item => item.Trim()
                ).ToList();
                // Assert.Contains("Timed out after 0.25s, while waiting for:", lines);
                // Assert.Contains("Browser.Element(a).JsClick(0, 0)", lines);
                // Assert.Contains("Reason:", lines);
                Assert.Contains(
                    "no such element: Unable to locate element: "
                    + "{\"method\":\"css selector\",\"selector\":\"a\"}", lines);
            }
        }
    }
}

