
using System;
using NSelene.Conditions;
using NUnit.Framework;

namespace NSeleneTests.Unit {

    [TestFixture]
    public class ConditionNotSpecs
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
                try
                {
                    return entity[this.number-1] != 0;
                }
                catch (Exception reason)
                {
                    throw new Exception("Condition failed, reason: " + reason.Message);
                }
            }
        }

        [Test]
        public void AnswersFalse_AppliedTo_Truthy()
        {
            Assert.IsFalse(new HasTruthyElement(1).Not.Apply(new [] {1, 2}));
        }

        [Test]
        public void AnswersTrue_AppliedTo_Falsy()
        {
            Assert.IsTrue(new HasTruthyElement(1).Not.Apply(new [] {0, 2}));
        }

        [Test]
        public void AnswersTrue_AppliedTo_FalsyExceptionLike()
        {
            Assert.IsTrue(new HasTruthyElement(3).Not.Apply(new [] {1, 2, /*no third*/}));
        }
    }
}