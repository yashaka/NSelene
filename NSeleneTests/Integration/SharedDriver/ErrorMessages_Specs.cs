using NSelene.Support.SeleneElementJsExtensions;
using NSelene.Tests.Integration.SharedDriver.Harness;
using System;

namespace NSelene.Tests.Integration.SharedDriver
{
    [TestFixture]
    public class ErrorMessages_Specs : BaseTest
    {
        // todo: make it "condition tests"

        [Test]
        public void SeleneElement_Click_Failure_OnHiddenElement()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(@"
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>"
            );

            var act = () => {
                S("a").Click();
            };
            
            Assert.That(act, Does.Timeout($$"""
                Browser.Element(a).ActualWebElement.Click()
                Reason:
                    element not interactable
                """));
        }

        [Test]
        public void SeleneElement_CustomizedToJsClick_Click_Failure_OnAbsentInDomElement()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(@"
                <h2 id='second'>Heading 2</h2>
            ");

            var act = () => {
                S("a").With(clickByJs: true).Click();
            };
            
            Assert.That(act, Does.Timeout($$"""
                Browser.Element(a).JsClick(centerXOffset: 0, centerYOffset: 0)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"a"}
                """));
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
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(@"
                <h2 id='second'>Heading 2</h2>
            ");

            var act = () =>
            {
                S("a").JsClick();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(a).JsClick(centerXOffset: 0, centerYOffset: 0)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"a"}
                """));  
        }
    }
}

