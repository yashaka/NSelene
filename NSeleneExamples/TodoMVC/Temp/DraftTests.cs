using System;
using NUnit.Framework;
using NSelene;
using static NSelene.Utils;

namespace NSeleneExamples.TodoMVC.Temp
{
    [TestFixture]
    public class DraftTests : BaseTest
    {

        [Test]
        public void ProvidesCommonTaskManagementTest()
        {
            Config.Timeout = 0.25;
            //Open("http://todomvc4tasj.herokuapp.com/");
            //S("#new-todo").SetValue("a").PressEnter();
            //S("#new-todo").SetValue("c").PressEnter();

            //SS("#todo-list>li").Should(Have.ExactTexts("i", "k"));

            //SS("#todo-list>li").FindBy(Have.ExactText("b"))
            //                   .DoubleClick();
        }

        [Test]
        public void ProvidesCommonTaskManagement()
        {
            Open("http://todomvc4tasj.herokuapp.com/");
            S("#new-todo") /* > S("input#new-todo") */
                .SetValue("a").PressEnter();  /* "a" > "do something" > " lk@ї\nи₴\"j;k*7 \n" */
            SS("#todo-list>li").Should(Have.ExactTexts("a")); /* though here weeker, may be missed in e2e when implicitly checked, 
                nevertheless 
                > SS("#todo-list>li").Get(0).Should(Have.ExactText("a"));
                > SS("#todo-list>li").Should(Have.Count(1)); */

            SS("#todo-list>li").FindBy(Have.ExactText("a")) /* a bit slower but
                    > S(By.XPath("//*[@id='todo-list']/li//*[.='a']")) 
                    ~ S("#todo-list>li").Get(0)
                    > S("#todo-list>li:nth-of-type(1)") */

                               /* = premature decomposition over debug */

                               .DoubleClick();
            SS("#todo-list>li").FindBy(Have.CssClass("editing")).S(".edit")/* a bit slower but
                    > S("todo-list>li.editing .edit")
                    > S(".editing .edit") 
                    x S(".edit") */
                               .SetValue("a edited").PressEnter();  /* "a edited ~ "a*" ~ "a'" > "b" */
            /* implicit checks when possible (even if weaker - can be missed in e2e...)
            > SS("#todo-list>li").Should(Have.ExactTexts("a edited")); */

            SS("#todo-list>li").FindBy(Have.ExactText("a edited"))
                               .S(".toggle") /* = .Find(".edit") */
                               .Click();
            /* no style checks 
               > SS("#todo-list>li").FilterBy(Have.CssClass("completed")).Should(Have.Texts("a edited")); */

            S("#clear-completed").Click();
            SS("#todo-list>li").Should(Be.Empty);

            S("#new-todo").SetValue("b").PressEnter();

            SS("#todo-list>li").FindBy(Have.ExactText("b")).Hover()
                               .S(".destroy").Click(); /* > .S("button").Click(); */
            SS("#todo-list>li").Should(Be.Empty);
        }
    }
}

