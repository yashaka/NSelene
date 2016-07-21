using System;
using OpenQA.Selenium;

namespace NSelene
{

    static class PrivateConfiguration 
    {
        public static SDriver SharedDriver = new SDriver();
    }

    public static class Configuration
    {
        // TODO: consider making Timeout and PollDuringWaits also threadlocal
        public static double Timeout = 4;
        public static double PollDuringWaits = 0.1;

        // TODO: consider making it properties and left same methods in Utils
        // because it's pretty unhandy to use Configuration.GetWebDriver over GetWebDriver
        // while using static for Configuration is not good idea, because Timeout and PllDuringWaits better to use via fully qualified name...
        public static IWebDriver GetWebDriver()
        {
            return PrivateConfiguration.SharedDriver.Value;
        }

        public static void SetWebDriver(IWebDriver value)
        {
            PrivateConfiguration.SharedDriver.Value = value;
        }
    }
}

