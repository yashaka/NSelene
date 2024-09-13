using static NSelene.Selene;
using NUnit.Framework;
using NSelene.Support.SeleneElementJsExtensions;
using NSelene.Tests.Integration.SharedDriver.Harness;
using System;

namespace NSelene.Tests.Integration.SharedDriver
{
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
            } 
            
            catch (TimeoutException error) 
            {
                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(a).ActualWebElement.Click()
                Reason:
                    element not interactable
                """.Trim()
                ));
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
                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(a).JsClick(centerXOffset: 0, centerYOffset: 0)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"a"}
                """.Trim()
                ));
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
            } catch (TimeoutException error) {
                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(a).JsClick(centerXOffset: 0, centerYOffset: 0)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"a"}
                """.Trim()
                ));  
            }
        }
    }
}

