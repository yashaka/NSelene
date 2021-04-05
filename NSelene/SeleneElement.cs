using OpenQA.Selenium;
using NSelene.Conditions;
using System.Drawing;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using System;
using NSelene.Support.SeleneElementJsExtensions;

namespace NSelene
{
    public interface WrapsWebElement
    {
        IWebElement ActualWebElement { get; }
    }

    // TODO: consider extracting SElement as interface... 
    public sealed class SeleneElement 
    : WrapsWebElement, IWebElement, ISearchContext, SeleneContext
    {
        readonly SeleneLocator<IWebElement> locator;

        public readonly _SeleneSettings_ config; // TODO: remove this
        // private readonly _SeleneSettings_ config;
        
        internal SeleneElement(
            SeleneLocator<IWebElement> locator, 
            _SeleneSettings_ config
        ) 
        {
            this.locator = locator;
            this.config = config;
        }        
        
        internal SeleneElement(
            By locator, 
            _SeleneSettings_ config
        ) 
        : this (
            new SearchContextWebElementSLocator(
                locator, 
                config
            ),
            config
        ) {}

        internal SeleneElement(IWebElement elementToWrap, _SeleneSettings_ config)
        : this(new WrappedWebElementSLocator(elementToWrap), config) {}

        public SeleneElement With(
            IWebDriver driver = null,
            double? timeout = null,
            double? pollDuringWaits = null,
            bool? setValueByJs = null
        )
        {
            _SeleneSettings_ customized = new Configuration();

            customized.Driver = driver;
            customized.Timeout = timeout;
            customized.PollDuringWaits = pollDuringWaits;
            customized.SetValueByJs = setValueByJs;

            return new SeleneElement(
                this.locator, 
                this.config.With(customized)
            );
        }

        public SeleneElement _With_(_SeleneSettings_ config)
        {
            return new SeleneElement(
                this.locator, 
                config
            );
        }

        // TODO: consider making it Obsolete, actions is an object with broader context than Element
        Actions Actions {
            get {
                return new Actions(this.config.Driver);
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
            return Selene.WaitFor(
                this, 
                condition, 
                this.config.Timeout ?? Configuration.Timeout,
                this.config.PollDuringWaits ?? Configuration.PollDuringWaits
            );
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
            if (this.config.SetValueByJs ?? Configuration.SetValueByJs) 
            {
                this.JsSetValue(keys);
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
            return new SeleneElement(
                new SearchContextWebElementSLocator(locator, this), 
                this.config
            );
        }

        public SeleneElement Find(string cssOrXPathSelector)
        {
            return this.Find(Utils.ToBy(cssOrXPathSelector));
        }

        public SeleneCollection FindAll(By locator)
        {
            return new SeleneCollection(
                new SearchContextWebElementsCollectionSLocator(locator, this), 
                this.config
            );
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
            return new SeleneElement(
                new SearchContextWebElementSLocator(by, this),
                this.config
            );
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements (By by)
        {
            return new SeleneCollection(
                new SearchContextWebElementsCollectionSLocator(by, this), 
                this.config
            ).ToReadOnlyWebElementsCollection();
        }

        //
        // SContext methods
        //

        IWebElement SeleneContext.FindElement (By by)
        {
            Should(Be.Visible); // TODO: do we need it here?
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
        /// </remarks>
        public object ExecuteScript(string scriptOnElementAndArgs, params object[] args)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)this.config.Driver;
            return js.ExecuteScript($@"
                return (function(element, args) {{
                    {scriptOnElementAndArgs}
                }})(arguments[0], arguments[1])", new object[] { this.ActualWebElement, args });
        }
    }

}
