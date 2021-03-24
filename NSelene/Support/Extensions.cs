using OpenQA.Selenium;
using NSelene.Conditions;
using System.Drawing;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using System;
using NSelene;

using SeleneElementJsExtentions = NSelene.Support.SeleneElementJsExtensions.SeleneElementJsExtensions;

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

        public static SeleneElement JsClick(this SeleneElement element, int centerXOffset = 0, int centerYOffset = 0)
        {
            return SeleneElementJsExtentions.JsClick(element, centerXOffset, centerYOffset);
        }

        public static SeleneElement JsScrollIntoView(this SeleneElement element)
        {
            return SeleneElementJsExtentions.JsScrollIntoView(element);
        }
    }
}