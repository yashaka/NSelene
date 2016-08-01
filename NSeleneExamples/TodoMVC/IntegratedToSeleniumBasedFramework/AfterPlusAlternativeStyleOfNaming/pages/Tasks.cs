using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.AfterPlusAlternativeStyleOfNaming.Core;
using NSelene;
using NSelene.Support.Extensions;

namespace NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.AfterPlusAlternativeStyleOfNaming.Pages
{
    public class Tasks : PageObject
    {

        /*
         * Once you have time, you refactor some PageObject to use only NSelene syntax (while other pages can use old one...)
         */

        SCollection list;
        SElement clearCompleted;

        public Tasks(IWebDriver driver) : base(driver) {
            list = Elements("#todo-list>li");
            clearCompleted = Element("#clear-completed");
        }

        /*
         * As you can see you don't need so much boilerplate as with PageFactory solution
         * You can define only those fields which you reuse more than once. 
         */

        public void Visit()
        {
            Browser.Open("https://todomvc4tasj.herokuapp.com/");
        }

        public void FilterActive()
        {
            Element(By.LinkText("Active")).Click();
        }

        public void FilterCompleted()
        {
            Element(By.LinkText("Completed")).Click();
        }

        public void Add(params string[] taskTexts)
        {
            foreach (var text in taskTexts)
            {
                Element("#new-todo").Should(Be.Enabled).SetValue(text).PressEnter();
            }
        }

        public void Toggle(string taskText)
        {
            IWebElementExtensions.Find(list.FindBy(Have.ExactText(taskText)),".toggle").Click();
        }

        public void ClearCompleted()
        {
            clearCompleted.Click();
            clearCompleted.ShouldNot(Be.Visible);
        }

        public void ShouldBe(params string[] names)
        {
            list.FilterBy(Be.Visible).Should(Have.Texts(names));
        }

        public void ShouldBeEmpty(params string[] names)
        {
            list.FilterBy(Be.Visible).Should(Be.Empty);
        }
    }

}
