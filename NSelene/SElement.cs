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

    // TODO: consider extracting SElement as interface... 
    public sealed class SElement : WrapsWebElement, IWebElement, ISearchContext, SContext
    {
        readonly SLocator<IWebElement> locator;

        readonly SDriver driver;

        public SElement(SLocator<IWebElement> locator, SDriver driver)
        {
            this.locator = locator;
            this.driver = driver;
        }

        public SElement(By locator, SDriver driver) 
            : this(new SearchContextWebElementSLocator(locator, driver), driver) {}

        public SElement(By locator) 
            : this(new SearchContextWebElementSLocator(locator, PrivateConfiguration.SharedDriver), PrivateConfiguration.SharedDriver) {}

        //TODO: consider renaming pageFactoryElement to element, becuase we nevertheless use it sometimes to just wrap non-proxy webelement
        public SElement(IWebElement pageFactoryElement, IWebDriver driver)
            : this(new WrappedWebElementSLocator(pageFactoryElement), new SDriver(driver)) {}

        //TODO: do we need it as public? it's a potential hole in context of violation "tell don't ask"
        public SLocator<IWebElement> SLocator 
        {
            get {
                return this.locator;
            }
        }

        // TODO: consider making it just a field initialized in constructor
        Actions Actions
        {
            get {
                return new Actions(this.driver);
            }
        }

        // TODO: would it be better to use method GetActualWebElement() instead?
        public IWebElement ActualWebElement
        {
            get {
                return locator.Find();
            }
        }

        public override string ToString()
        {
            return this.locator.Description;
        }

        public SElement Should(Condition<SElement> condition)
        {
            return Utils.WaitFor(this, condition);
        }

        public SElement AssertTo(Condition<SElement> condition)
        {
            return Should(condition);
        }

        public SElement ShouldNot(Condition<SElement> condition)
        {
            return Utils.WaitForNot(this, condition);
        }

        public SElement AssertToNot(Condition<SElement> condition)
        {
            return ShouldNot(condition);
        }

        // TODO: refactor to property Value ?
        public string GetValue()
        {
            return GetAttribute("value");
        }

        public SElement PressEnter()
        {
            SendKeys(Keys.Enter);
            return this;
        }

        public SElement PressTab()
        {
            SendKeys(Keys.Tab);
            return this;
        }

        public SElement PressEscape()
        {
            SendKeys(Keys.Escape);
            return this;
        }

        public SElement SetValue(string keys)
        {
            Should(Be.Visible);
            var webelement = this.ActualWebElement;
            webelement.Clear();
            webelement.SendKeys(keys);
            return this;
        }

        public SElement Set(string value)
        {
            return SetValue(value);
        }

        public SElement Hover()
        {
            Should(Be.Visible);
            this.Actions.MoveToElement(this.ActualWebElement).Perform();
            return this;
        }

        public SElement DoubleClick()
        {
            Should(Be.Visible);
            this.Actions.DoubleClick(this.ActualWebElement).Perform();
            return this;
        }

        public SElement Find(By locator)
        {
            return new SElement(new SearchContextWebElementSLocator(locator, this), this.driver);
        }

        public SElement S(By locator)
        {
            return this.Find(locator);
        }

        public SElement Find(string cssSelector)
        {
            return this.Find(By.CssSelector(cssSelector));
        }

        public SElement S(string cssSelector)
        {
            return this.Find(cssSelector);
        }

        public SCollection FindAll(By locator)
        {
            return new SCollection(new SearchContextWebElementsCollectionSLocator(locator, this), this.driver);
        }

        public SCollection FindAll(string cssSelector)
        {
            return this.FindAll(By.CssSelector(cssSelector));
        }

        public SCollection SS(By locator)
        {
            return this.FindAll(locator);
        }

        public SCollection SS(string cssSelector)
        {
            return this.FindAll(cssSelector);
        }

        //
        // SElement chainable alternatives to IWebElement void methods
        //

        public SElement Clear()
        {
            Should(Be.Visible);
            this.ActualWebElement.Clear();
            return this;
        }

        public SElement SendKeys(string keys)
        {
            Should(Be.Visible);
            this.ActualWebElement.SendKeys(keys);
            return this;
        }

        public SElement Submit()
        {
            Should(Be.Visible);
            this.ActualWebElement.Submit();
            return this;
        }

        public SElement Click()
        {
            Should(Be.Visible);
            this.ActualWebElement.Click();
            return this;
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
                Should(Be.InDOM);
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
            Should(Be.InDOM);
            return this.ActualWebElement.GetAttribute(name);
        }

        public string GetCssValue(string property)
        {
            Should(Be.InDOM);
            return this.ActualWebElement.GetCssValue(property);
        }

        //
        // ISearchContext methods
        //

        IWebElement ISearchContext.FindElement (By by)
        {
            //Should(Be.Visible);
            //return this.ActualWebElement.FindElement(by);
            return new SElement(new SearchContextWebElementSLocator(by, this), this.driver);
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements (By by)
        {
            //Should(Be.Visible);
            //return this.ActualWebElement.FindElements(by);
            return new SCollection(new SearchContextWebElementsCollectionSLocator(by, this), this.driver).ToReadOnlyWebElementsCollection();
        }

        //
        // SContext methods
        //

        IWebElement SContext.FindElement (By by)
        {
            Should(Be.Visible);
            return this.ActualWebElement.FindElement(by);
        }

        ReadOnlyCollection<IWebElement> SContext.FindElements (By by)
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
            Should(Be.InDOM);
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
    }
}
