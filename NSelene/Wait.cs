using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;

namespace NSelene
{
    internal interface Command<E>
    {
        void Call(E entity);
        string ToString();
    }

    internal interface Query<E, R>
    {
        R Call(E entity);
        string ToString();
    }

    public struct _Optionally<T>
    {
        private readonly T[] _value;

        public T Value
        {
            get
            {
                Contract.Requires(HasSomething);

                return _value[0];
            }
        }

        public bool HasSomething => _value.Length > 0;

        public bool HasNothing => !HasSomething;

        private _Optionally(T[] value)
        {
            _value = value;
        }

        public static _Optionally<T> Defined(T obj) => new _Optionally<T>(new T[] { obj });

        public static _Optionally<T> Undefined => new _Optionally<T>(new T[] { });
    }

    // TODO: SO... Computation vs Operation? - which is better? as a name... 
    public interface _Computation<TEntity, TResult> // TODO: TResult might be missed/undefined... how to reflect it?
    {
        _Optionally<TResult> _Invoke(TEntity entity); // TODO: should we name it as Perform?
        // string ToString(); // TODO: do we need to make this definition explicit?
    }

    /// Summary:
    ///     Kind of "named lambda".
    ///     called Lambda to represent its lambda-like nature of eather return value or be void
    /// TODO: keep it pseudo-internal (as _pre-underscored) unless sure about result of Invoke method :)
    ///       open points: should we return option-like Tuple instead of Optionally ?
    public class _Lambda<TEntity, TResult> : _Computation<TEntity, TResult> // TODO: consider naming it as Task/Calling/Operation/Computation/DescribedComputation/NamedComputation
    {
        // TODO: consider public Action and Func, that build itself based on each other ;)
        //       like given func defined, the action would just call it without returning anything.
        //       and  given action defined, the func would return something like null... ? 
        Action<TEntity> MaybeAction { get; }      // TODO: rename to MaybeAction? ;) cause might be null
        Func<TEntity, TResult> MaybeFunc { get; } // TODO: rename to MaybeFunc? ;) ...

        string Name { get; }

        internal _Lambda(Expression<Action<TEntity>> action)
        : this(action.ToString(), action.Compile(), null)
        { }

        internal _Lambda(Expression<Func<TEntity, TResult>> func)
        : this(func.ToString(), null, func.Compile())
        { }

        internal _Lambda(string name, Action<TEntity> action)
        : this(name, action, null)
        { }

        internal _Lambda(string name, Func<TEntity, TResult> func)
        : this(name, null, func)
        { }

        private _Lambda(
            string name,
            Action<TEntity> action,
            Func<TEntity, TResult> func
        )
        {
            MaybeAction = action;
            MaybeFunc = func;
            Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }

        // TODO: move from TResult to Maybe<TResult>
        public _Optionally<TResult> _Invoke(TEntity entity)  // TODO: should we name it Call like in selenidejs?
        {
            if (this.MaybeFunc != null)
            {
                return _Optionally<TResult>.Defined(this.MaybeFunc(entity));
            }
            this.MaybeAction(entity);
            return _Optionally<TResult>.Undefined;
        }
    }

    internal class Wait<T>
    {

        private readonly T entity;
        private readonly double timeout;
        private readonly double polling;
        private readonly Func<string, string> describeLambdaName;

        public Wait(
            T entity,
            double timeout,
            double polling,
            Func<string, string> _describeLambdaName = null
        )
        {
            this.entity = entity;
            this.timeout = timeout;
            this.polling = polling;
            this.describeLambdaName = _describeLambdaName ?? (name => name);
        }

        public bool Until(Expression<Action<T>> action) => Until(new _Lambda<T, object>(action));
        public bool Until<R>(Expression<Func<T, R>> func) => Until(new _Lambda<T, R>(func));

        internal bool Until<R>(_Lambda<T, R> lambda)
        {
            try
            {
                this.For(lambda);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        internal void For(Expression<Action<T>> action)
        {
            this.For(new _Lambda<T, object>(action));
        }
        internal R For<R>(Expression<Func<T, R>> func)
        {
            var optional = this.For(new _Lambda<T, R>(func));
            return optional.Value;
        }

        internal _Optionally<R> For<R>(_Computation<T, R> computation) // TODO: should we accept an interface here? make Lambda an interface? or add Operation interface?
        {
            var timeoutSpan = TimeSpan.FromSeconds(this.timeout);
            var finishTime = DateTime.Now.Add(timeoutSpan);

            // System.Exception failFastError; // TODO: consider some failfast logic...
            while (true)
            {
                try
                {
                    return computation._Invoke(this.entity);
                }
                // catch (InvalidCastException error)
                // {
                //     failFastError = error;
                //     break;
                // }
                catch (System.Exception error)
                {
                    if (DateTime.Now > finishTime)
                    {
                        // TODO: should we move this error formatting to the Error class definition?
                        var describedLambda = this.describeLambdaName(computation.ToString());
                        var failure = new TimeoutException(
                            "\n"
                            + $"\tTimed out after {this.timeout}s, while waiting for:\n"
                            + $"\t{this.entity}.{describedLambda}\n" // TODO: handle trailing spaces
                            + "Reason:\n"
                            + $"\t{error.Message}"
                            ,
                            error
                        );

                        throw failure;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(this.polling).Milliseconds);
                }
            }
            // throw failFastError;
        }
    }
}