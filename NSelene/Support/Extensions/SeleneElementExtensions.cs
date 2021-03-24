using OpenQA.Selenium;
using NSelene.Conditions;
using System.Drawing;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using System;
using NSelene;

namespace NSelene.Support.Extensions
{
    public static class SeleneElementExtensions
    {
        public static SeleneElement AssertTo(this SeleneElement element, Condition<SeleneElement> condition)
        {
            return element.Should(condition);
        }

        [Obsolete("Use the negative condition instead, e.g. AssertTo(Be.Not.Visible)")]
        public static SeleneElement AssertToNot(this SeleneElement element, Condition<SeleneElement> condition)
        {
            return element.ShouldNot(condition);
        }

        public static SeleneElement S(this SeleneElement element, By locator)
        {
            return element.Find(locator);
        }

        public static SeleneElement S(this SeleneElement element, string cssSelector)
        {
            return element.Find(cssSelector);
        }

        public static SeleneCollection SS(this SeleneElement element, By locator)
        {
            return element.FindAll(locator);
        }

        public static SeleneCollection SS(this SeleneElement element, string cssSelector)
        {
            return element.FindAll(cssSelector);
        }

        // todo: should we move Js* extensions to a separate namespace
        //       to use them like: `using Nselene.Support.Extensions.Js;`
        public static SeleneElement JsClick(this SeleneElement element, int centerXOffset = 0, int centerYOffset = 0)
        {
            element.ExecuteScript(@"
                    var centerXOffset = args[0];
                    var centerYOffset = args[1];

                    var rect = element.getBoundingClientRect();
                    element.dispatchEvent(new MouseEvent('click', {
                        'view': window,
                        'bubbles': true,
                        'cancelable': true,
                        'clientX': rect.left + rect.width/2 + centerXOffset,
                        'clientY': rect.top + rect.height/2 + centerYOffset 
                    }));
                    ",
                centerXOffset,
                centerYOffset);
            return element;
        }

        public static SeleneElement JsScrollIntoView(this SeleneElement element)
        {
            element.ExecuteScript("element.scrollIntoView({ behavior: 'smooth', block: 'center'})");
            return element;
        }
    }
}