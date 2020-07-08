using NUnit.Framework;
using static NSelene.Selene;
using NSelene.Tests.Integration.SharedDriver.Harness;
using NSelene.Conditions;
using NSelene.Support.Extensions;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver
{
    [TestFixture]
    public class SeleneElementJsExtensionsSpecs : BaseTest
    {
        private const string ELEMENT_IN_VIEEW = @"
                var windowHeight = window.innerHeight;
                var height = document.documentElement.clientHeight;
                var r = arguments[0].getClientRects()[0]; 

                return r.top > 0
                    ? r.top <= windowHeight 
                    : (r.bottom > 0 && r.bottom <= windowHeight);
                ";

        [Test]
        public void JsScrollIntoView()
        {
            Given.OpenedPageWithBody("<input style='margin-top:100cm;' type='text' value='ku ku'/>");
            SeleneElement element = S("input");
            new SeleneDriver(Selene.GetWebDriver()).Should(Have.No.JSReturnedTrue(ELEMENT_IN_VIEEW, element.ActualWebElement));

            element.JsScrollIntoView();

            new SeleneDriver(Selene.GetWebDriver()).Should(Have.JSReturnedTrue(ELEMENT_IN_VIEEW, element.ActualWebElement));
        }

        [Test]
        public void JsClick_ClicksOnHiddenElement()
        {
            Given.OpenedPageWithBody(@"
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>"
            );

            //S("a").JsClick();
            
            Assert.IsTrue(Selene.GetWebDriver().Url.Contains("second"));
        }

        // TODO: make it work and pass:)
        // [Test]
        // public void JsClick_ClicksWithOffsetFromCenter()
        // {
        //     Given.OpenedPageWithBody(@"
        //         <a id='to-first' href='#first'>go to Heading 1</a>
        //         </br>
        //         <a id='to-second' href='#second'>go to Heading 2</a>
        //         <h2 id='second'>Heading 1</h2>
        //         <h2 id='second'>Heading 2</h2>"
        //     );

        //     var toSecond = S("#to-second");
            
        //     toSecond.JsClick(0, -toSecond.Size.Height);
            
        //     Assert.IsTrue(Selene.GetWebDriver().Url.Contains("first"));
        // }
    }
}
