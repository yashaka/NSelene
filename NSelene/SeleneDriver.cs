﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.IO;

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

        public SeleneDriver(IWebDriverSource source)
        {
            this.source = source;
        }

        public SeleneDriver() : this(SharedThreadLocalDriverSource.Instance) {}

        public SeleneDriver(IWebDriver driver) : this(new ExplicitDriverSource(driver)) {} 

        IWebDriver asWebDriver()
        {
            return this;
        }

        //
        // SDriver methods
        //

        // TODO: consider moving Element/Elements to SDriverExtensions, and leaving Find/FindAll here
        // becuase SDriver#Find sounds better than SDriver#Element (Element sounded when we had Browser#Element in the past...)
        public SeleneElement Find(By by)
        {
            return new SeleneElement(by, this);
        }

        public SeleneElement Find(string cssSelector)
        {
            return Find(By.CssSelector(cssSelector));
        }

        public SeleneElement Find(IWebElement pageFactoryElement)
        {
            return new SeleneElement(pageFactoryElement, this);
        }

        public SeleneCollection FindAll(By by)
        {
            return new SeleneCollection(by, this);
        }

        public SeleneCollection FindAll(string cssSelector)
        {
            return FindAll(By.CssSelector(cssSelector));
        }

        public SeleneCollection FindAll(IList<IWebElement> pageFactoryElements)
        {
            return new SeleneCollection(pageFactoryElements, this);
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
            return new SeleneElement(by, this);
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements (By by)
        {
            return new SeleneCollection(by, this).ToReadOnlyWebElementsCollection();
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

            public static void TakeScreenshot(string path = @"screens")
            {
                IWebDriver driver = Selene.GetWebDriver();
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                string fileName = path + @"\" + DateTime.Now.ToString("screen_hh_mm_ss") + ".png";

                //workaround
                var dir = Path.GetDirectoryName(typeof(Selene).Assembly.Location);
                Directory.SetCurrentDirectory(dir);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);
            }

            public static void TakeScreenshot(this SeleneDriver driver, string path = @"screens")
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                string fileName = path + @"\" + DateTime.Now.ToString("screen_hh_mm_ss") + ".png";

                //workaround
                var dir = Path.GetDirectoryName(typeof(Selene).Assembly.Location);
                Directory.SetCurrentDirectory(dir);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);
            }
        }
    }

    [Obsolete("Browser is deprecated and will be removed in next version, please use SDriver class instead.")]
    public class Browser : SeleneDriver
    {
        public Browser(IWebDriver driver) : base(new ExplicitDriverSource(driver)) {}      
    }
}

