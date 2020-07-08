using OpenQA.Selenium;

namespace NSelene
{
    internal static class Utils
    {
        public static By ToBy(string cssOrXpathSelector)
        {
            if (cssOrXpathSelector.StartsWith("/") ||
                cssOrXpathSelector.StartsWith("./") ||
                cssOrXpathSelector.StartsWith("..") || 
                cssOrXpathSelector.StartsWith("("))
            {
                return By.XPath(cssOrXpathSelector);
            } else
            {
                return By.CssSelector(cssOrXpathSelector);
            }
        }
    }
}