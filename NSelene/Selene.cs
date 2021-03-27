using System;
using OpenQA.Selenium;
using System.Linq;
using System.Threading;
using NSelene.Conditions;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;

namespace NSelene
{
    public static partial class Selene
    {
        internal static SeleneDriver SharedDriver = new SeleneDriver();

        public static void SetWebDriver(IWebDriver driver)
        {
            Configuration.WebDriver = driver;
        }

        public static IWebDriver GetWebDriver()
        {
            return Configuration.WebDriver;
        }
        public static object ExecuteScript(string script)
        {
            return (GetWebDriver() as IJavaScriptExecutor).ExecuteScript(script);
        }

        public static SeleneElement S(By locator)
        {
            return new SeleneElement(locator);
        }

        public static SeleneElement S(string cssOrXPathSelector)
        {
            return S(Utils.ToBy(cssOrXPathSelector));
        }

        public static SeleneElement S(IWebElement pageFactoryElement, IWebDriver driver)
        {
            return new SeleneElement(pageFactoryElement, driver);
        }

        public static SeleneElement S(By locator, IWebDriver driver)
        {
            return new SeleneElement(locator, new SeleneDriver(driver));
        }

        public static SeleneElement S(string cssOrXPathSelector, IWebDriver driver)
        {
            return S(Utils.ToBy(cssOrXPathSelector), driver);
        }

        public static SeleneCollection SS(By locator)
        {
            return new SeleneCollection(locator);
        }

        public static SeleneCollection SS(string cssOrXPathSelector)
        {
            return SS(Utils.ToBy(cssOrXPathSelector));
        }

        public static SeleneCollection SS(IList<IWebElement> pageFactoryElementsList, IWebDriver driver)
        {
            return new SeleneCollection(pageFactoryElementsList, driver);
        }

        public static SeleneCollection SS(By locator, IWebDriver driver)
        {
            return new SeleneCollection(locator, new SeleneDriver(driver));
        }

        public static SeleneCollection SS(string cssOrXPathSelector, IWebDriver driver)
        {
            return SS(Utils.ToBy(cssOrXPathSelector), driver);
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

        public static TResult WaitFor<TResult>(TResult entity, Condition<TResult> condition, double timeout)
        {
            Exception lastException = null;
            var timeoutSpan = TimeSpan.FromSeconds(timeout);
            var otherDateTime = DateTime.Now.Add(timeoutSpan);
            var ignoredExceptionTypes = new [] { 
                typeof(WebDriverException), 
                typeof(IndexOutOfRangeException),
                typeof(ArgumentOutOfRangeException)
            };
            while (true)
            {
                try
                {
                    if (condition.Apply(entity))
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
                if (!(DateTime.Now < otherDateTime))
                {
                    string text = $"\nTimed out after {timeoutSpan.TotalSeconds} seconds " +
                                  $"\nwhile waiting entity with locator: {entity} " +
                                  $"\nfor condition: {condition}";
                    throw new WebDriverTimeoutException(text, lastException);
                }
                Thread.Sleep(TimeSpan.FromSeconds(Configuration.PollDuringWaits).Milliseconds);
            }
            return entity;
        }

        public static TResult WaitForNot<TResult>(TResult entity, Condition<TResult> condition, double timeout)
        {
            Exception lastException = null;
            var timeoutSpan = TimeSpan.FromSeconds(timeout);
            var otherDateTime = DateTime.Now.Add(timeoutSpan);
//            var ignoredExceptionTypes = new [] { typeof(WebDriverException), typeof(IndexOutOfRangeException) };
            while (true)
            {
                try
                {
                    if (!condition.Apply(entity))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex; // todo: probably we don't need it...
                    break;
//                    if (!ignoredExceptionTypes.Any(type => type.IsInstanceOfType(ex)))
//                    {
//                        throw;
//                    }
                }
                if (!(DateTime.Now < otherDateTime))
                {
                    string text = string.Format( "\nTimed out after {0} seconds \nwhile waiting entity with locator: {1}\nfor condition: not "
                                               , timeoutSpan.TotalSeconds, entity
                                               );
                    text = text + condition;
                    throw new WebDriverTimeoutException(text, lastException);
                }
                Thread.Sleep(TimeSpan.FromSeconds(Configuration.PollDuringWaits).Milliseconds);
            }
            return entity;
        }
    }
}
