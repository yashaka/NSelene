using System;
using System.Collections.ObjectModel;
using System.Threading;
using OpenQA.Selenium;

namespace NSelene
{

    public interface DriverProvider
    {
        IWebDriver Driver { get; }
        //IWebDriver GetDriver();
    }

    public interface DriverKeeper
    {
        //IWebDriver Driver { set; }
        void SetDriver(IWebDriver driver);
    }

    public interface DriverStorage : DriverProvider, DriverKeeper
    {
    }

    public class WrappedDriver : DriverProvider
    {
        readonly IWebDriver driver;

        public WrappedDriver(IWebDriver driver)
        {
            this.driver = driver;
        }

        public IWebDriver Driver {
            get {
                return this.driver;
            }
        }
    }

    public class ThreadLocalDriverStorage : DriverProvider, DriverStorage
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

        public void SetDriver(IWebDriver driver)
        {
            this.driver.Value = driver;
        }
    }

    public class SharedThreadLocalDriver : IWebDriver, ISearchContext, IDisposable
    {
        // todo: move it out from here, so this class can be reused by somebody else... ?
        public static SharedThreadLocalDriver Instance = new SharedThreadLocalDriver();

        ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();

        public IWebDriver Value {   // TODO: maybe other name? Driver over Value? else? Current over Value?
            get {
                return this.driver.Value;
            }

            set {
                this.driver.Value = value;
            }
        }

        //
        // Delegated Methods
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
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements (By by)
        {
            return Value.FindElements(by);
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
}

