using System.Collections.Generic;
using System.Linq;
using NSelene.Conditions;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
{
    namespace Conditions
    {
        public class ByStringContainsComparer : IEqualityComparer<string> {
            public bool Equals(string x, string y) {
                return (x.Contains(y));
            }

            public int GetHashCode(string obj) {
                return obj.GetHashCode();
            }
        }

        // TODO: ensure messages are relevant

        public class Texts : DescribedCondition<SeleneCollection>
        {

            protected string[] expected;
            protected string[] actual;

            public Texts(params string[] expectedTexts)
            {
                this.expected = expectedTexts;
            }

            public override bool Apply(SeleneCollection entity)
            {
                this.actual = entity.ActualWebElements
                                    .Select(element => element.Text)
                                    .ToArray(); //TODO: do we need conversion to array?

                return this.actual.SequenceEqual(this.expected, new ByStringContainsComparer());
            }

            public override string DescribeActual()
            {
                return "[" + string.Join(",", this.actual) + "]";
            }

            public override string DescribeExpected()
            {
                return "[" + string.Join(",", this.expected) + "]";
            }
        }

        public class ExactTexts : Texts
        {
            public ExactTexts(params string[] expected) : base(expected) {}

            public override bool Apply(SeleneCollection entity)
            {
                this.actual = entity.ActualWebElements.Select(element => element.Text).ToArray();
                return this.actual.SequenceEqual(this.expected);
            }
        }

    }

    public static partial class Have
    {
        public static Condition<SeleneCollection> Texts(params string[] expected) 
        => new Conditions.Texts(expected);

        public static Condition<SeleneCollection> ExactTexts(params string[] expected) 
        => new Conditions.ExactTexts(expected);

        static partial class No
        {
            public static Condition<SeleneCollection> Texts(params string[] expected) 
            => new Conditions.Texts(expected).Not;

            public static Condition<SeleneCollection> ExactTexts(params string[] expected) 
            => new Conditions.ExactTexts(expected).Not;
        }
    }

}
