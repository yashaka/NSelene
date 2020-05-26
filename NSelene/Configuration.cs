using OpenQA.Selenium;

namespace NSelene
{
    internal static class PrivateConfiguration  // todo: consider renaming
    {
        public static SeleneDriver SharedDriver = new SeleneDriver();
    }

    public static class Configuration
    {
        // TODO: consider making Timeout and PollDuringWaits also threadlocal
        public static double Timeout = 4;
        public static double PollDuringWaits = 0.1;

        public static bool SetValueByJs = false;

        public static IWebDriver WebDriver {
            get {
                return PrivateConfiguration.SharedDriver.Value;
            }
            set {
                PrivateConfiguration.SharedDriver.Value = value;
            }
        }
    }
}