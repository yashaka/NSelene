using System;
using System.Collections.Generic;
using System.Text;

namespace NSelene.Conditions
{
    public class Not<TEntity> : DescribedCondition<TEntity>
    {
        private readonly DescribedCondition<TEntity> condition;

        public Not(DescribedCondition<TEntity> condition)
        {
            this.condition = condition;
        }

        public override bool Apply(TEntity entity)
        {
            return ! condition.Apply(entity);
        }

        public override string DescribeActual()
        {
            return $"not {condition.DescribeActual()}";
        }

        public override string DescribeExpected()
        {
            return $"not {condition.DescribeActual()}";
        }
    }
}
