## Selenium Test Suites | Kid Problems
This test suite showcases Selenium WebDriver frameworks built with C# and xUnit to validate the Kid Problems web application. This test suite demonstrates SDET best practices including the Page Object Model (POM), clean code, and secure configuration management.

### Tech Stack
- Language: C# (.NET 8.0)
- Automation: Selenium WebDriver
- Test Runner: xUnit
- Pattern: Page Object Model (POM)

### Key Features
- Decoupled Logic: Separation of test scripts from page-specific elements for high maintainability.
- Fluent Wait Strategy: Implementation of WebDriverWait to eliminate flakiness caused by element load times.
- Easy Debugging: Implementation of screen capture on test failure.
- Easy Test Setup and Teardown: Implementation of calling API endpoints to set up and tear down test data.
- CI/CD Ready: Test credential can be provided through Environment Variables (*KPUITEST_USERNAME*, *KPUITEST_PASSWORD*, etc.), which is important for CI/CD pipeline.
