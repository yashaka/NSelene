using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.After.Core;
using NSelene;
using static NSelene.Selectors;
using NSelene.Support.Extensions;

namespace NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.AfterPlusAlternativeStyleOfPageFields.Pages
{
    public class Tasks : PageObject
    {

        /*
         * If you want, you can use more consice style of page fields definitions
         * with a bit less consice page fields usage...
         */

        By list = ByCss("#todo-list>li");
        By clearCompleted = ByCss("#clear-completed");

        public Tasks(IWebDriver driver) : base(driver) {}

        /*
         * By the way, you could use something like
         
        String list = "#todo-list>li";
        String clearCompleted = "#clear-completed";
         
         * It would be even more consice, but less obvious, even when you a going to add other fields, 
         * not String ones, but e.g. xpath or ByLinkText, etc...
         */

        public void Visit()
        {
            I.Open("https://todomvc4tasj.herokuapp.com/");
        }

        public void FilterActive()
        {
            I.Find(By.LinkText("Active")).Click();
        }

        public void FilterCompleted()
        {
            I.Find(By.LinkText("Completed")).Click();
        }

        public void Add(params string[] taskTexts)
        {
            foreach (var text in taskTexts)
            {
                I.Find("#new-todo").Should(Be.Enabled).SetValue(text).PressEnter();
            }
        }

        public void Toggle(string taskText)
        {
            IWebElementExtensions.Find(I.FindAll(list).FindBy(Have.ExactText(taskText)),".toggle").Click();
        }

        public void ClearCompleted()
        {
            I.Find(clearCompleted).Click();
            I.Find(clearCompleted).ShouldNot(Be.Visible);
        }

        public void ShouldBe(params string[] names)
        {
            I.FindAll(list).FilterBy(Be.Visible).Should(Have.Texts(names));
        }

        public void ShouldBeEmpty(params string[] names)
        {
            I.FindAll(list).FilterBy(Be.Visible).Should(Be.Empty);
        }
    }

}
