# Changelog

## 0.0.0.8 (in progress)

- updated selenium version to 3.12
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
