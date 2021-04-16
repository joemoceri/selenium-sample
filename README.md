# selenium-sample
> This is a repository containing sample C# code working with Selenium.WebDriver

* [Overview](#overview)

## Overview

App.cs

```csharp
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

namespace Io.JoeMoceri.SeleniumSample
{
    public class App : IDisposable
    {
        private IWebDriver driver;

        public void Dispose()
        {
            driver?.Dispose();
        }

        public void Run()
        {
            using (driver = GetChromeDriver())
            {
                // pre
                var action = new Actions(driver);

                driver.Manage().Window.Maximize();
                PageFactory.InitElements(driver, this);

                // get a form element
                var formElement = driver.TryFindElement(By.ClassName("your-class-name"), 5);

                // mouse over it
                MouseoverForm(formElement);

                var element = driver.TryFindElement(By.Id("field-name"), 5);

                element = ClearStaleElement(element, "field-name");
                action.MoveToElement(element).Perform();
                element = ClearStaleElement(element, "field-name");
                element.Clear();

                element = ClearStaleElement(element, "field-name");
                element.SendKeys("example message to appear in the form field");
            }

            void MouseoverForm(IWebElement element)
            {
                var javaScript = "var evObj = document.createEvent('MouseEvents');" +
                        "evObj.initMouseEvent(\"mouseover\",true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);" +
                        "arguments[0].dispatchEvent(evObj);";

                ((IJavaScriptExecutor)driver).ExecuteScript(javaScript, element);
            }

            IWebElement ClearStaleElement(IWebElement element, string idToWait)
            {
                bool staleElement = true;
                var i = 0;
                while (staleElement || i == 100)
                {
                    try
                    {
                        staleElement = false;
                    }
                    catch (StaleElementReferenceException)
                    {
                        staleElement = true;
                        element = driver.TryFindElement(By.Id(idToWait), 5);
                    }

                    i++;
                }

                return element;
            }

            IWebDriver GetChromeDriver()
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");

                chromeOptions.AddArgument("headless");

                return new ChromeDriver(chromeOptions);
            }
        }
    }
}

```

DriverExtensions.cs

```csharp
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Io.JoeMoceri.SeleniumSample
{
    public static class DriverExtensions
    {
        public static IWebElement TryFindElementByAttributeValue(this IWebDriver driver, string attributeName, string attributeValue, int timeoutInSeconds)
        {
            return driver.TryFindElement(By.XPath($"//*[@{attributeName} = '{attributeValue}']"), timeoutInSeconds);
        }

        public static IWebElement TryFindElementByTagAndValue(this IWebDriver driver, string tagName, string value, int timeoutInSeconds)
        {
            return driver.TryFindElement(By.XPath($"//{tagName}[contains(text(), '{value}')]"), timeoutInSeconds);
        }

        public static bool WaitForUrl(this IWebDriver driver, string url, int timeOutInSeconds)
        {
            var result = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutInSeconds))
                .Until(d => d.Url.ToLower().Contains(url));

            return result;
        }

        public static IWebElement TryFindElement(this IWebDriver driver, By by, int timeOutInSeconds)
        {
            DateTime startTime = DateTime.Now;
            NoSuchElementException lastException = new NoSuchElementException();

            while (DateTime.Now < startTime.AddSeconds(timeOutInSeconds))
            {
                try
                {
                    var result = driver.FindElements(by);

                    return result[0];
                }
                catch (NoSuchElementException ex)
                {
                    // Ignore until timeout
                    lastException = ex;
                    Thread.Sleep(250);
                }
            }

            throw lastException;
        }
    }
}

```
