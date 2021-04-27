
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

            public override void Invoke(int[] entity)
            {
                var actual = entity[this.number-1];
                if (actual == 0)
                {
                    throw new Exception($"Condition failed, reason: actual = {0}");
                }
            }
        }

        [Test]
        public void AnswersFalse_AppliedTo_Truthy()
        {
            Assert.IsFalse(new HasTruthyElement(1).Not._Predicate(new [] {1, 2}));
        }

        [Test]
        public void AnswersTrue_AppliedTo_Falsy()
        {
            Assert.IsTrue(new HasTruthyElement(1).Not._Predicate(new [] {0, 2}));
        }

        [Test]
        public void AnswersTrue_AppliedTo_FalsyExceptionLike()
        {
            Assert.IsTrue(new HasTruthyElement(3).Not._Predicate(new [] {1, 2, /*no third*/}));
        }
    }
}