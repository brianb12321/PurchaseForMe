using System;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace PurchaseForMeService.Blocks.Web.Selenium
{
    [RegisterBlock("web_openDriver", Category = "Selenium")]
    public class OpenDriverBlock : IBlock
    {
        public enum DriverType
        {
            Chrome, Remote
        }
        public override object Evaluate(Context context)
        {
            DriverType driverType = Enum.Parse<DriverType>(this.Fields.Get("driverType"));
            string url = this.Values.Evaluate("url", context).ToString();
            Statement body = this.Statements.Get("body");
            Statement setupBody = null;
            try
            {
                setupBody = this.Statements.Get("setupBody");
            }
            catch (ArgumentException)
            {

            }
            IWebDriver driver = null;
            try
            {
                switch (driverType)
                {
                    default:
                    case DriverType.Chrome:
                        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                        ChromeOptions localOptions = new ChromeOptions();
                        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CLUSTER_DOCKER")))
                        {
                            service.EnableVerboseLogging = true;
                            localOptions.AddArgument("--window-size=1920,1080");
                            localOptions.AddArgument("--disable-extensions");
                            localOptions.AddArgument("--proxy-server=\"direct://\"");
                            localOptions.AddArgument("--proxy-bypass-list=*");
                            localOptions.AddArgument("--start-maximized");
                            localOptions.AddArgument("--disable-dev-shm-usage");
                            localOptions.AddArgument("--no-sandbox");
                            localOptions.AddArgument("--ignore-certificate-errors");
                            localOptions.AddArgument("--allow-running-insecure-content");
                            localOptions.AddArgument("--verbose");
                            localOptions.AddArgument("--disable-blink-features=AutomationControlled");
                            string user_agent =
                                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
                            localOptions.AddArgument($"user-agent={user_agent}");
                            localOptions.AddAdditionalCapability("useAutomationExtension", false);;
                            localOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);
                            localOptions.SetLoggingPreference(LogType.Client, LogLevel.All);
                            localOptions.SetLoggingPreference(LogType.Driver, LogLevel.All);
                            localOptions.SetLoggingPreference(LogType.Profiler, LogLevel.All);
                            localOptions.SetLoggingPreference(LogType.Server, LogLevel.All);
                        }
                        driver = new ChromeDriver(service, localOptions);
                        _ = driver.Manage().Timeouts().ImplicitWait;
                        break;
                    case DriverType.Remote:
                        ChromeOptions options = new ChromeOptions();
                        options.AddArgument("verbose");
                        options.SetLoggingPreference(LogType.Browser, LogLevel.All);
                        options.SetLoggingPreference(LogType.Client, LogLevel.All);
                        options.SetLoggingPreference(LogType.Driver, LogLevel.All);
                        options.SetLoggingPreference(LogType.Profiler, LogLevel.All);
                        options.SetLoggingPreference(LogType.Server, LogLevel.All);
                        driver = new RemoteWebDriver(
                            new Uri(Environment.GetEnvironmentVariable("SELENIUM_REMOTE_DRIVER_URL")), options);
                        break;
                }
                context.GetRootContext().Variables.Add("__driver", driver);
                if (setupBody != null)
                {
                    setupBody.Evaluate(context);
                }

                driver.Navigate().GoToUrl(url);
                body.Evaluate(context);
            }
            finally
            {
                driver?.Quit();
                base.Evaluate(context);
            }

            return null;
        }
    }
}