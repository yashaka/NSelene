using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Threading;
using NSelene.Conditions;
using OpenQA.Selenium.Interactions;

namespace NSelene
{
    // TODO: consider renaming Utils to Selene
    public static partial class Utils
    {

        [Obsolete("SetDriver is deprecated and will be removed in next version, please use Utils.SetWebDriver method instead.")]
        public static void SetDriver(IWebDriver driver)
        {
            PrivateConfiguration.SharedDriver.Value = driver;
        }

        public static void SetWebDriver(IWebDriver driver)
        {
            PrivateConfiguration.SharedDriver.Value = driver;
        }

        [Obsolete("GetDriver is deprecated and will be removed in next version, please use Utils.GetWebDriver method instead.")]
        public static IWebDriver GetDriver()
        {
            return PrivateConfiguration.SharedDriver.Value;
        }

        public static IWebDriver GetWebDriver()
        {
            return PrivateConfiguration.SharedDriver.Value;
        }

        public static object ExecuteScript(string script)
        {
            return (GetWebDriver() as IJavaScriptExecutor).ExecuteScript(script);
        }

        public static SElement S(By locator)
        {
            return new SElement(locator);
        }

        public static SElement S(string cssSelector)
        {
            return S(By.CssSelector(cssSelector));
        }

        public static SElement S(By locator, IWebDriver driver)
        {
            return new SElement(locator, new SDriver(driver));
        }

        public static SElement S(string cssSelector, IWebDriver driver)
        {
            return S(By.CssSelector(cssSelector), driver);
        }

        public static SCollection SS(By locator)
        {
            return new SCollection(locator);
        }

        public static SCollection SS(string cssSelector)
        {
            return SS(By.CssSelector(cssSelector));
        }

        public static SCollection SS(By locator, IWebDriver driver)
        {
            return new SCollection(locator, new SDriver(driver));
        }

        public static SCollection SS(string cssSelector, IWebDriver driver)
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

        public static string Url()
        {
            return GetWebDriver().Url;
        }

        [Obsolete("SActions method is deprecated and will be removed in next version, please use Utils.Actions property instead.")]
        public static Actions SActions()
        {
            return new Actions(GetWebDriver());
        }

        public static Actions Actions{
            get {
                return new Actions(GetWebDriver());
            }
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
            var clock = new SystemClock();
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
            var clock = new SystemClock();
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
    }
}
