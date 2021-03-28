# Changelog

## NEXT
- refactor waiting logic in element actions
  - to wait for command to pass not for specific condition
- should we unmark ShouldNot as deprecated?
- mark DescribedCondition as obsolete
- make `Condition#Not` property public

## 1.0.0-alpha04 (to be released on 2020.06.18)
- added `Be.Not.*` and `Have.No.*` as entry points to "negated conditions"
- `.ShouldNot` is obsolete now, use `.Should(Be.Not.*)` or `.Should(Have.No.*)` instead
- removed `Actual: ...` clause from the error messages (usually it is obvious according the condition name)
- moved previous impl inside DescribedCondition to DescribedResult that is now the parent of the Condition class
  - don't use DescribedResult in your code, it's kind of "internal", but so far can't be made non-public...
  - DescribedCondition class probably will be maked as obsolete in future
- added `Not<TEntity> : Condition<TEntity>` class, yet kept as internal
  - let's finalize the naming in [#53](https://github.com/yashaka/NSelene/issues/53)
- added `Condition#Not` property, yet keeping it as internal
  - let's finalize the naming in [#53](https://github.com/yashaka/NSelene/issues/53)
- added SeleneElement extensions
  - `.JsScrollIntoView()`
  - `.JsClick(centerXOffset=0, centerYOffset=0)`
    - proper tests coverage is yet needed
- made Configuration ThreadLocal
- added SeleneElement methods:
  - `WaitUntil(Condition)` – like Should, but returns false on failure
  - `Matching(Condition)` - the predicate, like WaitUntil but without waiting
  - `With([timeout], [pollDuringWaits], [setValueByJs])` - to override corresponding selene setting from Configuration
  - `_With_(_SeleneSettings_)` option to fully disconnect element config from shared Configuration
    - underscores mean that method is signature might change...
- tuned selene elements representation in error messages
  - now code like `SS(".parent").FilterBy(Be.Visible)[0].SS(".child").FindBy(Have.CssClass("special")).S("./following-sibling::*")`
  - renders to: `Browser.All(.parent).By(Visible)[0].All(.child).FirstBy(has CSS class 'special').Element(./following-sibling::*)`

## 1.0.0-alpha03 (to be released on 2020.06.03)
- added `SeleneElement#Type(string keys)`, i.e. `S(selector).Type(text)`
  - with wait for visibility built in
- changed in `SeleneElement#SendKeys(string keys)`, i.e. `S(selector).SendKeys(keys)`
  - the wait from Be.Visible to Be.InDom 
  - to enable its usage for file upload
  - but can break some tests, where the "wait for visibility" is needed in context of "typing text"
    - this should be fixed in further versions

## 1.0.0-alpha02 (released on 2020.05.26)
- added `Configuration.SetValueByJs`, `false` by default

## 1.0.0-alpha01 (released on 2020.05.21)

- reformatted project to the SDK-style
- **switched target framework from net45 to netstandard2.0**
  - adding support of net45 is considered to be added soon

- removed all obsolete things deprecated till 0.0.0.7 inclusive

- removed dependency to Selenium.Support 
  - it's not used anymore anywhere in NSelene
- updated Selenium.Webdriver dependency to 3.141.0

- added Have.No.CssClass and Have.No.Attribute

- `S(selector)` and other similar methods now also accepts string with xpath

- removed from API (marked internal) yet unreleased:
  - NSelene.With.*

- kept deprecated:
  - NSelene.Selectors.ByCss
  - NSelene.Selectors.ByLinkText

- other
  - restructured tests a bit
  - removed NSeleneExamples from the solution
    - left a few examples in NSeleneTests 

## 0.0.0.8 (skipped for now)

- updated selenium version to 3.141
- deprecated:
  - Selectors.ByCss
  - Selectors.ByLinkText
- added:
  - With.Type
  - With.Value
  - With.IdContains
  - With.Text
  - With.ExactText
  - With.Id
  - With.Name
  - With.ClassName
  - With.XPath
  - With.Css
  - With.Attribute
  - With.AttributeContains


## 0.0.0.7 (released May 28, 2018)

### Summary
- Upgraded selenium support to >= 3.5.2. 

### New 
- added
  - Selene.WaitTo(Condition<IWebDriver>) method and SeleneDriver#Should correspondingly
  - Conditions.JSReturnedTrue (Have.JSReturnedTrue) 
  - SeleneElement#GetProperty (from IWebElement of 3.5.2 version)

## 0.0.0.6 (released Aug 1, 2016)

### API changes 
Should not break anything in this version (because "old names" was just marked as deprecated and will be removed completely in next version):
- renamed 
  - Config to Configuration
  - Browser to SeleneDriver
  - SElement to SeleneElement
  - SCollection to SeleneCollection
  - Utils to Selene
    to be more "selenide like" (which has com.codeborn.selenide.Selenide class as a container for utility methods)
    and make name more conceptual
  - Selene.SActions() to Selene.Actions and make it property
  - Be.InDOM to Be.InDom (according to standard naming convention)
- closed access to (made private/or internal)
  - SElement#Actions
  - SeleneElement and SeleneCollection constructors (internal) 
    In order to leave ability to rename classes, 
    if one day we extract SeleneElement/SeleneCollection as interfaces. 
    It's ok because we do not create their objects via constructors, but via Selene.S/Selene.SS, etc.
- removed 
  - SElement#SLocator property
  
Breaking changes:
- Left only the following aliasses: 
  - SeleneElement: Find, FindAll, Should, ShouldNot; 
  - SeleneCollection: Should, ShouldNot
  - SeleneDriver: Find, FindAll
  Everything else moved to NSelene.Support.Extensions, so to fix code: you have to add additional "using" statement
  It is recommended though to use these extensions only as "examples", because there are too much of them. The latter may lead to confusion in usage. Usually the user will need only some of them. So better to "copy&paste" needed ones to user's project namespace.
- changed Be.Blank() to Be.Blank (refactored to property);

### New
- enhanced interoperability with raw selenium. Now implicit waits for visibility can be added to all PageFactory webelements just via decorating new SDriver(driver); And all explicit driver calls for finding both IWebElement and IList<IWebElement> will produce NSelene proxy alternatives with both implicit waits for visibility and for indexed webelements of collections.

### Refactoring
- refactored all "static variable" conditions to be "static properties", which should ensure stability for parallel testing

### License
- Changed License to "MIT License"

## 0.0.0.5 (released May 29, 2016)
- added object oriented wrapper over WebDriver - implemented in Browser class
  - which makes it much easier to integrate NSelene to existing selenium based frameworks
  - and supports "creating several drivers per test"

## 0.0.0.4 (released May 12, 2016)  
- enhanced error messages via adding locators of elements and some other useful info when describing errors of failed search by condition in a list

## 0.0.0.3 (released May 11, 2016)  
- Added parallel drivers support and upgraded Selenium to 2.53.0

## 0.0.0.2 (released March 20, 2016)
- Stabilized version with main functionality, not optimised for speed, but yet fast enough and you will hardly notice the difference:)

## 0.0.0.1 (released December 30, 2015)
- Initial "pretty draft" version with basic features ported
- Published under Apache License 2.0
