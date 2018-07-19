using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.After.Core;
using NSelene;
using NSelene.Support.Extensions;

namespace NSeleneExamples.TodoMVC.IntegratedToSeleniumBasedFramework.After.Pages
{
    public class Tasks : PageObject
    {

        /*
         * Now, for all new functionality you can use more straightforward NSelene syntax 
         * with all its ajax-friendly helpers like Should (= AssertTo), etc...
         */

        public void ClearCompleted()
        {
            I.Find("#clear-completed").Click();
            I.Find("#clear-completed").AssertToNot(Be.Visible);  // same as ... .ShouldNot(Be.Visible); but fits better to "I. ..."
        }

        /*
         * You can also reuse your old pagefactory elements via wrapping them correspondingly:
         */

        public void ShouldBeEmpty()
        {
            I.FindAll(list).FilterBy(Be.Visible).AssertTo(Be.Empty);
        }

        /*
         * As you can see, all "old" code still works as it was working before...
         */

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
            I.Should (Have.JSReturnedTrue(
                "return " +
                "$._data($('#new-todo').get(0), 'events').hasOwnProperty('keyup')&& " +
                "$._data($('#toggle-all').get(0), 'events').hasOwnProperty('change') && " +
                "$._data($('#clear-completed').get(0), 'events').hasOwnProperty('click')"));
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
                /*
                 * Once you see that "old" code become unstable, 
                 * you can switch it to the NSelene version (which is more stable and ajax-friendly)
                 * still using your "old Selenium PageFactory elements"
                 */

                //newTodo.SetValue(text).PressEnter();
                I.Find(newTodo).AssertTo(Be.Enabled).SetValue(text).PressEnter();
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
