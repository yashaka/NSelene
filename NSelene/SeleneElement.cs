using OpenQA.Selenium;
using NSelene.Conditions;
using System.Drawing;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using System;
using NSelene.Support.SeleneElementJsExtensions;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
            bool? setValueByJs = null,
            bool? typeByJs = null,
            bool? clickByJs = null
        )
        {
            _SeleneSettings_ customized = new Configuration();

            customized.Driver = driver;
            customized.Timeout = timeout;
            customized.PollDuringWaits = pollDuringWaits;
            customized.SetValueByJs = setValueByJs;
            customized.TypeByJs = typeByJs;
            customized.ClickByJs = clickByJs;

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
        Actions Actions => new Actions(this.config.Driver);
        Wait<SeleneElement> Wait
        {
            get
            {
                var paramsAndTheirUsagePattern = new Regex(@"\(?(\w+)\)?\s*=>\s*?\1\.");
                return new Wait<SeleneElement>(
                    entity: this,
                    timeout: this.config.Timeout ?? Configuration.Timeout,
                    polling: this.config.PollDuringWaits ?? Configuration.PollDuringWaits,
                    _describeLambdaName: it => paramsAndTheirUsagePattern.Replace(
                        it, 
                        ""
                    )
                );
            }
        }

        // TODO: consider renaming it to something more concise and handy in use...
        //       take into account that maybe it's good to just add an alias
        //       because in failures it looks pretty good now:
        //       > Timed out after 0.25s, while waiting for:
        //       > Browser.Element(a).ActualWebElement.Click()
        //       
        //       some alias candidates:
        //       * Browser.Element(...).Get()
        //         - kind of tells that we get Element, not raw WebElement
        //         + one of the concisest
        //       * Browser.Element(...).Find()
        //         - not consistent with Find(selector), 
        //         + but tells that we actually finding something
        //       * Browser.Element(...).Locate()
        //         + like above but does not interfere with other names
        //         + consistent with Element.locator
        //         - not the concisest
        //       * Browser.Element(...).Raw
        //         + !!! in fact it's "raw" in its nature, and the most concise
        //         - maybe a bit "too technical", but for tech-guys probably pretty obvious
        //           yeah, Selene is for users not for coders, 
        //           + but actual raw webelement is also not for users;)
        //       - Browser.Element(...).Invoke()
        //       - Browser.Element(...).Call()
        public IWebElement ActualWebElement {
            get {
                return locator.Find();
            }
        }

        private IWebElement ActualVisibleWebElement {
            get {
                var webElement = locator.Find();
                if (! webElement.Displayed)
                {
                    throw new WebDriverException("Element not visible");
                }
                return webElement;
            }
        }

        /// 
        /// Returns:
        ///     actual not overlapped (and so visible too) webelement
        ///     or...
        /// 
        /// Throws:
        ///     WebDriverException if overlapped
        ///     Or whatever ActualVisibleWebElementAndMaybeItsCover throws
        ///
        private IWebElement ActualNotOverlappedWebElement {
            get {
                var (webElement, cover) = this.ActualVisibleWebElementAndMaybeItsCover();
                if (cover != null)
                {
                    throw new WebDriverException($"Element is overlapped by: {cover.GetAttribute("outerHTML")}");
                }
                return webElement;
            }
        }

        /// 
        /// Summary:
        ///     Checks wether visible element is covered/overlapped 
        ///     by another element at point from element's center with...
        ///
        /// Parameters:
        ///     centerXOffset
        ///     centerYOffset
        ///
        /// Returns:
        ///     Tuple<IWebElement> with:
        ///         [webelement, null] if not overlapped else [webelement, coveredWebElement]
        /// Throws: 
        ///     if element is not visible: 
        ///         javascript error: element is not visible
        private (IWebElement, IWebElement) ActualVisibleWebElementAndMaybeItsCover(
            int centerXOffset = 0, 
            int centerYOffset = 0
        )
        {
            // TODO: will it work if element is not in view but is not covered?
            // check in https://developer.mozilla.org/en-US/docs/Web/API/Document/elementFromPoint:
            // > If the specified point is outside the visible bounds of the document 
            // > or either coordinate is negative, the result is null.
            // TODO: cover it by tests (also the iframe case (check the docs by above link))
            var results = this.ExecuteScript(
                @"
                var centerXOffset = args[0];
                var centerYOffset = args[1];

                var isVisible = !!( 
                    element.offsetWidth 
                    || element.offsetHeight 
                    || element.getClientRects().length 
                ) && window.getComputedStyle(element).visibility !== 'hidden'

                if (!isVisible) {
                    throw 'element is not visible'
                }

                var rect = element.getBoundingClientRect();
                var x = rect.left + rect.width/2 + centerXOffset;
                var y = rect.top + rect.height/2 + centerYOffset;

                // TODO: now we return [element, null] in case of elementFromPoint returns null
                //       (kind of – if we don't know what to do, let's at least not block the execution...)
                //       rethink this... and handle the iframe case
                //       read more in https://developer.mozilla.org/en-US/docs/Web/API/Document/elementFromPoint

                var elementByXnY = document.elementFromPoint(x,y);
                if (elementByXnY == null) {
                    return [element, null];
                }

                var isNotOverlapped = element.isSameNode(elementByXnY);

                return isNotOverlapped 
                       ? [element, null]
                       : [element, elementByXnY];
                "
                , centerXOffset
                , centerYOffset
            );
            if (results.GetType() == typeof(ReadOnlyCollection<IWebElement>))
            {
                var webelements = (ReadOnlyCollection<IWebElement>) results;
                return (webelements[0], webelements[1]);
            }
            var objects = (ReadOnlyCollection<object>) results;
            return ((IWebElement) objects[0], objects[1] as IWebElement);
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

        // TODO: consider moving to Extensions or even deprecate
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
        // SeleneElement Commands
        // (chainable alternatives to IWebElement void methods)
        //

        public SeleneElement Clear()
        {
            // TODO: consider something like AllowActionOnHidden
            //       is it enough? should we separate AllowActionOnOverlapped and AllowActionOnHidden?
            //       (overlapped is is also kind of "hidden" in context of normal meaning...)
            //       why we would want to allow clear on hidden? for example to clear upload file hidden input;)
            //       (TODO: by the way clear is not allowed on input[type=file] while sendKeys is, why?)
            /*
            if (this.config.AllowActionOnHidden ?? Configuration.AllowActionOnHidden) 
            {
                // this.Wait.For(self => self.ActualWebElement.Clear()); // this will yet fail with ElementNotInteractableException
                // so to really allow, we should do here something like:
                this.Wait.For(self => self.JsClear());
                // should we?
                // - use AllowActionOnHidden for this, or use ClearByJs? (consistent with other...)

            } else 
            {
                this.Wait.For(self => self.ActualNotOverlappedWebElement.Clear());
            }
             */

            this.Wait.For(self => self.ActualNotOverlappedWebElement.Clear());
            return this;
        }

        public SeleneElement Type(string keys)
        {
            if (this.config.TypeByJs ?? Configuration.TypeByJs) 
            {
                this.Wait.For(new _Lambda<SeleneElement, object>(
                    $"JsType({keys})",
                    self => self.JsType(keys)
                ));
            } else
            {
                this.Wait.For(new _Lambda<SeleneElement, object>(
                    $"ActualNotOverlappedWebElement.SendKeys({keys})", // TODO: should we render it as Type({keys})?
                    self => self.ActualNotOverlappedWebElement.SendKeys(keys)
                ));
            }
            return this;
        }

        /// 
        /// Summary:
        ///     A low level method similar to raw selenium webdriver one, 
        ///     with similar behaviour in context of "hidden" elements.
        ///     Waits only till "Be.InDom".
        ///     Useful to send keys to hidden elements, like "upload file input"
        ///     Also it is chainable, like other SeleneElement's methods.
        ///
        public SeleneElement SendKeys(string keys)
        {
            // TODO: should we deprecate it? and keep just something like:
            //           element.With(allowActionOnHidden: true).Type(keys)
            //       ?
            //       should we consider adding Upload(string file)?
            //       * to cover the corresponding case...
            //       * yet be valid only for web... not for mobile :(

            // TODO: consider failing fast (skip waiting) if got something like "WebDriverException : invalid argument: File not found"
            //       how would we implement it by the way? :D
            this.Wait.For(new _Lambda<SeleneElement, object>(
                $"ActualWebElement.SendKeys({keys})",
                self => self.ActualWebElement.SendKeys(keys)
            ));
            return this;
        }

        public SeleneElement Submit()
        {
            // TODO: consider making ActualNotOverlappedWebElement configurable, cause somebody may not want extra js checks....
            this.Wait.For(self => self.ActualNotOverlappedWebElement.Submit());
            return this;
        }

        public SeleneElement Click()
        {
            if (this.config.ClickByJs ?? Configuration.ClickByJs)
            {
                // TODO: should we incorporate wait into this.ExecuteScript ? maybe make it configurable (Configuration.WaitForExecuteScript, false by default)?
                // TODO: to keep here just this.JsClick(); ?
                this.Wait.For(self => self.JsClick(0, 0));
            } 
            else 
            {
                this.Wait.For(self => self.ActualWebElement.Click());
            }
            return this;
        }

        //
        // Queries
        //

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
            // TODO: this method fails if this.ActualWebElement failed – this is pretty not in NSelene style!
            //       probably we have to  wrap it inside wait!
            IJavaScriptExecutor js = (IJavaScriptExecutor)this.config.Driver;
            return js.ExecuteScript($@"
                return (function(element, args) {{
                    {scriptOnElementAndArgs}
                }})(arguments[0], arguments[1])", new object[] { this.ActualWebElement, args });
        }
    }

}
