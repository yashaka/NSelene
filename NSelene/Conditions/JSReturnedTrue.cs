using System;
using OpenQA.Selenium;

namespace NSelene
{
    namespace Conditions 
    {
        public class JSReturnedTrue : DescribedCondition<IWebDriver>
        {
            private string script;
            private object [] arguments;
            private object result;

            public JSReturnedTrue (string script, params object [] arguments)
            {
                this.script = script;
                this.arguments = arguments;
            }

            public override bool Apply (IWebDriver entity)
            {
                try {
                    this.result = (entity as IJavaScriptExecutor).ExecuteScript (this.script, this.arguments);
                    return (bool) this.result;
                } catch (Exception) {
                    return false;
                }
            }

            public override string DescribeActual ()
            {
                return "" + (bool) this.result;
            }
        }
    }

    public static partial class Have
    {
        public static Conditions.Condition<IWebDriver> JSReturnedTrue (string script, params object [] arguments)
        {
            return new Conditions.JSReturnedTrue (script, arguments);
        }
    }
}
