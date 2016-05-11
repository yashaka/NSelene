using OpenQA.Selenium;
using NSelene.Conditions;
using System.Drawing;
using System;

namespace NSelene
{

    public delegate IWebElement FindsWebElement();

    public interface GetsWebElement
    {
        FindsWebElement GetActualWebElement { get; }
    }

    public sealed class SElement : GetsWebElement
    {
        readonly By locator;
        readonly FindsWebElement finder;

        public SElement(By locator, FindsWebElement finder)
        {
            this.locator = locator;
            this.finder = finder;
        }

        public SElement(By locator, IWebDriver driver) 
            : this(locator, () => driver.FindElement(locator)) {}

        public SElement(By locator) 
            : this(locator, () => Utils.GetDriver().FindElement(locator)) {}

        public FindsWebElement GetActualWebElement {
            get {
                return this.finder;
            }
        }

        public override string ToString()
        {
            return this.locator.ToString();
        }
    }

    public static partial class Utils
    {
        public static SElement S(By locator)
        {
            //return () => Find(locator);
            return new SElement(locator);
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
            element.GetActualWebElement().Clear();
            return element;
        }

        public static SElement Click(this SElement element)
        {
            element.Should(Be.Visible);
            element.GetActualWebElement().Click();
            return element;
        }

        public static string GetAttribute(this SElement element, string name)
        {
            element.Should(Be.InDOM);
            return element.GetActualWebElement().GetAttribute(name);
        }

        public static string GetValue(this SElement element)
        {
            return element.GetAttribute("value");
        }

        public static string GetCssValue(this SElement element, string property)
        {
            element.Should(Be.InDOM);
            return element.GetActualWebElement().GetCssValue(property);
        }

        public static SElement SendKeys(this SElement element, string keys)
        {
            element.Should(Be.Visible);
            element.GetActualWebElement().SendKeys(keys);
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
            var webelement = element.GetActualWebElement();
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
            element.GetActualWebElement().Submit();
            return element;
        }

        public static bool IsDisplayed(this SElement element)
        {
            element.Should(Be.InDOM);
            return element.GetActualWebElement().Displayed;
        }

        public static bool IsEnabled(this SElement element)
        {
            element.Should(Be.Visible);
            return element.GetActualWebElement().Enabled;
        }

        public static Point GetLocation(this SElement element)
        {
            element.Should(Be.Visible);
            return element.GetActualWebElement().Location;
        }

        public static bool IsSelected(this SElement element)
        {
            element.Should(Be.Visible);
            return element.GetActualWebElement().Selected;
        }

        public static Size GetSize(this SElement element)
        {
            element.Should(Be.Visible);
            return element.GetActualWebElement().Size;
        }

        public static string GetTagName(this SElement element)
        {
            element.Should(Be.Visible);
            return element.GetActualWebElement().TagName;
        }

        public static string GetText(this SElement element)
        {
            element.Should(Be.Visible);
            return element.GetActualWebElement().Text;
        }

        public static SElement Hover(this SElement element)
        {
            element.Should(Be.Visible);
            Utils.SActions().MoveToElement(element.GetActualWebElement()).Perform();
            return element;
        }

        public static SElement DoubleClick(this SElement element)
        {
            element.Should(Be.Visible);
            Utils.SActions().DoubleClick(element.GetActualWebElement()).Perform();
            return element;
        }

        public static SElement Find(this SElement element, By locator)
        {
            return new SElement(new PseudoBy(string.Format("By.Selene: ({0}).FindInner({1})", element, locator))
                                , () => element.Should(Be.Visible).GetActualWebElement().FindElement(locator)
                               );
            //return () => element.Should(Be.Visible)().FindElement(locator);
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
            return new SCollection(new PseudoBy(string.Format("By.Selene: ({0}).FindAllInner({1})", element, locator))
                                   , () => element.Should(Be.Visible).GetActualWebElement().FindElements(locator));
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
