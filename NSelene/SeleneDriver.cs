using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using NSelene.Conditions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NSelene
{

    // todo: why the function over class would not be enough in c#? 
    //       seems like there was some delegates limitation... 
    // todo: should we make it internal?
    public interface IWebDriverSource
    {
        IWebDriver Driver { get; set; }
    }

    internal class SharedThreadLocalDriverSource : IWebDriverSource
    {
        public IWebDriver Driver {
            get {
                return Configuration.Driver;
            }

            set {
                Configuration.Driver = value;
            }
        }
    }

    public class ExplicitDriverSource : IWebDriverSource
    {

        public IWebDriver Driver { get; set; }

        public ExplicitDriverSource(IWebDriver driver)
        {
            this.Driver = driver;
        }
    }

    internal class ConfigDriverSource : IWebDriverSource
    {

        public IWebDriver Driver 
        { 
            get
            {
                return this.config.Driver;
            } 
            set
            {
                this.config.Driver = value;
            } 
        }
        private readonly _SeleneSettings_ config;

        public ConfigDriverSource(_SeleneSettings_ config)
        {
            this.config = config;
        }
    }

    // TODO: consider implementing Browser as pure user oriented abstraction, 
    //       with no low level iwebdriverish things 
    // TODO: consider implementing IJavaScriptExecutor
    public class SeleneDriver : IWebDriver, ISearchContext, IDisposable, INavigation, SeleneContext
    {
        IWebDriverSource source;

        public IWebDriver Value {   // TODO: maybe other name? Driver over Value? else? Current over Value?
            get {
                return this.source.Driver;
            }

            set {
                this.source.Driver = value;
            }
        }
        private readonly _SeleneSettings_ config;

        internal SeleneDriver(_SeleneSettings_ config)
        : this(new ConfigDriverSource(config), config)
        {}

        public SeleneDriver(IWebDriverSource source)
        : this(source, Configuration.Shared)
        {}

        internal SeleneDriver(IWebDriverSource source, _SeleneSettings_ config)
        {
            this.source = source;
            this.config = config;
        }

        public SeleneDriver() 
        : this(Configuration.Shared) 
        {}

        public SeleneDriver(IWebDriver driver) 
        : this(Configuration._With_(driver: driver)) 
        {} 

        IWebDriver asWebDriver()
        {
            return this;
        }

        public override string ToString()
        {
            return "Browser";
        }

        public SeleneDriver With(
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

            return new SeleneDriver(
                this.config.With(customized)
            );
        }

        public SeleneDriver _With_(_SeleneSettings_ config)
        {
            return new SeleneDriver(
                config
            );
        }

        //
        // SDriver methods
        //

        // TODO: consider moving Element/Elements to SDriverExtensions, and leaving Find/FindAll here
        // becuase SDriver#Find sounds better than SDriver#Element (Element sounded when we had Browser#Element in the past...)
        public SeleneElement Find(By by)
        {
            return new SeleneElement(by, this.config);
        }

        public SeleneElement Find(string cssSelector)
        {
            return Find(By.CssSelector(cssSelector));
        }

        public SeleneElement Find(IWebElement pageFactoryElement)
        {
            return new SeleneElement(pageFactoryElement, this.config);
        }

        public SeleneCollection FindAll(By by)
        {
            return new SeleneCollection(by, this.config);
        }

        public SeleneCollection FindAll(string cssSelector)
        {
            return FindAll(By.CssSelector(cssSelector));
        }

        public SeleneCollection FindAll(IList<IWebElement> pageFactoryElements)
        {
            return new SeleneCollection(pageFactoryElements, this.config);
        }

        public Actions Actions()
        {
            return new Actions(this.Value);
        }

        public SeleneDriver Should(Condition<IWebDriver> condition) 
        {
            Selene.WaitFor(
                this.Value, 
                condition, 
                this.config.Timeout ?? Configuration.Timeout,
                this.config.PollDuringWaits ?? Configuration.PollDuringWaits
            );
            return this;
        }

        public bool Matching(Condition<IWebDriver> condition)
        {
            try 
            {
                condition.Invoke(this);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WaitUntil(Condition<IWebDriver> condition)
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
 
        //
        // INavigation methods
        //

        public void GoToUrl(string url)
        {
            asWebDriver().Navigate().GoToUrl(url);
        }

        public void GoToUrl(Uri url)
        {
            asWebDriver().Navigate().GoToUrl(url);
        }

        public void Back()
        {
            asWebDriver().Navigate().Back();
        }

        public void Forward()
        {
            asWebDriver().Navigate().Forward();
        }

        public void Refresh()
        {
            asWebDriver().Navigate().Refresh();
        }

        //
        // Some IWebDriver original properties and methods
        //

        public string Title {
            get {
                return asWebDriver().Title;
            }
        }

        public string Url {
            get {
                return asWebDriver().Url;
            }

            set {
                asWebDriver().Url = value;
            }
        }

        public void Close()
        {
            asWebDriver().Close();
        }

        public void Quite()
        {
            asWebDriver().Quit();
        }

        // TODO: consider refactor to correspondent ITargetLocator methods;
        public ITargetLocator SwitchTo()
        {
            return asWebDriver().SwitchTo();
        }

        //
        // Delegated Methods
        //

        //
        // Via IWebDriver interface
        //

        string IWebDriver.CurrentWindowHandle {
            get {
                return Value.CurrentWindowHandle;
            }
        }

        string IWebDriver.PageSource {
            get {
                return Value.PageSource;
            }
        }

        string IWebDriver.Title {
            get {
                return Value.Title;
            }
        }

        string IWebDriver.Url {
            get {
                return Value.Url;
            }

            set {
                Value.Url = value;
            }
        }

        ReadOnlyCollection<string> IWebDriver.WindowHandles {
            get {
                return Value.WindowHandles;
            }
        }

        void IWebDriver.Close ()
        {
            Value.Close();
        }

        IOptions IWebDriver.Manage ()
        {
            return Value.Manage();
        }

        INavigation IWebDriver.Navigate ()
        {
            return Value.Navigate();
        }

        void IWebDriver.Quit ()
        {
            Value.Quit();
        }

        ITargetLocator IWebDriver.SwitchTo ()
        {
            return Value.SwitchTo();
        }

        //
        // ISearchContext methods
        //

        IWebElement ISearchContext.FindElement (By by)
        {
            return new SeleneElement(by, this.config);
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements (By by)
        {
            return new SeleneCollection(by, this.config).ToReadOnlyWebElementsCollection();
        }

        //
        // SContext methods
        //

        IWebElement SeleneContext.FindElement (By by)
        {
            return this.Value.FindElement(by);
        }

        ReadOnlyCollection<IWebElement> SeleneContext.FindElements (By by)
        {
            return this.Value.FindElements(by);
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
            this.source.Driver.Dispose();
            if (!disposedValue) {
                if (disposing) {
                    // dispose managed state (managed objects).
                    this.source.Driver.Dispose();
                    /* TODO: is this.driver - managed or not managed? :D
                     * TODO: Do we actually need this code? Maybe the following will be enouth:
                    this.driver.Dispose()
                     */
                }

                //TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                //TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SDriver() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose ()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose (true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    namespace Support.Extensions
    {
        public static class SeleneDriverExtensions
        {
            public static void Open(this SeleneDriver driver, string url)
            {
                driver.GoToUrl(url);
            }

            public static void Open(this SeleneDriver driver, Uri url)
            {
                driver.GoToUrl(url);
            }

            // TODO: consider deprecating, since it make no sense to call it like driver.S ;)
            public static SeleneElement S(this SeleneDriver browser, By by)
            {
                return browser.Element(by);
            }

            public static SeleneElement S(this SeleneDriver browser, string cssSelector)
            {
                return browser.Element(cssSelector);
            }

            public static SeleneElement S(this SeleneDriver browser, IWebElement pageFactoryElement)
            {
                return browser.Element(pageFactoryElement);
            }

            public static SeleneCollection SS(this SeleneDriver browser, By by)
            {
                return browser.Elements(by);
            }

            public static SeleneCollection SS(this SeleneDriver browser, string cssSelector)
            {
                return browser.Elements(cssSelector);
            }

            public static SeleneCollection SS(this SeleneDriver browser, IList<IWebElement> pageFactoryElements)
            {
                return browser.Elements(pageFactoryElements);
            }

            public static SeleneElement Element(this SeleneDriver browser, By by)
            {
                return browser.Find(by);
            }

            public static SeleneElement Element(this SeleneDriver browser, string cssSelector)
            {
                return browser.Find(cssSelector);
            }

            public static SeleneElement Element(this SeleneDriver browser, IWebElement pageFactoryElement)
            {
                return browser.Find(pageFactoryElement);
            }

            public static SeleneCollection Elements(this SeleneDriver browser, By by)
            {
                return browser.FindAll(by);
            }

            public static SeleneCollection Elements(this SeleneDriver browser, string cssSelector)
            {
                return browser.FindAll(cssSelector);
            }

            public static SeleneCollection Elements(this SeleneDriver browser, IList<IWebElement> pageFactoryElements)
            {
                return browser.FindAll(pageFactoryElements);
            }
        }
    }

    // [Obsolete("Browser is deprecated and will be removed in next version, please use SeleneDriver class instead.")]
    // public class Browser_ : SeleneDriver
    // {
    //     public Browser_(IWebDriver driver) : base(new ExplicitDriverSource(driver)) {}      
    // }
}

