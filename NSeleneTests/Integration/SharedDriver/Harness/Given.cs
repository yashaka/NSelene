using System;
using System.Reflection;
using NSelene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.Harness
{
    public class When
    {
        public static void WithBody(string html)
        {
            Selene.ExecuteScript(
                "document.getElementsByTagName('body')[0].innerHTML = `" 
                + html + "`;"
            );
        }

        public static void WithBody(string html, IWebDriver driver)
        {
            (driver as IJavaScriptExecutor).ExecuteScript(
                "document.getElementsByTagName('body')[0].innerHTML = `" 
                + html + "`;"
            );
        }

        public static void WithBodyTimedOut(string html, double timeout)
        {
            Selene.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('body')[0].innerHTML = `" + html + @"`
                    }, 
                    " + timeout + ");"
            );
        }

        public static void WithBodyTimedOut(string html, double timeout, IWebDriver driver)
        {
            (driver as IJavaScriptExecutor).ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('body')[0].innerHTML = `" + html + @"`
                    }, 
                    " + timeout + ");"
            );
        }

        public static void ExecuteScriptWithTimeout(string js, double timeout)
        {
            Selene.ExecuteScript(
                $"setTimeout(function () {{ {js} }}, {timeout})"
            );
        }

        public static void ExecuteScriptWithTimeout(string js, double timeout, IWebDriver driver)
        {
            (driver as IJavaScriptExecutor).ExecuteScript(@"
                setTimeout(
                    function(){
                        " + js + @"
                    }, 
                    " + timeout + ");"
            );
        }
    }

    public class Given : When
    {

        public static void OpenedEmptyPage(){
            Selene.Open(
                new Uri(
                    new Uri(Assembly.GetExecutingAssembly().Location), 
                    "../../../Resources/empty.html"
                ).AbsoluteUri
            ); 
        }

        public static void OpenedEmptyPage(IWebDriver driver){
            driver.Navigate().GoToUrl(
                new Uri(
                    new Uri(Assembly.GetExecutingAssembly().Location), 
                    "../../../Resources/empty.html"
                ).AbsoluteUri
            ); 
        }

        public static void OpenedPageWithBody(string html)
        {
            Given.OpenedEmptyPage();
            When.WithBody(html);
        }

        public static void OpenedPageWithBodyTimedOut(string html, double timeout)
        {
            Given.OpenedEmptyPage();
            When.WithBodyTimedOut(html, timeout);
        }

        public static void OpenedPageWithBody(string html, IWebDriver driver)
        {
            Given.OpenedEmptyPage(driver);
            When.WithBody(html, driver);
        }
    }
}

