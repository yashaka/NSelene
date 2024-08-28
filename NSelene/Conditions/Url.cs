using System;
using NSelene.Conditions;
using OpenQA.Selenium;

namespace NSelene
{
    namespace Conditions 
    {
        public class Url : Condition<IWebDriver>
        {
            private string expected;

            public Url (string expected)
            {
                this.expected = expected;
            }

            public override string ToString()
            {
                return $"Have.Url(«{this.expected}»)";
            }

            public override void Invoke(IWebDriver entity)
            {
                var actual = entity.Url;
                if (!actual.Equals(this.expected))
                {
                    throw new ConditionNotMatchedException(() => 
                        $"Actual url: «{actual}»\n"
                    );
                }
            }
        }
        public class UrlContaining : Condition<IWebDriver>
        {
            private string expected;

            public UrlContaining (string expected)
            {
                this.expected = expected;
            }

            public override string ToString()
            {
                return $"Have.UrlContaining(«{this.expected}»)";
            }

            public override void Invoke(IWebDriver entity)
            {
                var actual = entity.Url;
                if (!actual.Contains(this.expected))
                {
                    throw new ConditionNotMatchedException(() => 
                        $"Actual url: «{actual}»\n"
                    );
                }
            }
        }
    }

    public static partial class Have
    {
        public static Conditions.Condition<IWebDriver> UrlContaining(string expected)
        {
            return new Conditions.UrlContaining(expected);
        }

        public static Conditions.Condition<IWebDriver> Url(string expected)
        {
            return new Conditions.Url(expected);
        }

        static partial class No
        {
            public static Conditions.Condition<IWebDriver> UrlContaining(string expected) 
            => new Conditions.UrlContaining(expected).Not;

            public static Conditions.Condition<IWebDriver> Url(string expected) 
            => new Conditions.Url(expected).Not;
        }
    }
}
