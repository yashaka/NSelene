using System;

namespace NSelene.Helpers
{

    internal class Ref<T>
    {
        public T Value 
        { 
            get 
            {
                return this.getter();
            }
            set
            {
                this.setter(value);
            }
        }
        private readonly Func<T> getter;
        private readonly Action<T> setter;
        private T value;
        public Ref(T value)
        : this()
        {
            this.value = value;
        }
        public Ref()
        {
            this.getter = () => this.value;
            this.setter = val => this.value = val;
        }
        public Ref(Func<T> getter, Action<T> setter)
        {
            this.setter = setter;
            this.getter = getter;
        }
    }
}