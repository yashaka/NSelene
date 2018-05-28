using System;
using OpenQA.Selenium;
using System.Linq;
using System.Threading;
using NSelene.Conditions;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;

namespace NSelene
{

    // TODO: consider renaming Utils to Selene
    public static partial class Selene
    {
        public static void SetWebDriver(IWebDriver driver)
        {
            PrivateConfiguration.SharedDriver.Value = driver;
        }

        public static IWebDriver GetWebDriver()
        {
            return PrivateConfiguration.SharedDriver.Value;
        }

        public static object ExecuteScript(string script)
        {
            return (GetWebDriver() as IJavaScriptExecutor).ExecuteScript(script);
        }

        public static SeleneElement S(By locator)
        {
            return new SeleneElement(locator);
        }

        public static SeleneElement S(string cssSelector)
        {
            return S(By.CssSelector(cssSelector));
        }

        public static SeleneElement S(IWebElement pageFactoryElement, IWebDriver driver)
        {
            return new SeleneElement(pageFactoryElement, driver);
        }

        public static SeleneElement S(By locator, IWebDriver driver)
        {
            return new SeleneElement(locator, new SeleneDriver(driver));
        }

        public static SeleneElement S(string cssSelector, IWebDriver driver)
        {
            return S(By.CssSelector(cssSelector), driver);
        }

        public static SeleneCollection SS(By locator)
        {
            return new SeleneCollection(locator);
        }

        public static SeleneCollection SS(string cssSelector)
        {
            return SS(By.CssSelector(cssSelector));
        }

        public static SeleneCollection SS(IList<IWebElement> pageFactoryElementsList, IWebDriver driver)
        {
            return new SeleneCollection(pageFactoryElementsList, driver);
        }

        public static SeleneCollection SS(By locator, IWebDriver driver)
        {
            return new SeleneCollection(locator, new SeleneDriver(driver));
        }

        public static SeleneCollection SS(string cssSelector, IWebDriver driver)
        {
            return SS(By.CssSelector(cssSelector), driver);
        }

        public static void Open(string url)
        {
            GoToUrl(url);
        }

        public static void GoToUrl(string url)
        {
            GetWebDriver().Navigate().GoToUrl(url);
        }

        // TODO: consider changing to static property
        public static string Url()
        {
            return GetWebDriver().Url;
        }

        public static Actions Actions{
            get {
                return new Actions(GetWebDriver());
            }
        }

        public static IWebDriver WaitTo(Condition<IWebDriver> condition) {
            return WaitFor(GetWebDriver(), condition);
        }

        public static TResult WaitFor<TResult>(TResult sEntity, Condition<TResult> condition)
        {
            return WaitFor(sEntity, condition, Configuration.Timeout);
        }

        public static TResult WaitForNot<TResult>(TResult sEntity, Condition<TResult> condition)
        {
            return WaitForNot(sEntity, condition, Configuration.Timeout);
        }

        public static TResult WaitFor<TResult>(TResult sEntity, Condition<TResult> condition, double timeout)
        {
            Exception lastException = null;
            var clock = new OpenQA.Selenium.Support.UI.SystemClock();
            var timeoutSpan = TimeSpan.FromSeconds(timeout);
            DateTime otherDateTime = clock.LaterBy(timeoutSpan);
            var ignoredExceptionTypes = new [] { 
                typeof(WebDriverException), 
                typeof(IndexOutOfRangeException),
                typeof(ArgumentOutOfRangeException)
            };
            while (true)
            {
                try
                {
                    if (condition.Apply(sEntity))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (!ignoredExceptionTypes.Any(type => type.IsInstanceOfType(ex)))
                    {
                        throw;
                    }
                    lastException = ex;
                }
                if (!clock.IsNowBefore(otherDateTime))
                {
                    string text = string.Format("\nTimed out after {0} seconds \nwhile waiting entity with locator: {1} \nfor condition: "
                                                , timeoutSpan.TotalSeconds
                                                , sEntity
                                               );
                    text = text + condition;
                    throw new WebDriverTimeoutException(text, lastException);
                }
                Thread.Sleep(TimeSpan.FromSeconds(Configuration.PollDuringWaits).Milliseconds);
            }
            return sEntity;
        }

        public static TResult WaitForNot<TResult>(TResult sEntity, Condition<TResult> condition, double timeout)
        {
            Exception lastException = null;
            var clock = new OpenQA.Selenium.Support.UI.SystemClock();
            var timeoutSpan = TimeSpan.FromSeconds(timeout);
            DateTime otherDateTime = clock.LaterBy(timeoutSpan);
//            var ignoredExceptionTypes = new [] { typeof(WebDriverException), typeof(IndexOutOfRangeException) };
            while (true)
            {
                try
                {
                    if (!condition.Apply(sEntity))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    break;
//                    if (!ignoredExceptionTypes.Any(type => type.IsInstanceOfType(ex)))
//                    {
//                        throw;
//                    }
                }
                if (!clock.IsNowBefore(otherDateTime))
                {
                    string text = string.Format( "\nTimed out after {0} seconds \nwhile waiting entity with locator: {1}\nfor condition: not "
                                               , timeoutSpan.TotalSeconds, sEntity
                                               );
                    text = text + condition;
                    throw new WebDriverTimeoutException(text, lastException);
                }
                Thread.Sleep(TimeSpan.FromSeconds(Configuration.PollDuringWaits).Milliseconds);
            }
            return sEntity;
        }

