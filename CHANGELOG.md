# Changelog

## 0.0.0.6 (development...)
API changes
- renamed Config to Configuration
- made SElement#Actions private
- renamed Browser to SDriver
- renamed Utils.SActions to Utils.Actions and make it property
- made SElement and SCollection constructors internal (in order to leave ability to rename them, if one day we extract SElement/SCollection as interfaces);
- removed SElement#SLocator property
- renamed NSelene.Utils to NSelene.Selene to be more "selenide like" with com.codeborn.selenide.Selenide class as a container for utility methods
- renamed Be.InDOM to Be.InDom
- Left only the following aliasses: 
  - SElement: S, SS, Should, ShouldNot; 
  - SCollection: Should, ShouldNot
  - SDriver: Find, FindAll
  Everything else moved to NSelene.Support.Extensions
  It is recommended though to use this extensions only as "example", because there are too much of them. The latter may lead to confusion in usage. Usually the user will need only some of them. so better to "copy&paste" needed ones to user's project namespace.

New
- enhanced interoperability with raw selenium. Now implicit waits for visibility can be added to all PageFactory webelements just via decorating new SDriver(driver); And all explicit driver calls for finding both IWebElement and IList<IWebElement> will produce NSelene proxy alternatives with both implicit waits for visibility and for indexed webelements of collections.

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
