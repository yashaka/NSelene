using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace NSelene
{
    public static class Configuration
    {
        static ThreadLocal<double> _timeout = new ThreadLocal<double>(() => 4);
        public static double Timeout
        {
            get
            {
                return _timeout.Value;
            }
            set
            {
                _timeout.Value = value;
            }
        }


        static ThreadLocal<double> _pollDuringWaits = (
            new ThreadLocal<double>(() => 0.1)
        );
        public static double PollDuringWaits
        {
            get
            {
                return _pollDuringWaits.Value;
            }
            set
            {
                _pollDuringWaits.Value = value;
            }
        }

        static ThreadLocal<bool> _setValueByJs = (
            new ThreadLocal<bool>(() => false)
        );
        public static bool SetValueByJs
        {
            get
            {
                return _setValueByJs.Value;
            }
            set
            {
                _setValueByJs.Value = value;
            }
        }

        static ThreadLocal<IWebDriver> _webDriver = (
            new ThreadLocal<IWebDriver>(
                () => new ChromeDriver()
                /* // TODO: add automatic install?
                () => {
                    new DriverManager().SetUpDriver(new ChromeConfig(), version: chromeVersion);
                    return new ChromeDriver();
                }
                 */
            )
        );
        public static IWebDriver WebDriver
        {
            get
            {
                return _webDriver.Value;
            }
            set
            {
                _webDriver.Value = value;
            }
        }
    }
}