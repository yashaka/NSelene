using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.ConfigurationSpec
{
    using Harness;

    [TestFixture]
    public class Configuration_SetValueByJs_Should : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.SetValueByJs = false;
        }

        [Test]
        public void WaitForVisibility_OnActionsLikeClick()
        {
            Configuration.SetValueByJs = true;

            // todo: fix to work only when SetValueByJs
            // Given.OpenedPageWithBody(@"
            //         <div id='todo-app'>
            //           <input id='new-todo' placeholder='what needs to be done?' />
            //           <ul id='todo-list'>
            //           </ul>
            //         </div>"
            // );
            // Selene.ExecuteScript(@"
            //         let addTodo = function(event) {
            //           let inputField = event.target;
            //           let text = inputField.value.trim();

            //           if (event.which === enterCode && text !== '') {
            //             let newli = document.createElement('li');
            //             newli.textContent = text;

            //             document.getElementById('todo-list').appendChild(newli);
            //             inputField.value = '';
            //           }
            //         };

            //         let teachNewTodoFieldToAddTodos = function() {
            //           document.getElementById('new-todo').addEventListener('keyup', addTodo);
            //         };

            //         document.addEventListener('DOMContentLoaded', teachNewTodoFieldToAddTodos)
            //     "
            // );
            Open("http://todomvc.com/examples/emberjs");
            S("#new-todo").SetValue("very long task description set instantly").PressEnter();
            SS("#todo-list>li").Should(Have.Texts("long task"));
        }
    }
}

