using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSelene;
using NSelene.Support.Extensions;

/*
 * This "core" part emulates some custom "pageobjects & Concise API to Selenium" framework 
 * - PageObjects based on OpenQA.Selenium.Support.PageObjects (+PageFactory)
 * - Concise API based on extension methods
 * - without AJAX support
 */
namespace NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.AfterPlusAlternativeStyleOfNaming.Core
{
    public class PageObject
    {

        IWebDriver driver;

        /*******************************************************************
         * Integrating all NSelene power into "driver" wrapper property - I
         * in order to use it in the code of pageobjects like: 
         * - Browser.Element or Element, 
         * - Browser.Element or Elements, 
         * - Browser.Open, 
         * - etc.
         */
        public SDriver Browser { get; private set;}

        public PageObject(IWebDriver driver)
        {
            this.driver = driver;
            this.Browser = new SDriver(this.driver);
            PageFactory.InitElements(this.driver, this);
        }

        public SElement Element(string cssSelector)
        {
            return Browser.Element(cssSelector);
        }

        public SElement Element(By locator)
        {
            return Browser.Element(locator);
        }

        public SCollection Elements(string cssSelector)
        {
            return Browser.Elements(cssSelector);
        }

        // etc.

        /******************************************************************/

        public void Open(string url)
        {
            driver.Navigate().GoToUrl("https://todomvc4tasj.herokuapp.com/");
        }
    }

    public static class IListOfWebElementsExtensions
    {
        public static void ShouldHaveExactVisibleTexts(this IList<IWebElement> elements, params string[] texts)
        {
            var actual = elements.Where(element => element.Displayed)
                                 .Select(element => element.Text);
            CollectionAssert.AreEqual(texts, actual);
        }

        public static IWebElement FindByText(this IList<IWebElement> elements, string text)
        {
            return elements.FirstOrDefault(element => element.Text == text);
        }
    }

    public static class IWebElementExtensions
    {

        public static IWebElement Find(this IWebElement element, string cssSelector)
        {
            return element.FindElement(By.CssSelector(cssSelector));
        }

        public static IWebElement SetValue(this IWebElement element, string text)
        {
            element.Clear();
            element.SendKeys(text);
            return element;
        }

        public static IWebElement PressEnter(this IWebElement element)
        {
            element.SendKeys(Keys.Enter);
            return element;
        }
    }

}
