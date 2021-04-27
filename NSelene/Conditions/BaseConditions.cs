using System;
using System.Linq;

namespace NSelene
{
    // TODO: consider refactoring "describing" logic for conditions... 
    namespace Conditions
    {
        public class ConditionNotMatchedException : Exception // TODO: Should we use InvalidOperationException instead?
        {
            public ConditionNotMatchedException() : this("condition not matched")
            {
            }

            public ConditionNotMatchedException(string message) : this(() => message)
            {
            }

            public ConditionNotMatchedException(Func<string> renderMessage) : base("")
            {
                this.RenderMessage = renderMessage;
            }

            public ConditionNotMatchedException(
                string message, 
                Exception innerException
            ) 
            : this(() => message, innerException)
            {
            }

            public ConditionNotMatchedException(
                Func<string> renderMessage,
                Exception innerException
            )
            : base("", innerException)
            {
                this.RenderMessage = renderMessage;
            }

            public Func<string> RenderMessage { get; }

            public override string Message => this.RenderMessage();
        }
        
    // todo: consider renaming to Negated
    //       or maybe just NotCondition
    public class Not<TEntity> : Condition<TEntity>
    {
        private readonly Condition<TEntity> condition;

        public Not(Condition<TEntity> condition)
        {
            this.condition = condition;
        }

        public override void Invoke(TEntity entity)
        {
            try
            {
                this.condition.Invoke(entity);
            }
            catch (Exception)
            {
                return;
            }
            throw new ConditionNotMatchedException();
        }

        public override string ToString()
        {
            return $"not {this.condition}";
        }
    }

        // TODO: find the way to DRY conditions:) Generics? keep it simple enough though...

        public abstract class Condition<TEntity> : _Computation<TEntity, object> // object means void here:D 
        {
            /// Summary:
            ///     Either passes implying that the condition is matched, 
            ///     or fails with Exception impything that the condition is not matched
            ///     When implementing custom conditions and throwing custom exception
            ///     to describe the actual reason of "not mathced"
            ///     the ConditionNotMatchedException is recommended to use;)
            public abstract void Invoke(TEntity entity);

            _Optionally<object> _Computation<TEntity, object>._Invoke(TEntity entity)
            {
                ((Condition<TEntity>)this).Invoke(entity);
                return _Optionally<object>.Undefined;
            }

            public Condition<TEntity> Not 
                => new Not<TEntity>(this);

            [Obsolete(
                "bool Condition#Apply is obsolete use void Condition#Invoke() "
                + "throwing exeption on failure instead, "
                + "or use bool DescribedCondition#Apply"
            )]
            public virtual bool Apply(TEntity entity) // TODO: Consider adding something like bool condition.matched instead, think on matched vs matching vs matches?
            {
                // just a dummy implementation
                try
                {
                    this.Invoke(entity);
                    return true;
                }
                catch (Exception)
                {
                    return false; // actually original Condition.Apply could throw exceptions that will be handled in old Selene.WaitFor ... o_O ?
                }
            }

            // TODO: consider adding implicit conversion to delegate Func<TEntity, bool>
            //       but ensure that it will not interfere with Should(Expression<Func<TEntity, bool>>)
            //       cause probably we would want to allow user implement custom conditions as delegates of in-place style
            /*
            public static implicit operator Func<TEntity, bool>(Condition<TEntity> condition)
            {
                return delegate(TEntity entity) {
                    try
                    {
                        condition.Invoke(entity);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                };
            }
            */

            // TODO: would implicit or explicit operator be better?
            // TODO: would simple public bool Matched(TEntity entity) be better?
            //       seems like yes, compare:
            //           Assert.IsFalse(new HasTruthyElement(1).Not._Predicate(new [] {1, 2}));
            //       vs
            //           Assert.IsFalse(new HasTruthyElement(1).Not.Matched(new [] {1, 2}));
            public Func<TEntity, bool> _Predicate // TODO: consider as method of ToPredicate or AsPredicate naming style
            => delegate (TEntity entity)
            {
                try
                {
                    this.Invoke(entity);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            };

            public override string ToString()
            {
                return this.GetType().Name;
            }

            [Obsolete("condition.Explain is obsolete use condition.ToString() instead")]
            public virtual string Explain()
            {
                return this.ToString();
            }
        }

        public abstract class DescribedCondition<TEntity> : Condition<TEntity>
        {
            public abstract new bool Apply(TEntity entity);

            public override void Invoke(TEntity entity)
            {
                var result = this.Apply(entity);
                if (!result)
                {
                    throw new ConditionNotMatchedException("actual: " + this.DescribeActual());
                }
            }

            public virtual string DescribeActual()
            {
                // TODO: consider providing more "universal" approach... 
                //       because this makes sense only 
                //       in case of failed condition:(
                return false.ToString(); 
            }

            public virtual string DescribeExpected()
            {
                return true.ToString();
            }

            [Obsolete("condition.Explain is obsolete use condition.ToString() instead")]
            public override string Explain()
            {
                return this.ToString();
            }

            public override string ToString()
            {
                var expected = this.DescribeExpected();

                return expected == true.ToString() 
                ? this.GetType().Name 
                : expected;
            }
        }
    }

}
