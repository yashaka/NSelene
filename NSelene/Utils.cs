using System;
using OpenQA.Selenium;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Globalization;
using System.Threading;
using NSelene.Conditions;

namespace NSelene
{

    public static class Config
    {
        public static double Timeout = 4;
        public static double PollDuringWaits = 0.1;
    }

    public static partial class Utils
    {
        static IWebDriver _driver = null;

        public static void SetDriver(IWebDriver driver)
        {
            _driver = driver;
        }

        public static IWebDriver GetDriver()
        {
            return _driver;
        }

        public static object ExecuteScript(string script)
        {
            return (GetDriver() as IJavaScriptExecutor).ExecuteScript(script);
        }

        public static IWebElement Find(By locator)
        {
            return _driver.FindElement(locator);
        }

        public static IReadOnlyCollection<IWebElement> FindAll(By locator)
        {
            return _driver.FindElements(locator);
        }

        public static void Open(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }

        public static string Url()
        {
            return _driver.Url;
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
                    string text = string.Format(CultureInfo.InvariantCulture, "\nTimed out after {0} seconds \n while waiting for condition: ", new object[]
                        {
                            timeoutSpan.TotalSeconds
                        });
                    text = text + ": " + condition;
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
                    string text = string.Format(CultureInfo.InvariantCulture, "\nTimed out after {0} seconds \n while waiting for NOT condition: ", new object[]
                        {
                            timeoutSpan.TotalSeconds
                        });
                    text = text + ": " + condition;
                    throw new WebDriverTimeoutException(text, lastException);
                }
                Thread.Sleep(TimeSpan.FromSeconds(Config.PollDuringWaits).Milliseconds);
            }
            return sEntity;
        }


    }
}
