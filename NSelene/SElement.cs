using OpenQA.Selenium;
using NSelene.Conditions;

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

        public static SElement Click(this SElement element)
        {
            element.Should(Be.Visible);
            element().Click();
            return element;
        }

        public static SElement SendKeys(this SElement element, string keys)
        {
            element.Should(Be.Visible);
            element().SendKeys(keys);
            return element;
        }

        public static SElement PressEnter(this SElement element)
        {
            element.Should(Be.Visible);
            element().SendKeys(Keys.Enter);
            return element;
        }

        public static SElement SetValue(this SElement element, string value)
        {
            element.Should(Be.Visible);
            element().Clear();
            element().SendKeys(value);
            return element;
        }

        public static SElement Find(this SElement element, By locator)
        {
            return () => element.Should(Be.Visible)().FindElement(locator);
            /* \
                                                      why do we need it? seem's like we do not... (#TODO)*/
        }

        public static SElement Find(this SElement element, string cssSelector)
        {
            return element.Find(By.CssSelector(cssSelector));
        }
    }
		
}
