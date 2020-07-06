using NUnit.Framework;
using static NSelene.Selene;
using NSelene.Tests.Integration.SharedDriver.Harness;
using NSelene.Conditions;
using NSelene.Support.Extensions;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver
{
    [TestFixture]
    public class SeleneElementJSActionsSpecs : BaseTest
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
        public void SElementJsScrollIntoView()
        {
            Given.OpenedPageWithBody("<input style='margin-top:100cm;' type='text' value='ku ku'/>");
            SeleneElement element = S("input");
            new SeleneDriver(Selene.GetWebDriver()).Should(Have.No.JSReturnedTrue(ELEMENT_IN_VIEEW, element.ActualWebElement));

            element.JsScrollIntoView();

            new SeleneDriver(Selene.GetWebDriver()).Should(Have.JSReturnedTrue(ELEMENT_IN_VIEEW, element.ActualWebElement));
        }
    }
}
