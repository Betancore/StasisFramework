# Selenium based framework template (codename 'Stasis')
## Description
This is a very simple Selenium based framework with few optional features. Here is the list of main advantages and disadvantages:
Pros:
* Pure Selenium, no need to decompile any third-party wrapper libs to upderstand what is under the hood
* [Support of Google Api (Drive, Gmail, Spreadsheets)](https://github.com/Betancore/StasisFramework/blob/master/Framework/Helpers/GoogleApiHelper.cs)
* [Support of Browserstack](https://github.com/Betancore/StasisFramework/blob/master/Framework/Helpers/BrowserstackHelper.cs)
* [Support of screenshoting](https://github.com/Betancore/StasisFramework/blob/master/Framework/Helpers/ScreenshotHelper.cs)
Cons:
* Due to singleton and static driver instance - no parallelization out of the box
* No reporting

## Installing
1. Add new project in solution
2. Reference "Framework" project

## Tips and tricks
* See the example of implementation in [DummyTests](https://github.com/Betancore/StasisFramework/tree/master/DummyTests) project
* Basic [settings](https://github.com/Betancore/StasisFramework/blob/master/resources/settings.xml) link is placed in Framework project. Most likely you would like to add separate configs for each test project - so remove it from Framework and add to each test project. Or use it as is in case of single test project.
