using OpenQA.Selenium;
using NSelene.Conditions;
using System.Drawing;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using System;

namespace NSelene
{
    public interface WrapsWebElement
    {
        IWebElement ActualWebElement { get; }
    }

    // TODO: consider extracting SElement as interface... 
    public sealed class SeleneElement : WrapsWebElement, IWebElement, ISearchContext, SeleneContext
    {
        readonly SeleneLocator<IWebElement> locator;

        readonly SeleneDriver driver;

        internal SeleneElement(SeleneLocator<IWebElement> locator, SeleneDriver driver)
        {
            this.locator = locator;
            this.driver = driver;
        }

        internal SeleneElement(By locator, SeleneDriver driver) 
            : this(new SearchContextWebElementSLocator(locator, driver), driver) {}

        internal SeleneElement(By locator) 
            : this(new SearchContextWebElementSLocator(locator, Selene.SharedDriver), Selene.SharedDriver) {}

        internal SeleneElement(IWebElement elementToWrap, IWebDriver driver)
            : this(new WrappedWebElementSLocator(elementToWrap), new SeleneDriver(driver)) {}

        // TODO: consider making it just a field initialized in constructor
        Actions Actions {
            get {
                return this.driver.Actions();
            }
        }

        public IWebElement ActualWebElement {
            get {
                return locator.Find();
            }
        }

        public override string ToString()
        {
            return this.locator.Description;
        }

        public SeleneElement Should(Condition<SeleneElement> condition)
        {
            return Selene.WaitFor(this, condition);
        }

        [Obsolete("Use the negative condition instead")]
        public SeleneElement ShouldNot(Condition<SeleneElement> condition)
        {
            return Selene.WaitForNot(this, condition);
        }

