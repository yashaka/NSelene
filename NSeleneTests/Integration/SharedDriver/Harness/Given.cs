namespace NSelene.Tests.Integration.SharedDriver.Harness
{
    public class When
    {
        public static void WithBody(string html)
        {
            Selene.ExecuteScript($"""
                document.getElementsByTagName('body')[0].innerHTML = `{html}`; 
                """
            );
        }

        public static void WithBodyTimedOut(string html, TimeSpan timeout)
        {
            ExecuteScriptWithTimeout($"""
                document.getElementsByTagName('body')[0].innerHTML = `{html}`;
                """, timeout
            );
        }

        public static void ExecuteScriptWithTimeout(string js, double timeout)
        {
            ExecuteScriptWithTimeout(js, TimeSpan.FromMilliseconds(timeout));
        }
        public static void ExecuteScriptWithTimeout(string js, TimeSpan timeout)
        {
            Selene.ExecuteScript(
                $"setTimeout(function () {{ {js} }}, {timeout.TotalMilliseconds})"
            );
        }
    }

    public class Given : When
    {

        public static void OpenedEmptyPage()
        {
            Selene.Open(BaseTest.EmptyHtmlUri); 
        }

        public static void OpenedEmptyPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(BaseTest.EmptyHtmlUri); 
        }

        public static void OpenedPageWithBody(string html)
        {
            Given.OpenedEmptyPage();
            When.WithBody(html);
        }

        public static void OpenedPageWithBodyTimedOut(string html, TimeSpan timeout)
        {
            Given.OpenedEmptyPage();
            When.WithBodyTimedOut(html, timeout);
        }
    }
}

