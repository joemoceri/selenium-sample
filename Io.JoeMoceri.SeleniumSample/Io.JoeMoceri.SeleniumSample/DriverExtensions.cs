using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeleniumFFmpeg
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

                    if (result[0] != null)
                    {
                        if (!result[0].Enabled && !result[0].Displayed)
                        {
                            throw new NoSuchElementException();
                        }

                        return result[0];
                    }
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
