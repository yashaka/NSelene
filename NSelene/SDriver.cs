using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NSelene
{

    public interface IWebDriverSource : IDisposable
    {
        IWebDriver Driver { get; set; }
    }

    public class SharedThreadLocalDriverSource : IWebDriverSource, IDisposable
    {

        ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();

        public IWebDriver Driver {
            get {
                return this.driver.Value;
            }

            set {
                this.driver.Value = value;
            }
        }

        public static SharedThreadLocalDriverSource Instance = new SharedThreadLocalDriverSource();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    // dispose managed state (managed objects).
                    foreach(IWebDriver local in this.driver.Values)
                    {
                        local.Quit();
                    }
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

    public class ExplicitDriverSource : IWebDriverSource, IDisposable
    {

        public IWebDriver Driver { get; set; }

        public ExplicitDriverSource(IWebDriver driver)
        {
            this.Driver = driver;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    // dispose managed state (managed objects).
                    this.Driver.Quit();
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

    //TODO: consider implementing IJavaScriptExecutor
    public class SDriver : IWebDriver, ISearchContext, IDisposable, INavigation
    {
        IWebDriverSource source;

        public SDriver(IWebDriverSource source)
        {
            this.source = source;
        }

        public SDriver() : this(SharedThreadLocalDriverSource.Instance) {}

        public SDriver(IWebDriver driver) : this(new ExplicitDriverSource(driver)) {} 

        public IWebDriver Value {   // TODO: maybe other name? Driver over Value? else? Current over Value?
            get {
                return this.source.Driver;
            }

            set {
                this.source.Driver = value;
            }
        }

        IWebDriver asWebDriver()
        {
            return (IWebDriver) this;
        }

        //
        // SDriver methods
        //

        public SElement Element(By by)
        {
            return new SElement(by, this);
        }

        public SElement Element(string cssSelector)
        {
            return Element(By.CssSelector(cssSelector));
        }

        public SElement Element(IWebElement pageFactoryElement)
        {
            return new SElement(pageFactoryElement, this);
        }

        public SCollection Elements(By by)
        {
            return new SCollection(by, this);
        }

        public SCollection Elements(string cssSelector)
        {
            return Elements(By.CssSelector(cssSelector));
        }

        public SCollection Elements(IList<IWebElement> pageFactoryElements)
        {
            return new SCollection(pageFactoryElements, this);
        }

        // TODO: this method works with driver's value, not source... this make it possibly not thread safe... 
        public Actions Actions()
        {
            return new Actions(this.Value);
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

        IWebElement ISearchContext.FindElement (By by)
        {
            return Value.FindElement(by);
            //return new SElement(by, this);
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements (By by)
        {
            return Value.FindElements(by);
            //return new SCollection(by, this).ToReadOnlyWebElementsCollection();
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
            this.source.Dispose();
            if (!disposedValue) {
                if (disposing) {
                    // dispose managed state (managed objects).
                    this.source.Dispose();
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

    public static class SdriverExtensions
    {
        public static void Open(this SDriver driver, string url)
        {
            driver.GoToUrl(url);
        }

        public static void Open(this SDriver driver, Uri url)
        {
            driver.GoToUrl(url);
        }

        //
        // TODO: Should we remove them?
        //

        public static SElement Find(this SDriver browser, By by)
        {
            return browser.Element(by);
        }

        public static SElement Find(this SDriver browser, string cssSelector)
        {
            return browser.Element(cssSelector);
        }

        public static SElement Find(this SDriver browser, IWebElement pageFactoryElement)
        {
            return browser.Element(pageFactoryElement);
        }

        public static SCollection FindAll(this SDriver browser, By by)
        {
            return browser.Elements(by);
        }

        public static SCollection FindAll(this SDriver browser, string cssSelector)
        {
            return browser.Elements(cssSelector);
        }

        public static SCollection FindAll(this SDriver browser, IList<IWebElement> pageFactoryElements)
        {
            return browser.Elements(pageFactoryElements);
        }
    }

    [Obsolete("Browser is deprecated and will be removed in next version, please use SDriver class instead.")]
    public class Browser : SDriver
    {
        public Browser(IWebDriver driver) : base(new ExplicitDriverSource(driver)) {}      
    }
}

