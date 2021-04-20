﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NSelene.Conditions
{
    // todo: consider renaming to Negated
    //       or maybe just NotCondition
    public class Not<TEntity> : Condition<TEntity>
    {
        private readonly Condition<TEntity> condition;

        public Not(Condition<TEntity> condition)
        {
            this.condition = condition;
        }

        public override bool Apply(TEntity entity)
        {
            try
            {
                return !condition.Apply(entity);
            }
            catch
            {
                return true;
            }
        }

        public override string Explain()
        {
            return $"not {condition.Explain()}";
        }

        public override string DescribeActual()
        {
            var original = condition.DescribeActual();
            return original == false.ToString() ? false.ToString()
                : original;
        }

        // public override string DescribeExpected()
        // {
        //     var original = condition.DescribeExpected();
        //     return original == true.ToString() ? true.ToString()
        //         : $"not {original}";
        // }

        public override _Optionally<object> _Invoke(TEntity entity)
        {
            bool result;
            try
            {
                result = !this.condition.Apply(entity);
            }
            catch (Exception)
            {
                result = true;
            }
            if (!result)
            {
                throw new ConditionNotMatchedException("actual: " + this.DescribeActual());
            }
            return _Optionally<object>.Undefined;
        }
    }
}
