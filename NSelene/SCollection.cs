using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using NSelene.Conditions;
using System.Collections.ObjectModel;

namespace NSelene
{
    /*
     * :) It's a very bad way to to like this, 
     * but it's extremely fast comparing to implementing own Bys as wrappers around Selenium ones 
     *
     * TODO: and btw move it some proper place */
    class PseudoBy : By
    {
        public PseudoBy(string description)
        {
            this.Description = description;
        }
    }

    //public delegate IReadOnlyCollection<IWebElement> SCollection();

    public delegate IReadOnlyCollection<IWebElement> FindsAllWebElements();

    public interface GetsAllWebElements
    {
        FindsAllWebElements GetAllActualWebElements { get; }
    }

    public sealed class SCollection : GetsAllWebElements
    {
        readonly By locator;
        readonly FindsAllWebElements finder;

        public SCollection(By locator, FindsAllWebElements finder)
        {
            this.locator = locator;
            this.finder = finder;
        }

        public SCollection(By locator, IWebDriver driver) 
            : this(locator, () => driver.FindElements(locator)) {}

        public SCollection(By locator) 
            : this(locator, () => Utils.GetDriver().FindElements(locator)) {}

        public FindsAllWebElements GetAllActualWebElements {
            get {
                return this.finder;
            }
        }

        public override string ToString()
        {
            return this.locator.ToString();
        }
    }

    public static partial class Utils
    {
        public static SCollection SS(By locator)
        {
            return new SCollection(locator);
        }

        public static SCollection SS(string cssSelector)
        {
            return SS(By.CssSelector(cssSelector));
        }
    }

    public static class SCollectionExtensions
    {

        public static SCollection Should(this SCollection elements, Condition<SCollection> condition)
        {
            return Utils.WaitFor(elements, condition);
        }

        public static SCollection ShouldNot(this SCollection elements, Condition<SCollection> condition)
        {
            return Utils.WaitForNot(elements, condition);
        }

        public static SElement Get(this SCollection elements, int index)
        {
            return elements.ElementAt(index);
        }

//        public static SElement First(this SCollection elements)
//        {
//            return elements.ElementAt(0);
//        }

        public static SElement ElementAt(this SCollection elements, int index)
        {
            // TODO: refactor following usage from elements to elements.Loacator
            return new SElement(new PseudoBy(string.Format("By.Selene: ({0})[{1}]", elements, index))
                                , () => elements.Should(Have.CountAtLeast(index+1)).GetAllActualWebElements().ElementAt(index)
                               );
        }

        public static SElement FindBy(this SCollection elements, Condition<SElement> condition)
        {
            return new SElement(new PseudoBy(string.Format("By.Selene: ({0}).FindBy({1})", elements, condition.Explain()))
                                , () =>
            {
                var webelments = elements.GetAllActualWebElements();
                var found = webelments.ToList()
                                      .Find(element => condition.Apply(
                                          new SElement(new PseudoBy(string.Format("By.Selene: ({0}).FindBy({1})", elements, condition.Explain())) 
                                                     /* 
                                                      * ??? TODO: do we actually need here so meaningful PseudoBy?
                                                      * does it make sense to use it but to put index for each element?
                                                      * via using FindIndex ?
                                                      */
                                                     , () => element)
                                       ));
                if (found == null) 
                {
                    var actualTexts = webelments.ToList().Select(element => element.Text).ToArray();
                    var htmlelements = webelments.ToList().Select(element => element.GetAttribute("outerHTML")).ToArray();
                    throw new NotFoundException("element was not found in collection by condition "
                                                + condition.Explain()
                                                + "\n  Actual visible texts : " + "[" + string.Join(",", actualTexts) + "]"  // TODO: think: this line is actually needed in the case of FindBy(ExactText ...) ... is there any way to add such information not here?
                                                + "\n  Actual html elements : " + "[" + string.Join(",", htmlelements) + "]" 
                                               // TODO: should we add here some other info about elements? e.g. visiblitiy?
                                               );
                    /*
                     * TODO: think on better messages
                     */
                }
                return found;
            });
        }

        public static SCollection FilterBy(this SCollection elements, Condition<SElement> condition)
        {
            return new SCollection(new PseudoBy(string.Format("By.Selene: ({0}).FilterBy({1})", elements, condition.Explain()))
                                   , () => 
            {
                return new ReadOnlyCollection<IWebElement>(  // TODO: don't we need here ReadONlyCollection<SElement> ?
                    elements.GetAllActualWebElements()
                            .Where(element => condition.Apply(
                                new SElement(new PseudoBy(string.Format("By.Selene: ({0}).FindBy({1})", elements, condition.Explain()))
                                            , () => element)))
                            .ToList());
            });
        }

        public static int GetCount(this SCollection elements)
        {
            return  elements.GetAllActualWebElements().Count; // TODO: should we count only visible elements? or all?
        }

    }
}
