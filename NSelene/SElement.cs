using OpenQA.Selenium;
using NSelene.Conditions;
using System.Drawing;
using System;
using OpenQA.Selenium.Interactions;

namespace NSelene
{
    // TODO: consider name: WrapsWebElement
    public interface FindsWebElement
    {
        IWebElement ActualWebElement { get; }
    }

    // TODO: refactor impl to implement IWebElement interface in order to be able to do ToWebElement() conversion for integration purposes
    // TODO: implement ISearchContext
    public sealed class SElement : FindsWebElement
    {
        readonly SLocator<IWebElement> locator;

        readonly DriverProvider engine;

        public SElement(SLocator<IWebElement> locator, DriverProvider engine)
        {
            this.locator = locator;
            this.engine = engine;
        }

        public SElement(By locator, IWebDriver driver) 
            : this(new DriverWebElementSLocator(locator, new WrappedDriver(driver)), new WrappedDriver(driver)) {}

        public SElement(By locator) 
            : this(new DriverWebElementSLocator(locator, Config.DriverStorage), Config.DriverStorage) {}

        public SElement(IWebElement pageFactoryElement, IWebDriver driver)
            : this(new WrappedWebElementSLocator(pageFactoryElement), new WrappedDriver(driver)) {}

        public SLocator<IWebElement> SLocator 
        {
            get {
                return this.locator;
            }
        }

        public Actions Actions
        {
            get {
                //this.Should(Be.Visible); // TODO: should it be here? or should we create separate ajax friendly Actions wrapper?
                        // currently it also would duplicate other check for visibility inside common selement actions
                return new Actions(this.engine.Driver);
            }
        }

        // would it be better to use method GetActualWebElement() instead?
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
            return this.Should(condition);
        }

        public SElement ShouldNot(Condition<SElement> condition)
        {
            return Utils.WaitForNot(this, condition);
        }

        public SElement AssertToNot(Condition<SElement> condition)
        {
            return this.ShouldNot(condition);
        }

        public SElement Clear()
        {
            this.Should(Be.Visible);
            this.ActualWebElement.Clear();
            return this;
        }

        public SElement Click()
        {
            this.Should(Be.Visible);
            this.locator.Find().Click();
            return this;
        }
        
        public string GetAttribute(string name)
        {
            this.Should(Be.InDOM);
            return this.ActualWebElement.GetAttribute(name);
        }

        public string GetValue()
        {
            return this.GetAttribute("value");
        }

        public string GetCssValue(string property)
        {
            this.Should(Be.InDOM);
            return this.ActualWebElement.GetCssValue(property);
        }

        public SElement SendKeys(string keys)
        {
            this.Should(Be.Visible);
            this.ActualWebElement.SendKeys(keys);
            return this;
        }

        public SElement PressEnter()
        {
            return this.SendKeys(Keys.Enter);
        }

        public SElement PressTab()
        {
            return this.SendKeys(Keys.Tab);
        }

        public SElement PressEscape()
        {
            return this.SendKeys(Keys.Escape);
        }

        public SElement SetValue(string keys)
        {
            this.Should(Be.Visible);
            var webelement = this.ActualWebElement;
            webelement.Clear();
            webelement.SendKeys(keys);
            return this;
        }

        public SElement Set(string value)
        {
            return this.SetValue(value);
        }

        public SElement Submit()
        {
            this.Should(Be.Visible);
            this.ActualWebElement.Submit();
            return this;
        }

        public bool Displayed
        {
            get {
                this.Should(Be.InDOM);
                return this.ActualWebElement.Displayed;
            }
        }

        [Obsolete("IsDisplayed is deprecated and will be removed in next version, please use Displayed property instead.")]
        public bool IsDisplayed()
        {
            this.Should(Be.InDOM);
            return this.ActualWebElement.Displayed;
        }

        public bool Enabled
        {
            get {
                this.Should(Be.Visible);
                return this.ActualWebElement.Enabled;
            }
        }

        [Obsolete("IsEnabled is deprecated and will be removed in next version, please use Enabled property instead.")]
        public bool IsEnabled()
        {
            this.Should(Be.Visible);
            return this.ActualWebElement.Enabled;
        }

        public Point Location
        {
            get {
                this.Should(Be.Visible);
                return this.ActualWebElement.Location;
            }
        }

        [Obsolete("GetLocation is deprecated and will be removed in next version, please use Location property instead.")]
        public Point GetLocation()
        {
            this.Should(Be.Visible);
            return this.ActualWebElement.Location;
        }

        public bool Selected
        {
            get {
                this.Should(Be.Visible);
                return this.ActualWebElement.Selected;
            }
        }

        [Obsolete("IsSelected is deprecated and will be removed in next version, please use Selected property instead.")]
        public bool IsSelected()
        {
            this.Should(Be.Visible);
            return this.ActualWebElement.Selected;
        }

        public Size Size
        {
            get {
                this.Should(Be.Visible);
                return this.ActualWebElement.Size;
            }
        }

        [Obsolete("GetSize is deprecated and will be removed in next version, please use Size property instead.")]
        public Size GetSize()
        {
            this.Should(Be.Visible);
            return this.ActualWebElement.Size;
        }

        public string TagName
        {
            get {
                this.Should(Be.Visible);
                return this.ActualWebElement.TagName;
            }
        }

        [Obsolete("GetTagName is deprecated and will be removed in next version, please use TagName property instead.")]
        public string GetTagName()
        {
            this.Should(Be.Visible);
            return this.ActualWebElement.TagName;
        }

        public string Text
        {
            get {
                this.Should(Be.Visible);
                return this.ActualWebElement.Text;
            }
        }

        [Obsolete("GetText is deprecated and will be removed in next version, please use Text property instead.")]
        public string GetText()
        {
            this.Should(Be.Visible);
            return this.ActualWebElement.Text;
        }

        public SElement Hover()
        {
            this.Should(Be.Visible);
            this.Actions.MoveToElement(this.ActualWebElement).Perform();
            return this;
        }

        public SElement DoubleClick()
        {
            this.Should(Be.Visible);
            this.Actions.DoubleClick(this.ActualWebElement).Perform();
            return this;
        }

        public SElement Find(By locator)
        {
            return new SElement(new InnerWebElementSLocator(locator, this), this.engine);
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
            return new SCollection(new InnerWebElementsCollectionSLocator(locator, this), this.engine);
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
    }
}
