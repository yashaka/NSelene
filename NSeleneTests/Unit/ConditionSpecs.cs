
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
        public void AnswersTrue_AppliedTo_Truthy()
        {
            Assert.IsTrue(new HasTruthyElement(1)._Predicate(new [] {1, 2}));
        }

        [Test]
        public void AnswersFalse_AppliedTo_Falsy()
        {
            Assert.IsFalse(new HasTruthyElement(1)._Predicate(new [] {0, 2}));
        }

        [Test]
        public void RaiseException_AppliedTo_FalsyExceptionLike()
        {
            Assert.Catch<IndexOutOfRangeException>(
                () => new HasTruthyElement(3).Invoke(new [] {1, 2, /*no third*/})
            );
            Assert.IsFalse(new HasTruthyElement(3)._Predicate(new [] {1, 2, /*no third*/}));
        }
    }
}