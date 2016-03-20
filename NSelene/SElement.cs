using OpenQA.Selenium;
using NSelene.Conditions;
using System.Drawing;

namespace NSelene
{

    public delegate IWebElement SElement();

    public static partial class Utils
    {
        public static SElement S(By locator)
        {
            return () => Find(locator);
        }

        public static SElement S(string cssSelector)
        {
            return S(By.CssSelector(cssSelector));
        }
    }

    public static class SElementExtensions
    {

        public static SElement Should(this SElement element, Condition<SElement> condition)
        {
            return Utils.WaitFor(element, condition);
        }

        public static SElement ShouldNot(this SElement element, Condition<SElement> condition)
        {
            return Utils.WaitForNot(element, condition);
        }

        // TODO: consider moving actions to separate file

        public static SElement Clear(this SElement element)
        {
            element.Should(Be.Visible);
            element().Clear();
            return element;
        }

        public static SElement Click(this SElement element)
        {
            element.Should(Be.Visible);
            element().Click();
            return element;
        }

        public static string GetAttribute(this SElement element, string name)
        {
            element.Should(Be.InDOM);
            return element().GetAttribute(name);
        }

        public static string GetValue(this SElement element)
        {
            return element.GetAttribute("value");
        }

        public static string GetCssValue(this SElement element, string property)
        {
            element.Should(Be.InDOM);
            return element().GetCssValue(property);
        }

        public static SElement SendKeys(this SElement element, string keys)
        {
            element.Should(Be.Visible);
            element().SendKeys(keys);
            return element;
        }

        public static SElement PressEnter(this SElement element)
        {
            return element.SendKeys(Keys.Enter);
        }

        public static SElement PressTab(this SElement element)
        {
            return element.SendKeys(Keys.Tab);
        }

        public static SElement PressEscape(this SElement element)
        {
            return element.SendKeys(Keys.Escape);
        }

        public static SElement SetValue(this SElement element, string keys)
        {
            element.Should(Be.Visible);
            var webelement = element();
            webelement.Clear();
            webelement.SendKeys(keys);
            return element;
        }

        public static SElement Set(this SElement element, string value)
        {
            return element.SetValue(value);
        }

        public static SElement Submit(this SElement element)
        {
            element.Should(Be.Visible);
            element().Submit();
            return element;
        }

        public static bool IsDisplayed(this SElement element)
        {
            element.Should(Be.InDOM);
            return element().Displayed;
        }

        public static bool IsEnabled(this SElement element)
        {
            element.Should(Be.Visible);
            return element().Enabled;
        }

        public static Point GetLocation(this SElement element)
        {
            element.Should(Be.Visible);
            return element().Location;
        }

        public static bool IsSelected(this SElement element)
        {
            element.Should(Be.Visible);
            return element().Selected;
        }

        public static Size GetSize(this SElement element)
        {
            element.Should(Be.Visible);
            return element().Size;
        }

        public static string GetTagName(this SElement element)
        {
            element.Should(Be.Visible);
            return element().TagName;
        }

        public static string GetText(this SElement element)
        {
            element.Should(Be.Visible);
            return element().Text;
        }

        public static SElement Find(this SElement element, By locator)
        {
            return () => element.Should(Be.Visible)().FindElement(locator);
            /* \
                                                      why do we need it? seem's like we do not... (#TODO)*/
        }

        public static SElement S(this SElement element, By locator)
        {
            return element.Find(locator);
        }

        public static SElement Find(this SElement element, string cssSelector)
        {
            return element.Find(By.CssSelector(cssSelector));
        }

        public static SElement S(this SElement element, string cssSelector)
        {
            return element.Find(cssSelector);
        }

        public static SCollection FindAll(this SElement element, By locator)
        {
            return () => element.Should(Be.Visible)().FindElements(locator);
        }

        public static SCollection FindAll(this SElement element, string cssSelector)
        {
            return element.FindAll(By.CssSelector(cssSelector));
        }

        public static SCollection SS(this SElement element, By locator)
        {
            return element.FindAll(locator);
        }

        public static SCollection SS(this SElement element, string cssSelector)
        {
            return element.FindAll(cssSelector);
        }


    }
		
}
