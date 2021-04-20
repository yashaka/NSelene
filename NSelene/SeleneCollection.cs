using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using NSelene.Conditions;
using System.Collections.ObjectModel;
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace NSelene
{
    public interface WrapsWebElementsCollection
    {
        ReadOnlyCollection<IWebElement> ActualWebElements { get; }
    }

    public sealed class SeleneCollection 
        :  WrapsWebElementsCollection
        , IReadOnlyList<SeleneElement>
        , IReadOnlyCollection<SeleneElement>
        , IList<SeleneElement>
        , IList<IWebElement>
        , ICollection<SeleneElement>
        , IEnumerable<SeleneElement>
        , IEnumerable
    {
        readonly SeleneLocator<ReadOnlyCollection<IWebElement>> locator;

        public readonly _SeleneSettings_ config; // TODO: remove
        // private readonly _SeleneSettings_ config;
        
        internal SeleneCollection(
            SeleneLocator<ReadOnlyCollection<IWebElement>> locator, 
            _SeleneSettings_ config
        ) 
        {
            this.locator = locator;
            this.config = config;
        }

        internal SeleneCollection(
            SeleneLocator<ReadOnlyCollection<IWebElement>> locator
        )
        : this(locator, Configuration.Shared) {}        
        
        internal SeleneCollection(
            By locator, 
            _SeleneSettings_ config
        ) 
        : this (
            new SearchContextWebElementsCollectionSLocator(
                locator, 
                config
            ),
            config
        ) {}

        internal SeleneCollection(
            IList<IWebElement> elementsListToWrap, 
            _SeleneSettings_ config
        )
        : this(
            new WrappedWebElementsCollectionSLocator(elementsListToWrap), 
            config
        ) 
        {}

        public SeleneCollection With(
            IWebDriver driver = null,
            double? timeout = null,
            double? pollDuringWaits = null,
            bool? setValueByJs = null
        )
        {
            _SeleneSettings_ customized = new Configuration();

            customized.Driver = driver;
            customized.Timeout = timeout;
            customized.PollDuringWaits = pollDuringWaits;
            customized.SetValueByJs = setValueByJs;

            /* same but another style and not so obvious with harder override logic: 
            // mentioned here just for an example, to think about later on API improvements

            _SeleneSettings_ customized = Configuration._With_(
                driver: driver ?? this.config.Driver,
                timeout: timeout ?? this.config.Timeout,
                pollDuringWaits: pollDuringWaits ?? this.config.PollDuringWaits,
                setValueByJs: setValueByJs ?? this.config.SetValueByJs
            );
            */

            return new SeleneCollection(
                this.locator, 
                this.config.With(customized)
            );
        }

        public SeleneCollection _With_(_SeleneSettings_ config)
        {
            return new SeleneCollection(
                this.locator, 
                config
            );
        }
        
        public ReadOnlyCollection<IWebElement> ActualWebElements
        {
            get {
                return locator.Find();
            }
        }
        Wait<SeleneCollection> Wait
        {
            get
            {
                var paramsAndTheirUsagePattern = new Regex(@"\(?(\w+)\)?\s*=>\s*?\1\.");
                return new Wait<SeleneCollection>(
                    entity: this,
                    timeout: this.config.Timeout ?? Configuration.Timeout,
                    polling: this.config.PollDuringWaits ?? Configuration.PollDuringWaits,
                    _describeLambdaName: it => paramsAndTheirUsagePattern.Replace(
                        it, 
                        ""
                    )
                );
            }
        }

        // TODO: consider depracating
        SeleneLocator<ReadOnlyCollection<IWebElement>> SLocator 
        {
            get {
                return this.locator;
            }
        }

        public override string ToString()
        {
            return this.locator.Description;
        }

        public SeleneCollection Should(Condition<SeleneCollection> condition)
        {
            this.Wait.For(condition);
            return this;
        }

        [Obsolete("Use the negative condition instead, e.g. Should(Have.No.Count(0))")]
        public SeleneCollection ShouldNot(Condition<SeleneCollection> condition)
        {
            return this.Should(condition.Not);
        }

        public bool Matching(Condition<SeleneCollection> condition)
        {
            try 
            {
                return condition.Apply(this);
            }
            catch
            {
                return false;
            }
        }
        public bool WaitUntil(Condition<SeleneCollection> condition)
        {
            try 
            {
                this.Should(condition);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public SeleneElement FindBy(Condition<SeleneElement> condition)
        {
            return new SeleneElement(
                new SCollectionWebElementByConditionSLocator(condition, this, this.config), 
                this.config
            );
        }

        public SeleneCollection FilterBy(Condition<SeleneElement> condition)
        {
            return new SeleneCollection(
                new SCollectionFilteredWebElementsCollectionSLocator(
                    condition, this, this.config
                ), 
                this.config
            );
        }

        public ReadOnlyCollection<IWebElement> ToReadOnlyWebElementsCollection()
        {
            return new ReadOnlyCollection<IWebElement>(this);
        }

        //
        // IReadOnlyList
        //

        public SeleneElement this [int index] {
            get {
                return new SeleneElement(
                    new SCollectionWebElementByIndexSLocator(index, this), 
                    this.config
                );
            }
        }

        //
        // IReadOnlyCollection
        //

        public int Count
        {
            get {
                return this.ActualWebElements.Count;
            }
        }

        //
        // IEnumerator
        //

        //TODO: is it stable enought in context of "ajax friendly"?
        public IEnumerator<SeleneElement> GetEnumerator ()
        {
            //TODO: is it lazy? seems like not... because of ToList() conversion? should it be lazy?
            return new ReadOnlyCollection<SeleneElement>(
                this.ActualWebElements.Select(
                    webelement 
                    => 
                    new SeleneElement(webelement, this.config)).ToList()
                ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        //
        // IList<IWebElement> methods
        //

        int ICollection<IWebElement>.Count {
            get {
                return this.Count;
            }
        }

        bool ICollection<IWebElement>.IsReadOnly {
            get {
                return true;
            }
        }

        IWebElement IList<IWebElement>.this [int index] {
            get {
                return this[index];
            }

            set {
                throw new NotImplementedException ();
            }
        }

        int IList<IWebElement>.IndexOf (IWebElement item)
        {
            throw new NotImplementedException ();
        }

        void IList<IWebElement>.Insert (int index, IWebElement item)
        {
            throw new NotImplementedException ();
        }

        void IList<IWebElement>.RemoveAt (int index)
        {
            throw new NotImplementedException ();
        }

        void ICollection<IWebElement>.Add (IWebElement item)
        {
            throw new NotImplementedException ();
        }

        void ICollection<IWebElement>.Clear ()
        {
            throw new NotImplementedException ();
        }

        bool ICollection<IWebElement>.Contains (IWebElement item)
        {
            throw new NotImplementedException ();
        }

        void ICollection<IWebElement>.CopyTo (IWebElement [] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Rank > 1)
                throw new ArgumentException("array is multidimensional.");
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Not enough elements after index in the destination array.");

            for (int i = 0; i < Count; ++i)
                array.SetValue(this[i], i + arrayIndex);
        }

        bool ICollection<IWebElement>.Remove (IWebElement item)
        {
            throw new NotImplementedException ();
        }

        IEnumerator<IWebElement> IEnumerable<IWebElement>.GetEnumerator ()
        {
            return this.GetEnumerator();
        }

        //
        // IList<SElement> methods
        //

        bool ICollection<SeleneElement>.IsReadOnly {
            get {
                return true;
            }
        }

        SeleneElement IList<SeleneElement>.this [int index] {
            get {
                return this[index];
            }

            set {
                throw new NotImplementedException ();
            }
        }

        int IList<SeleneElement>.IndexOf (SeleneElement item)
        {
            throw new NotImplementedException ();
        }

        void IList<SeleneElement>.Insert (int index, SeleneElement item)
        {
            throw new NotImplementedException ();
        }

        void IList<SeleneElement>.RemoveAt (int index)
        {
            throw new NotImplementedException ();
        }

        void ICollection<SeleneElement>.Add (SeleneElement item)
        {
            throw new NotImplementedException ();
        }

        void ICollection<SeleneElement>.Clear ()
        {
            throw new NotImplementedException ();
        }

        bool ICollection<SeleneElement>.Contains (SeleneElement item)
        {
            throw new NotImplementedException ();
        }

        void ICollection<SeleneElement>.CopyTo (SeleneElement [] array, int arrayIndex)
        {
            throw new NotImplementedException ();
        }

        bool ICollection<SeleneElement>.Remove (SeleneElement item)
        {
            throw new NotImplementedException ();
        }
    }



    namespace Support.Extensions 
    {
        public static class SeleneCollectionExtensions 
        {
            public static SeleneCollection AssertTo(this SeleneCollection selements, Condition<SeleneCollection> condition)
            {
                return selements.Should(condition);
            }

            [Obsolete("Use the negative condition instead, e.g. AssertTo(Have.No.Count(0))")]
            public static SeleneCollection AssertToNot(this SeleneCollection selements, Condition<SeleneCollection> condition)
            {
                return selements.ShouldNot(condition);
            }
        }
    }
}
