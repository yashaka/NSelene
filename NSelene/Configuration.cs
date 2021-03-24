using System.Threading;
using OpenQA.Selenium;

namespace NSelene
{
    internal static class PrivateConfiguration  // todo: consider renaming
    {
        public static SeleneDriver SharedDriver = new SeleneDriver();
    }

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

        public static IWebDriver WebDriver
        {
            get
            {
                // todo: simplify the impl to same style as above... 
                return PrivateConfiguration.SharedDriver.Value;
            }
            set
            {
                PrivateConfiguration.SharedDriver.Value = value;
            }
        }
    }
}