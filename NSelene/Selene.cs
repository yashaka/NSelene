using System;
using OpenQA.Selenium;
using System.Linq;
using System.Threading;
using NSelene.Conditions;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NSelene
{
    public static partial class Selene
    {

        public static void SetWebDriver(IWebDriver driver)
        {
            Configuration.Driver = driver;
        }

        public static IWebDriver GetWebDriver()
        {
            return Configuration.Driver;
        }
        
        /// Summary:
        ///   Might return null if not casted to IJavaScriptExecutor, 
        ///   e.g. for some Appium Driver instance
        ///   TODO: should we return null here or something else? 
        ///         (ExecuteScript can return null on its own... 
        ///         do we need to distinguish such cases?)
        public static object ExecuteScript(string script)
        {
            return (GetWebDriver() as IJavaScriptExecutor)?.ExecuteScript(script);
        }

        public static SeleneElement S(By locator)
        {
            return new SeleneElement(locator, Configuration.Shared);
        }

        public static SeleneElement S(string cssOrXPathSelector)
        {
            return S(Utils.ToBy(cssOrXPathSelector));
        }

        public static SeleneElement S(IWebElement pageFactoryElement, IWebDriver driver)
        {
            return new SeleneElement(
                pageFactoryElement, Configuration._With_(driver: driver)
            );
        }

        public static SeleneElement S(By locator, IWebDriver driver)
        {
            return new SeleneElement(locator, Configuration._With_(driver: driver));
        }

        public static SeleneElement S(string cssOrXPathSelector, IWebDriver driver)
        {
            return S(Utils.ToBy(cssOrXPathSelector), driver);
        }

        public static SeleneCollection SS(By locator)
        {
            return new SeleneCollection(locator, Configuration.Shared);
        }

        // TODO: consider:
        // public static SeleneCollection All(By locator)
        // {
        //     return SS(locator);
        // }
        // AND:
        // public static SeleneCollection All(string cssOrXPathSelector)
        // {
        //     return SS(Utils.ToBy(cssOrXPathSelector));
        // }

        public static SeleneCollection SS(string cssOrXPathSelector)
        {
            return SS(Utils.ToBy(cssOrXPathSelector));
        }

        public static SeleneCollection SS(IList<IWebElement> pageFactoryElementsList, IWebDriver driver)
        {
            return new SeleneCollection(
                pageFactoryElementsList, Configuration._With_(driver: driver)
            );
        }

        public static SeleneCollection SS(By locator, IWebDriver driver)
        {
            return new SeleneCollection(locator, Configuration._With_(driver: driver));
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
            ResizeWindow();
            
            var absoluteUrl = Uri.IsWellFormedUriString(url, UriKind.Absolute) 
                ? url 
                : Configuration.BaseUrl + url;
            GetWebDriver().Navigate().GoToUrl(absoluteUrl);
        }

        private static void ResizeWindow()
        {
            var width = Configuration.WindowWidth;
            var height = Configuration.WindowHeight;
            var sizeBefore = GetWebDriver().Manage().Window.Size;

            if (width.HasValue || height.HasValue)
            {
                if (!(width.HasValue && height.HasValue))
                {
                    width = width ?? sizeBefore.Width;
                    height = height ?? sizeBefore.Height;
                }
                
                GetWebDriver().Manage().Window.Size = new System.Drawing.Size(width.Value, height.Value);
            }
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


        internal static Wait<IWebDriver> Wait // TODO: Consider making it public
        {
            get
            {
                var paramsAndTheirUsagePattern = new Regex(@"\(?(\w+)\)?\s*=>\s*?\1\.");
                return new Wait<IWebDriver>(
                    entity: GetWebDriver(),
                    timeout: Configuration.Timeout,
                    polling: Configuration.PollDuringWaits,
                    _describeComputation: it => paramsAndTheirUsagePattern.Replace(
                        it, 
                        ""
                    ),
                    _hookAction: Configuration._HookWaitAction
                );
            }
        }

        public static void Should(Condition<IWebDriver> condition)
        {
            var wait = Wait.With(
                _describeComputation: (name => $"Should({name})")
            );
            wait.For(condition);
        }

        public static IWebDriver WaitTo(Condition<IWebDriver> condition) {
            Should(condition);
            return GetWebDriver();
        }

        public static TResult WaitFor<TResult>(
            TResult sEntity, 
            Condition<TResult> condition
        )
        {
            return WaitFor(
                sEntity, 
                condition, 
                Configuration.Timeout, 
                Configuration.PollDuringWaits
            );
        }

        public static TResult WaitForNot<TResult>(
            TResult sEntity, 
            Condition<TResult> condition
        )
        {
            return WaitForNot(
                sEntity, 
                condition, 
                Configuration.Timeout, 
                Configuration.PollDuringWaits
            );
        }

        public static TResult WaitFor<TResult>(
            TResult entity, 
            Condition<TResult> condition, 
            double timeout, 
            double pollDuringWaits=0
        )
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
                    #pragma warning disable 0618
                    if (condition.Apply(entity))
                    #pragma warning restore 0618
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
                    string text = 
                        $$"""

                        Timed out after {{timeoutSpan.TotalSeconds}} seconds "
                        while waiting entity with locator: {{entity}} "
                        for condition: {{condition}}
                        """;
                    throw new WebDriverTimeoutException(text, lastException);
                }
                Thread.Sleep(TimeSpan.FromSeconds(pollDuringWaits).Milliseconds);
            }
            return entity;
        }

        public static TResult WaitForNot<TResult>(
            TResult entity, 
            Condition<TResult> condition, 
            double timeout, 
            double pollDuringWaits=0
        )
        {
            Exception lastException = null;
            var timeoutSpan = TimeSpan.FromSeconds(timeout);
            var otherDateTime = DateTime.Now.Add(timeoutSpan);
//            var ignoredExceptionTypes = new [] { typeof(WebDriverException), typeof(IndexOutOfRangeException) };
            while (true)
            {
                try
                {
                    #pragma warning disable 0618
                    if (!condition.Apply(entity))
                    #pragma warning restore 0618
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
                    string text = string.Format(
                        """
                        
                        Timed out after {0} seconds
                        while waiting entity with locator: {1}
                        for condition: not
                        """
                        ,
                        timeoutSpan.TotalSeconds,
                        entity
                    );
                    text = text + condition;
                    throw new WebDriverTimeoutException(text, lastException);
                }
                Thread.Sleep(TimeSpan.FromSeconds(pollDuringWaits).Milliseconds);
            }
            return entity;
        }
    }
}
