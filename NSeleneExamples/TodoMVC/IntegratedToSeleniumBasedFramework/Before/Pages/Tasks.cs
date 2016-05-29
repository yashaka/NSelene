using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.Before.Core;

namespace NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.Before.Pages
{
    public class Tasks : PageObject
    {
        public Tasks(IWebDriver driver) : base(driver) {}
        
        [FindsBy(How = How.Id, Using = "new-todo")]
        IWebElement newTodo;

        [FindsBy(How = How.CssSelector, Using = "#todo-list>li")]
        IList<IWebElement> list;

        [FindsBy(How = How.LinkText, Using = "Active")]
        IWebElement activeFilter;

        [FindsBy(How = How.LinkText, Using = "Completed")]
        IWebElement completedFilter;

        public void Visit()
        {
            Open("https://todomvc4tasj.herokuapp.com/");
        }

        public void FilterActive()
        {
            activeFilter.Click();
        }

        public void FilterCompleted()
        {
            completedFilter.Click();
        }

        public void Add(params string[] taskTexts)
        {
            foreach (var text in taskTexts)
            {
                newTodo.SetValue(text).PressEnter();
            }
        }

        public void Toggle(string taskText)
        {
            list.FindByText(taskText).Find(".toggle").Click();
        }

        public void ShouldBe(params string[] names)
        {
            list.ShouldHaveExactVisibleTexts(names);
        }
    }

}
