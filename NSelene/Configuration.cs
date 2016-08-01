using System;
using OpenQA.Selenium;

namespace NSelene
{

    static class PrivateConfiguration 
    {
        public static SeleneDriver SharedDriver = new SeleneDriver();
    }

    public static class Configuration
    {
        // TODO: consider making Timeout and PollDuringWaits also threadlocal
        public static double Timeout = 4;
        public static double PollDuringWaits = 0.1;

        public static IWebDriver WebDriver {
            get {
                return PrivateConfiguration.SharedDriver.Value;
            }
            set {
                PrivateConfiguration.SharedDriver.Value = value;
            }
        }
    }

    [Obsolete("Config class is deprecated and will be removed in next version, please use Configuration static class instead.")]
    public static class Config
    {
        public static double Timeout{
            get {
                return Configuration.Timeout;
            }
            set {
                Configuration.Timeout = value;
            }
        }

        public static double PollDuringWaits{
            get {
                return Configuration.PollDuringWaits;
            }
            set {
                Configuration.PollDuringWaits = value;
            }
        }
    }
}

