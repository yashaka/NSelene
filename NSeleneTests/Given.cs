using System;
using System.Reflection;
using NSelene;
using OpenQA.Selenium;

namespace NSeleneTests
{
    public static class When
    {
        public static void WithBody(string html)
        {
            Utils.ExecuteScript(
                "document.getElementsByTagName('body')[0].innerHTML = \"" 
                + html.Replace("\n", "") + "\";"
            );
        }

        public static void WithBody(string html, IWebDriver driver)
        {
            (driver as IJavaScriptExecutor).ExecuteScript(
                "document.getElementsByTagName('body')[0].innerHTML = \"" 
                + html.Replace("\n", "") + "\";"
            );
        }

        // TODO: consider renaming to WithBodyTimedOut
        public static void WithBodyTimedOut(string html, int timeout)
        {
            Utils.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('body')[0].innerHTML = """ + html.Replace("\n", "") + @"""
                    }, 
                    " + timeout + ");"
            );
        }

        public static void WithBodyTimedOut(string html, int timeout, IWebDriver driver)
        {
            (driver as IJavaScriptExecutor).ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('body')[0].innerHTML = """ + html.Replace("\n", "") + @"""
                    }, 
                    " + timeout + ");"
            );
        }

        public static void ExecuteScriptWithTimeout(string js, int timeout)
        {
            Utils.ExecuteScript(@"
                setTimeout(
                    function(){
                        " + js + @"
                    }, 
                    " + timeout + ");"
            );
        }

        public static void ExecuteScriptWithTimeout(string js, int timeout, IWebDriver driver)
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

    public static class Given
    {

        public static void OpenedEmptyPage(){
            Utils.Open(
                new Uri(
                    new Uri(Assembly.GetExecutingAssembly().Location), 
                    "../../Resources/empty.html"
                ).AbsoluteUri
            ); 
        }

        public static void OpenedEmptyPage(IWebDriver driver){
            driver.Navigate().GoToUrl(
                new Uri(
                    new Uri(Assembly.GetExecutingAssembly().Location), 
                    "../../Resources/empty.html"
                ).AbsoluteUri
            ); 
        }

        public static void OpenedPageWithBody(string html)
        {
            Given.OpenedEmptyPage();
            When.WithBody(html);
        }

        public static void OpenedPageWithBody(string html, IWebDriver driver)
        {
            Given.OpenedEmptyPage(driver);
            When.WithBody(html, driver);
        }
    }
}

