using NUnit.Framework;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver
{
    using Harness;
    using NSelene;
    using NSelene.Conditions;
    using OpenQA.Selenium;
    using System;

    [TestFixture]
    public class SeleneElementActionsSpecs : BaseTest
    {
        //TODO: consider not using shoulds here...

        //TODO: check "waiting InDom/Visible" aspects 

        [Test]
        public void SElementClear()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            S("input").Clear().Should(Be.Blank);
        }

        [Test]
        public void SElementScrollIntoView()
        {
            // prepare
            Given.OpenedPageWithBody("<input  style='margin-top:100cm;' type='text' value='ku ku'/>");
            SeleneElement ele = S("input");
            new SeleneDriver(Selene.GetWebDriver()).ShouldNot(BeInView(ele));

            // act
            ele.ScrollIntoView();

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
    }
}
