using System.Text.RegularExpressions;

namespace NSelene
{
    namespace Conditions
    {
        public class AttributeWithValueMatching : Condition<SeleneElement>
        {
            private string attributeName;
            private string pattern;

            public AttributeWithValueMatching(string attributeName, string pattern)
            {
                this.attributeName = attributeName;
                this.pattern = pattern;
            }

            public override string ToString()
            {
                return $"Have.Attribute({this.attributeName} matching regex pattern: «{this.pattern}»)";
            }

            public override void Invoke(SeleneElement entity)
            {
                var webelement = entity.ActualWebElement;
                var maybeActual = webelement.GetAttribute(this.attributeName);

                if (maybeActual == null || !Regex.IsMatch(maybeActual, pattern))
                {
                    throw new ConditionNotMatchedException(() =>
                        (maybeActual == null
                            ? $"Actual {this.attributeName}: Null (attribute is absent)\n"
                            : $"Actual {this.attributeName}: «{maybeActual}»\n"
                        )
                        + $"Actual webelement: {webelement.GetAttribute("outerHTML")}"
                    );
                }
            }
        }
    }

    public static partial class Have
    {
        public static Conditions.Condition<SeleneElement> AttributeWithValueStarting(string attributeName, string value)
        {
            var pattern = $"^{Regex.Escape(value)}";
            return new Conditions.AttributeWithValueMatching(attributeName, pattern);
        }

        public static Conditions.Condition<SeleneElement> AttributeWithValueEnding(string attributeName, string value)
        {
            var pattern = $"{Regex.Escape(value)}$";
            return new Conditions.AttributeWithValueMatching(attributeName, pattern);
        }

        public static Conditions.Condition<SeleneElement> AttributeWithValueContaining(string attributeName,
            string value)
        {
            var pattern = $".*{Regex.Escape(value)}.*";
            return new Conditions.AttributeWithValueMatching(attributeName, pattern);
        }


        public static partial class No
        {
            public static Conditions.Condition<SeleneElement> AttributeWithValueStarting(string attributeName,
                string value)
            {
                var pattern = $"^{Regex.Escape(value)}";
                return new Conditions.AttributeWithValueMatching(attributeName, pattern).Not;
            }

            public static Conditions.Condition<SeleneElement> AttributeWithValueEnding(string attributeName,
                string value)
            {
                var pattern = $"{Regex.Escape(value)}$";
                return new Conditions.AttributeWithValueMatching(attributeName, pattern).Not;
            }

            public static Conditions.Condition<SeleneElement> AttributeWithValueContaining(string attributeName,
                string value)
            {
                var pattern = $".*{Regex.Escape(value)}.*";
                return new Conditions.AttributeWithValueMatching(attributeName, pattern).Not;
            }
        }
    }
}