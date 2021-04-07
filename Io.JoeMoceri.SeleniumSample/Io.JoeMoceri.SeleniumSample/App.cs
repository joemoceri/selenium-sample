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
