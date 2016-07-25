using System;
using OpenQA.Selenium;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Globalization;
using System.Threading;
using NSelene.Conditions;
using OpenQA.Selenium.Interactions;
using System.Reflection;

namespace NSelene
{
    /*
    public sealed class Browser
    {
        readonly IWebDriver driver;

        public Browser(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void Open(string url)
        {
            this.driver.Navigate().GoToUrl(url);
        }

        public SElement Find(By locator)
        {
            return new SElement(locator, driver);
        }

        // TODO: consider making the only availabe naming - Browser.Element over I.Find, and leave the "I.Find" option for self-impl by users
        // hm... but it would be harder to implement... actually users will be able to implement fastly
        // only Find as Browser.Element instead of I.Find as Browser.Element :(
        // seems like better to leave both...
        public SElement Element(By locator)
        {
            return Find(locator);
        }

        public SElement Find(string cssSelector)
        {
            return this.Find(By.CssSelector(cssSelector));
        }

        public SElement Element(string cssSelector)
        {
            return Find(cssSelector);
        }

        public SElement Find(IWebElement pageFactoryElement)
        {
            return new SElement(pageFactoryElement, driver);
        }

        public SElement Element(IWebElement pageFactoryElement)
        {
            return Find(pageFactoryElement);
        }

        public SCollection FindAll(By locator)
        {
            return new SCollection(locator, driver);
        }

        public SCollection Elements(By locator)
        {
            return FindAll(locator);
        }

        public SCollection FindAll(string cssSelector)
        {
            return this.FindAll(By.CssSelector(cssSelector));
        }

        public SCollection Elements(string cssSelector)
        {
            return FindAll(cssSelector);
        }

        public SCollection FindAll(IList<IWebElement> pageFactoryElements)
        {
            return new SCollection(pageFactoryElements, this.driver);
        }

        public SCollection Elements(IList<IWebElement> pageFactoryElements)
        {
            return FindAll(pageFactoryElements);
        }

        public object ExecuteScript(string script)
        {
            return (this.driver as IJavaScriptExecutor).ExecuteScript(script);
        }

        public string GetCurrentUrl()
        {
            return this.driver.Url;
        }

        public string Url
        {
            get {
                return GetCurrentUrl();
            }
        }

        public Actions Do()
        {
            return new Actions(this.driver);
        }

        public  TResult WaitFor<TResult>(TResult sEntity, Condition<TResult> condition)
        {
            return Utils.WaitFor(sEntity, condition, Configuration.Timeout);
        }

        public  TResult WaitFor<TResult>(TResult sEntity, Condition<TResult> condition, double timeout)
        {
            return Utils.WaitFor(sEntity, condition, timeout);
        }

        public  TResult WaitForNot<TResult>(TResult sEntity, Condition<TResult> condition)
        {
            return Utils.WaitForNot(sEntity, condition, Configuration.Timeout);
        }

        public  TResult WaitForNot<TResult>(TResult sEntity, Condition<TResult> condition, double timeout)
        {
            return Utils.WaitForNot(sEntity, condition, timeout);
        }
    }
     */

    public static partial class Utils
    {

        //[Obsolete("SetDriver is deprecated and will be removed in next version, please use Configuration.SetWebDriver method instead.")]
        public static void SetDriver(IWebDriver driver)
        {
            PrivateConfiguration.SharedDriver.Value = driver;
        }

        public static void SetWebDriver(IWebDriver driver)
        {
            PrivateConfiguration.SharedDriver.Value = driver;
        }

        //[Obsolete("GetDriver is deprecated and will be removed in next version, please use Configuration.GetWebDriver method instead.")]
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
            return (Configuration.GetWebDriver() as IJavaScriptExecutor).ExecuteScript(script);
        }

        public static SElement S(By locator)
        {
            return new SElement(locator);
        }

        public static SElement S(string cssSelector)
        {
            return S(By.CssSelector(cssSelector));
        }

        public static SCollection SS(By locator)
        {
            return new SCollection(locator);
        }

        public static SCollection SS(string cssSelector)
        {
            return SS(By.CssSelector(cssSelector));
        }

        public static IWebElement FindElement(By locator)
        {
            return Configuration.GetWebDriver().FindElement(locator);
        }

        public static IReadOnlyCollection<IWebElement> FindElements(By locator)
        {
            return Configuration.GetWebDriver().FindElements(locator);
        }

        public static void Open(string url)
        {
            Configuration.GetWebDriver().Navigate().GoToUrl(url);
        }

        public static string Url()
        {
            return Configuration.GetWebDriver().Url;
        }

        public static Actions SActions()
        {
            return new Actions(Configuration.GetWebDriver());
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
