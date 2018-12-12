using System;
using OpenQA.Selenium;

namespace NSelene
{
    public static class Selectors
    {
        public static By ByCss(string cssSelector)
        {
            return By.CssSelector(cssSelector);
        }

        public static By ByLinkText(string text)
        {
            return By.LinkText(text);
        }

        public static By ByXpath(string xpath)
        {
            return By.XPath(xpath);
        }

        // TODO: add ByText & ByExactText, etc.
    }
}

