using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

/*
 * This "core" part emulates some custom "pageobjects & Concise API to Selenium" framework 
 * - PageObjects based on OpenQA.Selenium.Support.PageObjects (+PageFactory)
 * - Concise API based on extension methods
 * - without AJAX support
 */
namespace NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.Before.Core
{
    public class PageObject
    {

        IWebDriver driver;

        public PageObject(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(this.driver, this);
        }

        public void Open(string url)
        {
            driver.Navigate().GoToUrl(url);
            System.Threading.Thread.Sleep(3000);
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
