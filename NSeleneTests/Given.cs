using System;
using NSelene;

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

        // TODO: consider renaming to WithBodyTimedOut
        public static void WithBody(string html, int timeout)
        {
            Utils.ExecuteScript(@"
                setTimeout(
                    function(){
                        document.getElementsByTagName('body')[0].innerHTML = """ + html.Replace("\n", "") + @"""
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
                    new Uri(Environment.CurrentDirectory), 
                    "../Resources/empty.html"
                ).AbsoluteUri
            ); 
        }

        public static void OpenedPageWithBody(string html)
        {
            Given.OpenedEmptyPage();
            When.WithBody(html);
        }
    }
}