        //
        // Obsolete 
        //

        [Obsolete("SetDriver is deprecated and will be removed in next version, please use Utils.SetWebDriver method instead.")]
        public static void SetDriver(IWebDriver driver)
        {
            PrivateConfiguration.SharedDriver.Value = driver;
        }

        [Obsolete("GetDriver is deprecated and will be removed in next version, please use Utils.GetWebDriver method instead.")]
        public static IWebDriver GetDriver()
        {
            return PrivateConfiguration.SharedDriver.Value;
        }

        [Obsolete("SActions method is deprecated and will be removed in next version, please use Utils.Actions property instead.")]
        public static Actions SActions()
        {
            return new Actions(GetWebDriver());
        }
    }

    [Obsolete("NSelene.Utils class is deprecated and will be removed in next version, please use NSelene.Selene class instead.")]
    public static class Utils 
    {

        [Obsolete("SetDriver is deprecated and will be removed in next version, please use Selene.SetWebDriver method instead.")]
        public static void SetDriver(IWebDriver driver)
        {
            PrivateConfiguration.SharedDriver.Value = driver;
        }

        [Obsolete("Utils.SetWebDriver is deprecated and will be removed in next version, please use Selene.SetWebDriver method instead.")]
        public static void SetWebDriver(IWebDriver driver)
        {
            PrivateConfiguration.SharedDriver.Value = driver;
        }

        [Obsolete("Utils.GetDriver is deprecated and will be removed in next version, please use Selene.GetWebDriver method instead.")]
        public static IWebDriver GetDriver()
        {
            return PrivateConfiguration.SharedDriver.Value;
        }

        [Obsolete("Utils.GetWebDriver is deprecated and will be removed in next version, please use Selene.GetWebDriver method instead.")]
        public static IWebDriver GetWebDriver()
        {
            return PrivateConfiguration.SharedDriver.Value;
        }

        [Obsolete("Utils.ExecuteScript is deprecated and will be removed in next version, please use Selene.ExecuteScript method instead.")]
        public static object ExecuteScript(string script)
        {
            return (GetWebDriver() as IJavaScriptExecutor).ExecuteScript(script);
        }

        [Obsolete("Utils.S is deprecated and will be removed in next version, please use Selene.S method instead.")]
        public static SeleneElement S(By locator)
        {
            return new SeleneElement(locator);
        }

        [Obsolete("Utils.S is deprecated and will be removed in next version, please use Selene.S method instead.")]
        public static SeleneElement S(string cssSelector)
        {
            return S(By.CssSelector(cssSelector));
        }

        [Obsolete("Utils.S is deprecated and will be removed in next version, please use Selene.S method instead.")]
        public static SeleneElement S(By locator, IWebDriver driver)
        {
            return new SeleneElement(locator, new SeleneDriver(driver));
        }

        [Obsolete("Utils.S is deprecated and will be removed in next version, please use Selene.S method instead.")]
        public static SeleneElement S(string cssSelector, IWebDriver driver)
        {
            return S(By.CssSelector(cssSelector), driver);
        }

        [Obsolete("Utils.SS is deprecated and will be removed in next version, please use Selene.SS method instead.")]
        public static SeleneCollection SS(By locator)
        {
            return new SeleneCollection(locator);
        }

        [Obsolete("Utils.SS is deprecated and will be removed in next version, please use Selene.SS method instead.")]
        public static SeleneCollection SS(string cssSelector)
        {
            return SS(By.CssSelector(cssSelector));
        }

        [Obsolete("Utils.SS is deprecated and will be removed in next version, please use Selene.SS method instead.")]
        public static SeleneCollection SS(By locator, IWebDriver driver)
        {
            return new SeleneCollection(locator, new SeleneDriver(driver));
        }

        [Obsolete("Utils.SS is deprecated and will be removed in next version, please use Selene.SS method instead.")]
        public static SeleneCollection SS(string cssSelector, IWebDriver driver)
        {
            return SS(By.CssSelector(cssSelector), driver);
        }

        [Obsolete("Utils.Open is deprecated and will be removed in next version, please use Selene.Open method instead.")]
        public static void Open(string url)
        {
            GoToUrl(url);
        }

        [Obsolete("Utils.GoToUrl is deprecated and will be removed in next version, please use Selene.GoToUrl method instead.")]
        public static void GoToUrl(string url)
        {
            GetWebDriver().Navigate().GoToUrl(url);
        }

        [Obsolete("Utils.Url is deprecated and will be removed in next version, please use Selene.Url method instead.")]
        public static string Url()
        {
            return GetWebDriver().Url;
        }

        [Obsolete("SActions method is deprecated and will be removed in next version, please use Selene.Actions property instead.")]
        public static Actions SActions()
        {
            return new Actions(GetWebDriver());
        }
    }
}
