
using System;
using NSelene.Conditions;
using NUnit.Framework;

namespace NSeleneTests.Unit {

    [TestFixture]
    public class ConditionSpecs
    {

        class HasTruthyElement : Condition<int[]>
        {
            private readonly int number;

            public HasTruthyElement(int number)
            {
                this.number = number;
            }

            public override bool Apply(int[] entity)
            {
                return entity[this.number-1] != 0;
            }
        }

        [Test]
        public void AnswersTrue_AppliedTo_Truthy()
        {
            Assert.IsTrue(new HasTruthyElement(1).Apply(new [] {1, 2}));
        }

        [Test]
        public void AnswersFalse_AppliedTo_Falsy()
        {
            Assert.IsFalse(new HasTruthyElement(1).Apply(new [] {0, 2}));
        }

        [Test]
        public void RaiseException_AppliedTo_FalsyExceptionLike()
        {
            Assert.Catch<Exception>(
                () => new HasTruthyElement(3).Apply(new [] {1, 2, /*no third*/})
            );
        }
    }
}