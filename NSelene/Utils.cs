using System;
using OpenQA.Selenium;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Globalization;
using System.Threading;
using NSelene.Conditions;
using OpenQA.Selenium.Interactions;

namespace NSelene
{

    public static class Config
    {
        public static double Timeout = 4;
        public static double PollDuringWaits = 0.1;
    }

    public static partial class Utils
    {

        static IDictionary<int, IWebDriver> _drivers = new Dictionary<int, IWebDriver>();

        public static void SetDriver(IWebDriver driver)
        {
            var code = Thread.CurrentThread.GetHashCode();

            if (_drivers.ContainsKey(code)) 
            {
                _drivers[code] = driver;
            } else 
            {
                _drivers.Add(code, driver);
            }
        }

        public static IWebDriver GetDriver()
        {
            return _drivers[Thread.CurrentThread.GetHashCode ()];
        }

        public static object ExecuteScript(string script)
        {
            return (GetDriver() as IJavaScriptExecutor).ExecuteScript(script);
        }

        public static IWebElement Find(By locator)
        {
            return GetDriver().FindElement(locator);
        }

        public static IReadOnlyCollection<IWebElement> FindAll(By locator)
        {
            return GetDriver().FindElements(locator);
        }

        public static void Open(string url)
        {
            GetDriver().Navigate().GoToUrl(url);
        }

        public static string Url()
        {
            return GetDriver().Url;
        }

        public static Actions SActions()
        {
            return new Actions(GetDriver());
        }

        public static TResult WaitFor<TResult>(TResult sEntity, Condition<TResult> condition)
        {
            return WaitFor(sEntity, condition, Config.Timeout);
        }

        public static TResult WaitForNot<TResult>(TResult sEntity, Condition<TResult> condition)
        {
            return WaitForNot(sEntity, condition, Config.Timeout);
        }

        public static TResult WaitFor<TResult>(TResult sEntity, Condition<TResult> condition, double timeout)
        {
            Exception lastException = null;
            var clock = new SystemClock();
            var timeoutSpan = TimeSpan.FromSeconds(timeout);
            DateTime otherDateTime = clock.LaterBy(timeoutSpan);
            var ignoredExceptionTypes = new [] { typeof(WebDriverException), typeof(IndexOutOfRangeException) };
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
                Thread.Sleep(TimeSpan.FromSeconds(Config.PollDuringWaits).Milliseconds);
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
                Thread.Sleep(TimeSpan.FromSeconds(Config.PollDuringWaits).Milliseconds);
            }
            return sEntity;
        }


    }
}
