using System;
using OpenQA.Selenium;
using System.Linq;

namespace NSelene
{
    public static class Selectors
    {

        [Obsolete("Selectors.ByCss is deprecated and will be removed in next version, please use With.Css method instead.")]
        public static By ByCss(string cssSelector)
        {
            return By.CssSelector(cssSelector);
        }

        [Obsolete("Selectors.ByLinkText is deprecated and will be removed in next version, please use With.LinkText method instead.")]
        public static By ByLinkText(string text)
        {
            return By.LinkText(text);
        }
    }

    public static class With
    {
        // TODO: ensure these methods are covered with tests
        // originally this implementation were moved from the selenidejs repository
        // counting on "they were tested there" ;)

        const string NORMALIZE_SPACE_XPATH = "normalize-space(translate(string(.), '\t\n\r\u00a0', '    '))";

        public static By Type(string type)
        {
            return By.XPath($"//*[@type = '{type}']");
        }

        public static By Value(string value)
        {
            return By.XPath($"//*[@value = '{value}']");
        }

        public static By IdContains(params string[] idParts)
        {
            return By.XPath(
                "//*[" +
                string.Join(" and ",
                    idParts.ToList().Select(idPart => $"contains(@id, '{idPart}')")) +
                "]");
        }

        public static By Text(string text)
        {
            return By.XPath($"//*/text()[contains({NORMALIZE_SPACE_XPATH}, '{text}')]/parent::*");
        }

        public static By ExactText(string text)
        {
            return By.XPath($"//*/text()[{NORMALIZE_SPACE_XPATH} = '{text}']/parent::*");
        }

        public static By Id(string id)
        {
            return By.Id(id);
        }

        public static By Name(string name)
        {
            return By.Name(name);
        }

        public static By ClassName(string className)
        {
            return By.ClassName(className);
        }

        public static By XPath(string xpath)
        {
            return By.XPath(xpath);
        }

        public static By Css(string css)
        {
            return By.CssSelector(css);
        }

        public static By AttributeContains(string attributeName, string attributeValue)
        {
            return By.XPath($".//*[contains(@{attributeName}, '{attributeValue}')]");
        }

        public static By Attribute(string attributeName, string attributeValue)
        {
            return By.XPath($".//*[@{attributeName} = '{attributeValue}']");
        }
    }
}

