
namespace NSeleneTests.Unit
{

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
            Assert.That(new HasTruthyElement(1)._Predicate(new[] { 1, 2 }), Is.True);
        }

        [Test]
        public void AnswersFalse_AppliedTo_Falsy()
        {
            Assert.That(new HasTruthyElement(1)._Predicate(new[] { 0, 2 }), Is.False);
        }

        [Test]
        public void RaiseException_AppliedTo_FalsyExceptionLike()
        {
            Assert.Catch<IndexOutOfRangeException>(
                () => new HasTruthyElement(3).Invoke(new [] {1, 2, /*no third*/})
            );
            Assert.That(new HasTruthyElement(3)._Predicate(new[] { 1, 2, /*no third*/}), Is.False);
        }
    }
}