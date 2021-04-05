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

public struct Optionally<T>
{
    private readonly T [] _value;
 
    public T Value
    {
        get
        {
            Contract.Requires(HasSomething);
 
            return _value[0];
        }
    }
 
    public bool HasSomething
    {
        get { return _value.Length > 0; }
    }
 
    public bool HasNothing
    {
        get { return !HasSomething; }
    }
 
    private Optionally(T [] value)
    {
        _value = value;
    }

    public static Optionally<T> Defined(T obj)
    {
        return new Optionally<T>(new T [] {obj});
    }

    public static Optionally<T> Undefined
    => new Optionally<T>(new T [] {});
}

    internal class _Lambda<TEntity, TResult>
    {
        Action<TEntity> Action { get; }
        Func<TEntity, TResult> Func { get; }
        string Name { get; }

        internal _Lambda(Expression<Action<TEntity>> action) 
        : this(action.ToString(), action.Compile(), null) 
        {}

        internal _Lambda(Expression<Func<TEntity, TResult>> func)
        : this(func.ToString(), null, func.Compile())
        {}

        private _Lambda(
            string name, 
            Action<TEntity> action, 
            Func<TEntity, TResult> func
        )
        {
            Action = action;
            Func = func;
            Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }

        // TODO: move from TResult to Maybe<TResult>
        internal Optionally<TResult> Invoke(TEntity entity)  // TODO: should we name it Call like in selenidejs?
        {
            if (this.Func != null)
            {
                return Optionally<TResult>.Defined(this.Func(entity));
            }
            this.Action(entity);
            return Optionally<TResult>.Undefined;
        }
    }

internal class Wait<T> {

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

    internal Optionally<R> For<R>(_Lambda<T, R> lambda)
    {
        
        var timeoutSpan = TimeSpan.FromSeconds(this.timeout);
        var finishTime = DateTime.Now.Add(timeoutSpan);

        while (true) {
            try 
            {
                return lambda.Invoke(this.entity);
            } 
            catch (System.Exception error) 
            {
                if (DateTime.Now > finishTime) {
                    // TODO: should we move this error formatting to the Error class definition?
                    var describedLambda = this.describeLambdaName(lambda.ToString());
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
    }
}
}