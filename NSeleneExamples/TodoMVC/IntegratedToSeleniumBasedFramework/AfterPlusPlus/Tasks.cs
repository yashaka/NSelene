using System;
using OpenQA.Selenium;
using NSelene;
using static NSelene.Utils;
using NSelene.Conditions;

namespace NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.AfterPlus.Pages
{
    public class Tasks
    {
        public SCollection List = SS("#todo-list>li");

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
            List.FindBy(Have.ExactText(taskText)).Find(".toggle").Click();
        }

        public void ShouldBe(params string[] names)
        {
            List.FilterBy(Be.Visible).Should(Have.Texts(names));
        }
    }

}
