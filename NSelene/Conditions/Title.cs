using System;
using NSelene.Conditions;
using OpenQA.Selenium;

namespace NSelene
{
    namespace Conditions 
    {
        public class Title : Condition<IWebDriver>
        {
            private string expected;

            public Title (string expected)
            {
                this.expected = expected;
            }

            public override string ToString()
            {
                return $"Have.Title(«{this.expected}»)";
            }

            public override void Invoke(IWebDriver entity)
            {
                var actual = entity.Title;
                if (!actual.Equals(this.expected))
                {
                    throw new ConditionNotMatchedException(() => 
                        $"Actual title: «{actual}»\n"
                    );
                }
            }
        }
        public class TitleContaining : Condition<IWebDriver>
        {
            private string expected;

            public TitleContaining (string expected)
            {
                this.expected = expected;
            }

            public override string ToString()
            {
                return $"Have.TitleContaining(«{this.expected}»)";
            }

            public override void Invoke(IWebDriver entity)
            {
                var actual = entity.Title;
                if (!actual.Contains(this.expected))
                {
                    throw new ConditionNotMatchedException(() => 
                        $"Actual title: «{actual}»\n"
                    );
                }
            }
        }
    }

    public static partial class Have
    {
        public static Conditions.Condition<IWebDriver> TitleContaining(string expected)
        {
            return new Conditions.TitleContaining(expected);
        }

        public static Conditions.Condition<IWebDriver> Title(string expected)
        {
            return new Conditions.Title(expected);
        }

        static partial class No
        {
            public static Conditions.Condition<IWebDriver> TitleContaining(string expected) 
            => new Conditions.TitleContaining(expected).Not;

            public static Conditions.Condition<IWebDriver> Title(string expected) 
            => new Conditions.Title(expected).Not;
        }
    }
}
