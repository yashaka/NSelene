using System;
using OpenQA.Selenium;
using NSelene;
using static NSelene.Utils;
using NSelene.Conditions;

namespace NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.AfterPlusPlus.Pages
{
    public class Tasks
    {
        SCollection list = SS("#todo-list>li");

        SElement clearCompleted = S("#clear-completed");

        public void Visit()
        {
            Open("https://todomvc4tasj.herokuapp.com/");
        }

        public void FilterActive()
        {
            S(By.LinkText("Active")).Click();
        }

        public void FilterCompleted()
        {
            S(By.LinkText("Completed")).Click();
        }

        public void Add(params string[] taskTexts)
        {
            foreach (var text in taskTexts)
            {
                S("#new-todo").Should(Be.Enabled).SetValue(text).PressEnter();
            }
        }

        public void Toggle(string taskText)
        {
            list.FindBy(Have.ExactText(taskText)).Find(".toggle").Click();
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
