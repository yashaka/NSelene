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
        [Test]
        public void SElementJSScrollIntoView()
        {
            // prepare
            Given.OpenedPageWithBody("<input style='margin-top:100cm;' type='text' value='ku ku'/>");
            SeleneElement ele = S("input");
            new SeleneDriver(Selene.GetWebDriver()).ShouldNot(BeInView(ele));

            // act
            ele.JSScrollIntoView();

            // assert
            new SeleneDriver(Selene.GetWebDriver()).Should(BeInView(ele));
        }

        private static Condition<IWebDriver> BeInView(SeleneElement ele)
        {
            return Have.JSReturnedTrue(@"
                var windowHeight = window.innerHeight;
                var height = document.documentElement.clientHeight;
                var r = arguments[0].getClientRects()[0]; 

                return r.top > 0
                    ? r.top <= windowHeight 
                    : (r.bottom > 0 && r.bottom <= windowHeight);
                ", ele.ActualWebElement);
        }

        [Test]
        public void SElementJSClickWithOffset()
        {
            // prepare
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            SeleneElement ele = S("input");
            ele.Click();

            // act
            ele.JSClickWithOffset(-10, -10);

            // assert
        }

    }
}
