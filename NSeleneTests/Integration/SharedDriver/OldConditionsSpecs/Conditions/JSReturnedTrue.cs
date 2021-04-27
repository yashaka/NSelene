using System;
using NSelene.Conditions;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
{
    namespace Conditions 
    {

        public class JSReturnedTrue : DescribedCondition<IWebDriver>
        {
            private string script;
            private object [] arguments;
            private bool result;

            public JSReturnedTrue (string script, params object [] arguments)
            {
                this.script = script;
                this.arguments = arguments;
            }

            public override bool Apply (IWebDriver entity)
            {
                try {
                    this.result = (bool) (entity as IJavaScriptExecutor).ExecuteScript (this.script, this.arguments);
                } catch (Exception) {
                    this.result = false;
                }
                return this.result;
            }

            public override string DescribeActual ()
            {
                return "" + this.result;
            }
        }
    }

    public static partial class Have
    {
        public static Condition<IWebDriver> JSReturnedTrue(string script, params object[] arguments) 
        => new Conditions.JSReturnedTrue(script, arguments);

        public static partial class No
        {
            public static Condition<IWebDriver> JSReturnedTrue(string script, params object[] arguments) 
            => new JSReturnedTrue(script, arguments).Not;
        }
    }
}
