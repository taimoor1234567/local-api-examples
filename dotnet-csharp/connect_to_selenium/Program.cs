﻿using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Kameleo.LocalApiClient;
using OpenQA.Selenium.Support.UI;

namespace ConnectToSelenium
{
    class Program
    {
        // This is the port Kameleo.CLI is listening on. Default value is 5050, but can be overridden in appsettings.json file
        private const int KameleoPort = 5050;

        static async Task Main()
        {
            var client = new KameleoLocalApiClient(new Uri($"http://localhost:{KameleoPort}"));
            client.SetRetryPolicy(null);

            // Search Chrome Base Profiles
            var baseProfiles = await client.SearchBaseProfilesAsync(deviceType: "desktop", browserProduct: "chrome");

            // Create a new profile with recommended settings
            // for browser fingerprint protection
            var requestBody = BuilderForCreateProfile
                .ForBaseProfile(baseProfiles[0].Id)
                .SetRecommendedDefaults()
                .Build();

            var profile = await client.CreateProfileAsync(requestBody);

            // Start the browser
            await client.StartProfileAsync(profile.Id);

            // Connect to the running browser instance using WebDriver
            var uri = new Uri($"http://localhost:{KameleoPort}/webdriver");
            var opts = new ChromeOptions();
            opts.AddAdditionalOption("kameleo:profileId", profile.Id.ToString());
            var webdriver = new RemoteWebDriver(uri, opts);


            // Use any WebDriver command to drive the browser
            // and enjoy full protection from bot detection products
            webdriver.Navigate().GoToUrl("https://google.com");
            webdriver.FindElement(By.CssSelector("div[aria-modal=\"true\"][tabindex=\"0\"] button + button")).Click();
            webdriver.FindElement(By.Name("q")).SendKeys("Kameleo");
            webdriver.FindElement(By.Name("q")).SendKeys(Keys.Enter);
            var wait = new WebDriverWait(webdriver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("result-stats")));
            var title = webdriver.Title;
            Console.WriteLine("The title is " + title);

            // Stop the browser by stopping the Kameleo profile
            await client.StopProfileAsync(profile.Id);
        }
    }
}