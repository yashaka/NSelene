using System;
using System.Text.RegularExpressions;

namespace NSelene
{
    namespace Conditions
    {
        public class AttributeWithValueMatching : Condition<SeleneElement>
        {
            private string name;
            private string pattern;

            public AttributeWithValueMatching(string name, string pattern)
            {
                this.name = name;
                this.pattern = pattern;
            }

            public override string ToString()
            {
                return $"Have.Attribute({this.name} matching regex pattern: «{this.pattern}»)";
            }

            public override void Invoke(SeleneElement entity)
            {
                var webelement = entity.ActualWebElement;
                var maybeActual = webelement.GetAttribute(this.name);

                if (maybeActual == null || !Regex.IsMatch(maybeActual, pattern))
                {
                    throw new ConditionNotMatchedException(() =>
                        (maybeActual == null
                            ? $"Actual {this.name}: Null (attribute is absent){Environment.NewLine}"
                            : $"Actual {this.name}: «{maybeActual}»{Environment.NewLine}"
                        )
                        + (
                            entity.Config.LogOuterHtmlOnFailure ?? false
                                ? $"Actual webelement: {webelement.GetAttribute("outerHTML")}"
                                : ""
                        )
                    );
                }
            }
        }
    }

    public static partial class Have
    {
        public static Conditions.Condition<SeleneElement> AttributeWithValueStarting(string name, string value)
        {
            var pattern = $"^{Regex.Escape(value)}";
            return new Conditions.AttributeWithValueMatching(name, pattern);
        }

        public static Conditions.Condition<SeleneElement> AttributeWithValueEnding(string name, string value)
        {
            var pattern = $"{Regex.Escape(value)}$";
            return new Conditions.AttributeWithValueMatching(name, pattern);
        }

        public static Conditions.Condition<SeleneElement> AttributeWithValueContaining(string name,
            string value)
        {
            var pattern = $".*{Regex.Escape(value)}.*";
            return new Conditions.AttributeWithValueMatching(name, pattern);
        }


        public static partial class No
        {
            public static Conditions.Condition<SeleneElement> AttributeWithValueStarting(string name,
                string value)
            {
                var pattern = $"^{Regex.Escape(value)}";
                return new Conditions.AttributeWithValueMatching(name, pattern).Not;
            }

            public static Conditions.Condition<SeleneElement> AttributeWithValueEnding(string name,
                string value)
            {
                var pattern = $"{Regex.Escape(value)}$";
                return new Conditions.AttributeWithValueMatching(name, pattern).Not;
            }

            public static Conditions.Condition<SeleneElement> AttributeWithValueContaining(string name,
                string value)
            {
                var pattern = $".*{Regex.Escape(value)}.*";
                return new Conditions.AttributeWithValueMatching(name, pattern).Not;
            }
        }
    }
}