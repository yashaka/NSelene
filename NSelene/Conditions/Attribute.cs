using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class Attribute : Condition<SeleneElement>
        {
            private string name;
            private string expectedValue;

            public Attribute(string name, string value)
            {
                this.name = name;
                this.expectedValue = value;
            }

            public override string ToString()
            {
                return $"Attribute({this.name} = «{this.expectedValue}»)";
            }

            public override void Invoke(SeleneElement entity)
            {
                var webelement = entity.ActualWebElement;
                var maybeActual = webelement.GetAttribute(this.name);
                if (maybeActual == null || !maybeActual.Equals(this.expectedValue))
                {
                    throw new ConditionNotMatchedException(() => 
                        (maybeActual == null 
                        ? $"Actual {this.name}: Null (attribute is absent)\n" 
                        : $"Actual {this.name}: «{maybeActual}»\n"
                        )
                        + $"Actual webelement: {webelement.GetAttribute("outerHTML")}"
                    );
                }
            }
        }
    }

    public static partial class Have
    {
        public static Conditions.Condition<SeleneElement> Attribute(string name, string value) 
        => new Conditions.Attribute(name, value);

        public static Conditions.Condition<SeleneElement> Value(string expected) 
        => new Conditions.Attribute("value", expected);

        public static partial class No
        {
            public static Conditions.Condition<SeleneElement> Attribute(string name, string value) 
            => new Conditions.Attribute(name, value).Not;
            public static Conditions.Condition<SeleneElement> Value(string expected) 
            => new Conditions.Attribute("value", expected).Not;
        }
    }

    public static partial class Be
    {
        public static Conditions.Condition<SeleneElement> Blank 
        => new Conditions.Attribute("value", "");
        // TODO: consider as => new Conditions.Attribute("value", "").Or(Have.ExactText(""));

        public static partial class Not
        {
            public static Conditions.Condition<SeleneElement> Blank 
            => new Conditions.Attribute("value", "").Not;
        }
    }
}
