using System;
using System.Collections.Generic;
using System.Drawing;
using NSelene.Conditions;
using OpenQA.Selenium;
using System.Linq;
using System.Collections.ObjectModel;

namespace NSelene
{
    // TODO: consider removing public modifier for SContext interface and all SLocator implementations

    // TODO: consider renaming to either ISeleniumContext, IWebContext, IWebSearchContext, ISeleniumSearchContext
    public interface SeleneContext
    {
        // TODO: consider renaming to FindWebElement
        IWebElement FindElement (By by);
        ReadOnlyCollection<IWebElement> FindElements (By by);
    }

    // TODO: consider making it internal
    public abstract class SeleneLocator<TEntity>
    {
        public abstract string Description { get; }
        public abstract TEntity Find();

        public override string ToString ()
        {
            return Description;
        }
    }

    public abstract class WebElementSLocator : SeleneLocator<IWebElement>
    {
    }

    sealed class SearchContextWebElementSLocator : WebElementSLocator
    {
        readonly SeleneContext context;
        readonly By driverLocator;

        public SearchContextWebElementSLocator(By driverLocator, SeleneContext context)
        {
            this.driverLocator = driverLocator;
            this.context = context;
        }

        public override string Description {
            get {
                return (
                    $"{this.context}.Element({this.driverLocator})"
                    .Replace("By.CssSelector: ", "")
                    .Replace("By.XPath: ", "")
                    .Replace("NSelene.Configuration", "Browser") // TODO: remove once refactored
                );
            }
        }

        public override IWebElement Find ()
        {
            return this.context.FindElement(this.driverLocator);
        }
    }

    sealed class WrappedWebElementSLocator : WebElementSLocator
    {
        readonly string description;
        readonly IWebElement webelement;

        public WrappedWebElementSLocator(string description, IWebElement webelement)
        {
            this.webelement = webelement;
            this.description = description;
        }

        public WrappedWebElementSLocator(IWebElement webelement)
            : this("wrapped webelement", webelement) {}

        public override string Description {
            get {
                return this.description + ": " + this.webelement;
            }
        }

        public override IWebElement Find ()
        {
            return this.webelement;
        }
    }

    sealed class SCollectionWebElementByIndexSLocator : WebElementSLocator
    {
        readonly int index;
        readonly SeleneCollection context;

        public SCollectionWebElementByIndexSLocator(int index, SeleneCollection context)
        {
            this.index = index;
            this.context = context;
        }

        public override string Description {
            get {
                return $"{this.context}[{index}]";
            }
        }

        public override IWebElement Find ()
        {
            return this.context.Should(Have.CountAtLeast(this.index+1)).ActualWebElements[index];
        }
    }

    sealed class SCollectionWebElementByConditionSLocator : WebElementSLocator
    {
        readonly Condition<SeleneElement> condition;
        readonly SeleneCollection context;
        readonly _SeleneSettings_ config;

        public SCollectionWebElementByConditionSLocator(
            Condition<SeleneElement> condition, 
            SeleneCollection context, 
            _SeleneSettings_ config
        )
        {
            this.condition = condition;
            this.context = context;
            this.config = config;
        }

        public override string Description {
            get {
                return $"{this.context}.FirstBy({this.condition})";
            }
        }

        public override IWebElement Find ()
        {
            var webelments = this.context.ActualWebElements;

            Predicate<IWebElement> byCondition = delegate(IWebElement element) {
                return condition._Predicate(
                    new SeleneElement(
                        new WrappedWebElementSLocator(
                            string.Format("By.Selene: ({0}).FindBy({1})", this.context, condition)
                            , element), this.config)
                );/* 
                   * ??? TODO: do we actually need here so meaningful desctiption?
                   * does it make sense to use it but to put index for each element?
                   * via using FindIndex ?
                   */
            };

            var found = webelments.ToList()
                                  .Find(byCondition);
            if (found == null) 
            {
                var actualTexts = webelments.ToList().Select(element => element.Text).ToArray();
                var htmlelements = webelments.ToList().Select(element => element.GetAttribute("outerHTML")).ToArray();
                throw new NotFoundException("element was not found in collection by condition "
                                            + condition
                                            + "\n  Actual visible texts : " + "[" + string.Join(",", actualTexts) + "]"  // TODO: think: this line is actually needed in the case of FindBy(ExactText ...) ... is there any way to add such information not here?
                                            + "\n  Actual html elements : " + "[" + string.Join(",", htmlelements) + "]" 
                                            // TODO: should we add here some other info about elements? e.g. visiblitiy?
                                           );
                /*
                     * TODO: think on better messages
                     */
            }
            return found;
        }
    }

    // TODO: maybe SLocator<ReadOnlyCollection<IWebElement>> ?
    abstract class WebElementsCollectionSLocator : SeleneLocator<ReadOnlyCollection<IWebElement>>
    {
    }

    sealed class SearchContextWebElementsCollectionSLocator : WebElementsCollectionSLocator
    {
        readonly SeleneContext context;
        readonly By driverLocator;

        public SearchContextWebElementsCollectionSLocator(
            By driverLocator, 
            SeleneContext context
        )
        {
            this.driverLocator = driverLocator;
            this.context = context;
        }

        public override string Description {
            get {
                return (
                    $"{this.context}.All({this.driverLocator})"
                    .Replace("By.CssSelector: ", "")  // TODO: DRY it and make configurable
                    .Replace("By.XPath: ", "")
                    .Replace("NSelene.Configuration", "Browser") // TODO: remove once refactored
                );
            }
        }

        public override ReadOnlyCollection<IWebElement> Find ()
        {
            return this.context.FindElements(this.driverLocator);
        }
    }

    sealed class WrappedWebElementsCollectionSLocator : WebElementsCollectionSLocator
    {
        readonly string description;
        readonly IList<IWebElement> webelements;

        public WrappedWebElementsCollectionSLocator(string description, IList<IWebElement> webelements)
        {
            this.webelements = webelements;
            this.description = description;
        }

        public WrappedWebElementsCollectionSLocator(IList<IWebElement> webelements) 
            : this("wrapped collection", webelements) {}  // TODO: think on better description

        public override string Description {
            get {
                return this.description + ": " + webelements.ToString();
            }
        }

        public override ReadOnlyCollection<IWebElement> Find()
        {
            return new ReadOnlyCollection<IWebElement>(this.webelements);  // TODO: think on switching SCollection impl to be based on IList<IWebElement>
        }
    }

    sealed class SCollectionFilteredWebElementsCollectionSLocator 
    : WebElementsCollectionSLocator
    {
        readonly Condition<SeleneElement> condition;
        readonly SeleneCollection context;
        readonly _SeleneSettings_ config;

        public SCollectionFilteredWebElementsCollectionSLocator(
            Condition<SeleneElement> condition, 
            SeleneCollection context, 
            _SeleneSettings_ config
        )
        {
            this.condition = condition;
            this.context = context;
            this.config = config;
        }

        public override string Description {
            get {
                return $"{this.context}.By({this.condition})";
            }
        }

        public override ReadOnlyCollection<IWebElement> Find ()
        {

            Func<IWebElement, bool> byCondition = delegate(IWebElement element) {
                return condition._Predicate(
                    new SeleneElement(
                        new WrappedWebElementSLocator(
                            $"{this.context}.FindBy({this.condition})"
                            , element
                        ), 
                        this.config
                    )
                );
            };

            return new ReadOnlyCollection<IWebElement>(  
                // TODO: don't we need here ReadONlyCollection<SElement> ?
                context.ActualWebElements.Where(byCondition).ToList());
        }
    }
}

