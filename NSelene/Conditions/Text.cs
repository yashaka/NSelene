using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class Text : Condition<SeleneElement>
        {
            protected string expected;

            public Text(string expected)
            {
                this.expected = expected;
            }

            public override string ToString()
            {
                return $"TextContaining(«{this.expected}»)";
            }

            public override void Invoke(SeleneElement entity)
            {
                var webelement = entity.ActualWebElement;
                var actual = webelement.Text;
                if (!actual.Contains(this.expected))
                {
                    throw new ConditionNotMatchedException(() => 
                        $"Actual text: «{actual}»\n"
                        + $"Actual webelement: {webelement.GetAttribute("outerHTML")}"
                    );
                }
            }
        }

        public class ExactText : Condition<SeleneElement>
        {
            protected string expected;

            public ExactText(string expected)
            {
                this.expected = expected;
            }

            public override string ToString()
            {
                return $"ExactText(«{this.expected}»)";
            }

            public override void Invoke(SeleneElement entity)
            {
                var webelement = entity.ActualWebElement;
                var actual = webelement.Text;
                if (!actual.Equals(this.expected))
                {
                    throw new ConditionNotMatchedException(() => 
                        $"Actual text: «{actual}»\n"
                        + $"Actual webelement: {webelement.GetAttribute("outerHTML")}"
                    );
                }
            }
        }
    }

    public static partial class Have
    {
        public static Conditions.Condition<SeleneElement> Text(string expected)
        {
            return new Conditions.Text(expected);
        }

        public static Conditions.Condition<SeleneElement> ExactText(string expected)
        {
            return new Conditions.ExactText(expected);
        }

        static partial class No
        {
            public static Conditions.Condition<SeleneElement> Text(string expected) 
            => new Conditions.Text(expected).Not;

            public static Conditions.Condition<SeleneElement> ExactText(string expected) 
            => new Conditions.ExactText(expected).Not;
        }
    }

}
