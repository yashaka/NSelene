using OpenQA.Selenium;
using NSelene.Conditions;
using System.Drawing;
using System;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace NSelene
{
    public interface WrapsWebElement
    {
        IWebElement ActualWebElement { get; }
    }

    // TODO: delete in next version, and mark SeleneElement as sealed
    [Obsolete("SElement is deprecated and will be removed in next version, please use SeleneElement type instead.")]
    public interface SElement : WrapsWebElement, IWebElement, ISearchContext, SeleneContext 
    {
        SeleneElement Should(Condition<SeleneElement> condition);
        SeleneElement ShouldNot(Condition<SeleneElement> condition);
        SeleneElement PressEnter();
        SeleneElement PressTab();
        SeleneElement PressEscape();
        SeleneElement SetValue(string keys);
        SeleneElement Set(string value);
        SeleneElement Hover();
        SeleneElement DoubleClick();
        SeleneElement Find(By locator);
        SeleneElement Find(string cssSelector);
        SeleneCollection FindAll(By locator);
        SeleneCollection FindAll(string cssSelector);
        new SeleneElement Clear();
        new SeleneElement SendKeys(string keys);
        new SeleneElement Submit();
        new SeleneElement Click();
        string Value
        {
            get;
        }
        //SeleneElement AssertTo(Condition<SeleneElement> condition);
        //SeleneElement AssertToNot(Condition<SeleneElement> condition);
        //SeleneElement S(By locator);
        //SeleneElement S(string cssSelector);
        //SeleneCollection SS(By locator);
        //SeleneCollection SS(string cssSelector);
    }

    // TODO: consider extracting SElement as interface... 
    public sealed class SeleneElement : SElement,
        WrapsWebElement, IWebElement, ISearchContext, SeleneContext
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
            : this(new SearchContextWebElementSLocator(locator, PrivateConfiguration.SharedDriver), PrivateConfiguration.SharedDriver) {}

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

        public SeleneElement ShouldNot(Condition<SeleneElement> condition)
        {
            return Selene.WaitForNot(this, condition);
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
            webelement.Clear();
            webelement.SendKeys(keys);
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

        public SeleneElement Find(string cssSelector)
        {
            return this.Find(By.CssSelector(cssSelector));
        }

        public SeleneCollection FindAll(By locator)
        {
            return new SeleneCollection(new SearchContextWebElementsCollectionSLocator(locator, this), this.driver);
        }

        public SeleneCollection FindAll(string cssSelector)
        {
            return this.FindAll(By.CssSelector(cssSelector));
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

        public SeleneElement SendKeys(string keys)
        {
            Should(Be.Visible);
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
                Should(Be.InDom);
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

        //
        // OBSOLETE MEMBERS
        //

        [Obsolete("GetSize is deprecated and will be removed in next version, please use Size property instead.")]
        public Size GetSize()
        {
            Should(Be.Visible);
            return this.ActualWebElement.Size;
        }

        [Obsolete("GetTagName is deprecated and will be removed in next version, please use TagName property instead.")]
        public string GetTagName()
        {
            Should(Be.Visible);
            return this.ActualWebElement.TagName;
        }

        [Obsolete("GetLocation is deprecated and will be removed in next version, please use Location property instead.")]
        public Point GetLocation()
        {
            Should(Be.Visible);
            return this.ActualWebElement.Location;
        }

        [Obsolete("IsEnabled is deprecated and will be removed in next version, please use Enabled property instead.")]
        public bool IsEnabled()
        {
            Should(Be.Visible);
            return this.ActualWebElement.Enabled;
        }

        [Obsolete("IsDisplayed is deprecated and will be removed in next version, please use Displayed property instead.")]
        public bool IsDisplayed()
        {
            Should(Be.InDom);
            return this.ActualWebElement.Displayed;
        }

        [Obsolete("IsSelected is deprecated and will be removed in next version, please use Selected property instead.")]
        public bool IsSelected()
        {
            Should(Be.Visible);
            return this.ActualWebElement.Selected;
        }

        [Obsolete("GetText is deprecated and will be removed in next version, please use Text property instead.")]
        public string GetText()
        {
            Should(Be.Visible);
            return this.ActualWebElement.Text;
        }

        [Obsolete("GetValue is deprecated and will be removed in next version, please use Value property instead.")]
        public string GetValue()
        {
            return GetAttribute("value");
        }
    }

    namespace Support.Extensions 
    {
        public static class SeleneElementExtensions 
        {
            public static SeleneElement AssertTo(this SeleneElement selement, Condition<SeleneElement> condition)
            {
                return selement.Should(condition);
            }

            public static SeleneElement AssertToNot(this SeleneElement selement, Condition<SeleneElement> condition)
            {
                return selement.ShouldNot(condition);
            }

            public static SeleneElement S(this SeleneElement selement, By locator)
            {
                return selement.Find(locator);
            }

            public static SeleneElement S(this SeleneElement selement, string cssSelector)
            {
                return selement.Find(cssSelector);
            }

            public static SeleneCollection SS(this SeleneElement selement, By locator)
            {
                return selement.FindAll(locator);
            }

            public static SeleneCollection SS(this SeleneElement selement, string cssSelector)
            {
                return selement.FindAll(cssSelector);
            }
        }
    }
}
