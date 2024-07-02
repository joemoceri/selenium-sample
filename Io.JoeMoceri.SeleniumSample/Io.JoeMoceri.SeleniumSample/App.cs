using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.PageObjects;

namespace SeleniumFFmpeg
{
    public class App : IDisposable
    {
        private IWebDriver driver;

        public void Dispose()
        {
            driver?.Dispose();
        }

        public async Task Run()
        {
            var url = "https://www.google.com/";
            var uri = new Uri(url);

            using (driver = GetChromeDriver())
            {
                // setup actions and driver
                var actions = new Actions(driver);
                SetupDriver(driver, url);

                var jsExecutor = (IJavaScriptExecutor)driver;

                // start recording the screen
                using (var screenRecorder = new ScreenRecorder())
                {
                    // start recording the screen
                    screenRecorder.StartRecording(uri.Authority);

                    await Task.Delay(1000);

                    // find the main google search textarea
                    var e = driver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/textarea"));

                    // search
                    e.SendKeys("2024 election");
                    e.SendKeys(Keys.Enter);

                    await Task.Delay(1000);

                    // begin scrolling the page from the top
                    var startPosition = 0;

                    // scroll to the bottom 5 times as the page loads more results
                    for (var i = 0; i < 5; i++)
                    {
                        startPosition = ScrollPage(jsExecutor, startPosition);
                    }

                    await Task.Delay(10000);
                }
            }
        }

        private int ScrollPage(IJavaScriptExecutor jsExecutor, int startPosition)
        {
            // get the documents current height
            var height = int.Parse(jsExecutor.ExecuteScript("return document.body.scrollHeight").ToString());

            // starting from the start position, slowly scroll down
            for (var i = startPosition; i < height; i++)
            {
                jsExecutor.ExecuteScript($"window.scrollTo({startPosition}, {i})");
            }

            // return the current height to be used for the next start position
            return height;
        }

        private void SetupDriver(IWebDriver driver, string url)
        {
            // maximize the window and navigate to the url
            driver.Manage().Window.Maximize();
            PageFactory.InitElements(driver, this);
            driver.Navigate().GoToUrl(url);
        }

        private void MouseoverForm(IWebElement element)
        {
            var javaScript = "var evObj = document.createEvent('MouseEvents');" +
                    "evObj.initMouseEvent(\"mouseover\",true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);" +
                    "arguments[0].dispatchEvent(evObj);";

            ((IJavaScriptExecutor)driver).ExecuteScript(javaScript, element);
        }

        private IWebElement ClearStaleElement(IWebElement element, string idToWait)
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

        private IWebDriver GetChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            var disabledFeatures = new[]
            {
                    //"OptimizationHints", "OptimizationHintsFetching", "Translate", "OptimizationTargetPrediction", "OptimizationGuideModelDownloading",
                    //"InsecureDownloadWarnings", "InterestFeedContentSuggestions", "PrivacySandboxSettings4", 
                    "SidePanelPinning",
                };

            // get rid of you can open bookmarks tab from PR https://github.com/seleniumbase/SeleniumBase/pull/2837
            chromeOptions.AddArguments($"--disable-features={string.Join(",", disabledFeatures)}");

            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");

            //chromeOptions.AddArgument("headless");

            return new ChromeDriver(chromeOptions);
        }
    }
}