        public bool Matching(Condition<SeleneElement> condition)
        {
            try 
            {
                return condition.Apply(this);
            }
            catch
            {
                return false;
            }
        }
        public bool WaitUntil(Condition<SeleneElement> condition)
        {
            try 
            {
                this.Should(condition);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public SeleneElement PressEnter()
        {
            SendKeys(Keys.Enter);
            return this;
        }

        public SeleneElement PressTab()
        {
            SendKeys(Keys.Tab);
            return this;
        }

        public SeleneElement PressEscape()
        {
            SendKeys(Keys.Escape);
            return this;
        }

        public SeleneElement SetValue(string keys)
        {
            Should(Be.Visible);
            var webelement = this.ActualWebElement;
            if (Configuration.SetValueByJs) 
            {
                // todo: refactor to make it possible to write this.ExecuteScript(...)
                IJavaScriptExecutor js = (IJavaScriptExecutor) this.driver.Value;
                js.ExecuteScript(
                    @"return (function(element, text) {
                        var maxlength = element.getAttribute('maxlength') === null
                            ? -1
                            : parseInt(element.getAttribute('maxlength'));
                        element.value = maxlength === -1
                            ? text
                            : text.length <= maxlength
                                ? text
                                : text.substring(0, maxlength);
                        return null;
                    })(arguments[0], arguments[1]);", webelement, keys);
            } else 
            {
                webelement.Clear();
                webelement.SendKeys(keys);
            }
            return this;
        }

        // TODO: consider moving to Extensions
        public SeleneElement Set(string value)
        {
            return SetValue(value);
        }

        public SeleneElement Hover()
        {
            Should(Be.Visible);
            this.Actions.MoveToElement(this.ActualWebElement).Perform();
            return this;
        }

        public SeleneElement DoubleClick()
        {
            Should(Be.Visible);
            this.Actions.DoubleClick(this.ActualWebElement).Perform();
            return this;
        }

        public SeleneElement Find(By locator)
        {
            return new SeleneElement(new SearchContextWebElementSLocator(locator, this), this.driver);
        }

        public SeleneElement Find(string cssOrXPathSelector)
        {
            return this.Find(Utils.ToBy(cssOrXPathSelector));
        }

        public SeleneCollection FindAll(By locator)
        {
            return new SeleneCollection(new SearchContextWebElementsCollectionSLocator(locator, this), this.driver);
        }

        public SeleneCollection FindAll(string cssOrXPathSelector)
        {
            return this.FindAll(Utils.ToBy(cssOrXPathSelector));
        }

        //
        // SElement chainable alternatives to IWebElement void methods
        //

        public SeleneElement Clear()
        {
            Should(Be.Visible);
            this.ActualWebElement.Clear();
            return this;
        }

        public SeleneElement Type(string keys)
        {
            Should(Be.Visible);
            this.ActualWebElement.SendKeys(keys);
            return this;
        }

        public SeleneElement SendKeys(string keys)
        {
            Should(Be.InDom);
            this.ActualWebElement.SendKeys(keys);
            return this;
        }

        public SeleneElement Submit()
        {
            Should(Be.Visible);
            this.ActualWebElement.Submit();
            return this;
        }

        public SeleneElement Click()
        {
            Should(Be.Visible);
            this.ActualWebElement.Click();
            return this;
        }

        public string Value
        {
            get {
                return GetAttribute("value");
            }
        }

        //
        // IWebElement Properties
        //

        public bool Enabled
        {
            get {
                Should(Be.Visible);
                return this.ActualWebElement.Enabled;
            }
        }

        public Point Location
        {
            get {
                Should(Be.Visible);
                return this.ActualWebElement.Location;
            }
        }

        public bool Selected
        {
            get {
                Should(Be.Visible);
                return this.ActualWebElement.Selected;
            }
        }

        public Size Size
        {
            get {
                Should(Be.Visible);
                return this.ActualWebElement.Size;
            }
        }

        public string TagName
        {
            get {
                Should(Be.Visible);
                return this.ActualWebElement.TagName;
            }
        }

        public string Text
        {
            get {
                Should(Be.Visible);
                return this.ActualWebElement.Text;
            }
        }

        public bool Displayed
        {
            get {
                Should(Be.InDom);  // todo: probably we should not care in dom it or not...
                return this.ActualWebElement.Displayed;
            }
        }

        //
        // IWebElement Methods
        //

        void IWebElement.Clear()
        {
            Clear();
        }

        void IWebElement.SendKeys(string keys)
        {
            SendKeys(keys);
        }

        void IWebElement.Submit()
        {
            Submit();
        }

        void IWebElement.Click()
        {
            Click();
        }

        public string GetAttribute(string name)
        {
            Should(Be.InDom);
            return this.ActualWebElement.GetAttribute(name);
        }

        public string GetProperty (string propertyName)
        {
            Should(Be.InDom);
            return this.ActualWebElement.GetProperty(propertyName);
        }

        public string GetCssValue(string property)
        {
            Should(Be.InDom);
            return this.ActualWebElement.GetCssValue(property);
        }

        //
        // ISearchContext methods
        //

        IWebElement ISearchContext.FindElement (By by)
        {
            //Should(Be.Visible);
            //return this.ActualWebElement.FindElement(by);
            return new SeleneElement(new SearchContextWebElementSLocator(by, this), this.driver);
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements (By by)
        {
            //Should(Be.Visible);
            //return this.ActualWebElement.FindElements(by);
            return new SeleneCollection(new SearchContextWebElementsCollectionSLocator(by, this), this.driver).ToReadOnlyWebElementsCollection();
        }

        //
        // SContext methods
        //

        IWebElement SeleneContext.FindElement (By by)
        {
            Should(Be.Visible);
            return this.ActualWebElement.FindElement(by);
        }

        ReadOnlyCollection<IWebElement> SeleneContext.FindElements (By by)
        {
            Should(Be.Visible);
            return this.ActualWebElement.FindElements(by);
        }

        /// <remarks>
        ///     This method executes JavaScript in the context of the currently selected frame or window.
        ///     This means that "document" will refer to the current document and "element" will refer to this element
        ///  </remarks>
        public object ExecuteScript(string scriptOnElementAndArgs, params object[] args)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)this.driver.Value;
            return js.ExecuteScript($@"
                return (function(element, args) {{
                    {scriptOnElementAndArgs}
                }})(arguments[0], arguments[1])", new object[] { this.ActualWebElement, args });
        }
    }

}
