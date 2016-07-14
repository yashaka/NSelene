using System;
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
}

