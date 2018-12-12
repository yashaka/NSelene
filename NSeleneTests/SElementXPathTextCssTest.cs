using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

using NUnit.Framework;

using NSelene;
using static NSelene.Selene;

namespace NSeleneTests
{
    [TestFixture]
    public class SElementXPathTextCssTest : BaseTest
    {
        [Test]
        public void SElementAlternativeCssSearch()
        {

            Given.OpenedPageWithBody("<h1 name='hello'>Hello Babe!</h1>");
            S("css = h1[name='hello']").Should(Have.Attribute("name", "hello"));
            S("h1:nth-of-type(1)").Should(Have.Text("Hello"));
        }


        [Test]
        public void SElementXpathSearch()
        {
            Given.OpenedPageWithBody("<h1>Hello Babe!</h1>");
            S("xpath = //h1").ShouldNot(Have.Text("Hello world!"));
            S("xpath = /h1[1]").ShouldNot(Have.Text("Hello world!"));

        }

        [Test]
        public void SElementTextSearch()
        {
            Given.OpenedPageWithBody("<h1>Hello Babe!</h1>");
            S("text = Hello").ShouldNot(Have.ExactText("Hello"));
            S("text = Hello Babe!").ShouldNot(Have.ExactText("Hello"));
        }

        [Test]
        public void SeleneCollectionsShouldWorkWithText()
        {
            Given.OpenedPageWithBody("<ul>Hello to:<li>Dear Bob</li><li>Dear Frank</li><li>Lovely Kate</li></ul>");
            // Begin with  bare bones Selenium WebDriver and assert XPath is valid
            ReadOnlyCollection<IWebElement> webElements = Selene.GetWebDriver().FindElements(By.XPath("//*[contains(text(),'Dear')]"));
            Assert.NotNull(webElements);
            Assert.Greater(webElements.Count, 0);
            StringAssert.Contains("Dear", webElements[0].Text);
            // inspect the underlying collection - 
            // not commonly done in the test
            String seleneLocator = "text = Dear";
            SeleneCollection seleWebElements = SS(seleneLocator);
            Assert.NotNull(seleWebElements);
            Assert.Greater(seleWebElements.Count, 0);
            StringAssert.Contains("Dear", seleWebElements[0].Text);

            // exercise NSelene extension methods
            SS(seleneLocator).Should(Have.CountAtLeast(1));
            SS(seleneLocator).Should(Have.Texts("Bob", "Frank"));
            SS(seleneLocator).ShouldNot(Have.Texts("Bob"));
            SS(seleneLocator).ShouldNot(Have.Texts("Bob", "Kate", "Frank"));
            SS(seleneLocator).Should(Have.ExactTexts("Dear Bob", "Dear Frank"));
        }

        [Test]
        public void SeleneCollectionsShouldWorkWithXPath()
        {
            Given.OpenedPageWithBody("<ul>Hello to:<li>Dear Bob</li><li>Dear Frank</li><li>Lovely Kate</li></ul>");
            String xpath = "//ul/li";
            // Begin with  bare bones Selenium WebDriver and assert XPath is valid
            ReadOnlyCollection<IWebElement> webElements = Selene.GetWebDriver().FindElements(By.XPath(xpath));
            Assert.NotNull(webElements);
            Assert.Greater(webElements.Count, 0);
            StringAssert.IsMatch("li", webElements[0].TagName);
            // check the underlying collection - commonly not sent to the test  
            SeleneCollection seleWebElements = null;
            String seleneLocator = String.Format("xpath = {0}", xpath);
            seleWebElements = SS(seleneLocator);
            Assert.NotNull(seleWebElements);
            Assert.Greater(seleWebElements.Count, 0);
            StringAssert.IsMatch("li", seleWebElements[0].TagName);
            // exercise NSelene extension methods
            SS(seleneLocator).Should(Have.ExactTexts("Dear Bob", "Dear Frank", "Lovely Kate"));
            SS(seleneLocator).Should(Have.CountAtLeast(1));
        }


    }
}