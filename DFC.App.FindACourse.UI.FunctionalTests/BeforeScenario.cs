// <copyright file="BeforeScenario.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.FindACourse.UI.FunctionalTests.Model;
using DFC.TestAutomation.UI;
using DFC.TestAutomation.UI.Extension;
using DFC.TestAutomation.UI.Helper;
using DFC.TestAutomation.UI.Settings;
using DFC.TestAutomation.UI.Support;
using System;
using TechTalk.SpecFlow;

namespace DFC.App.FindACourse.UI.FunctionalTests
{
    [Binding]
    public class BeforeScenario
    {
        public BeforeScenario(ScenarioContext context)
        {
            Context = context;

            if (Context == null)
            {
                throw new NullReferenceException($"The scenario context is null. The {GetType().Name} class cannot be initialised.");
            }
        }

        private ScenarioContext Context { get; set; }

        [BeforeScenario(Order = 0)]
        public void SetObjectContext()
        {
            Context.SetObjectContext(new ObjectContext());
        }

        [BeforeScenario(Order = 1)]
        public void SetSettingsLibrary()
        {
            Context.SetSettingsLibrary(new SettingsLibrary<AppSettings>());
        }

        [BeforeScenario(Order = 2)]
        public void SetApplicationUrl()
        {
            string appBaseUrl = Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl.ToString();
            Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl = new Uri($"{appBaseUrl}find-a-course");
        }

        [BeforeScenario(Order = 3)]
        public void ConfigureBrowserStack()
        {
            Context.GetSettingsLibrary<AppSettings>().BrowserStackSettings.Name = Context.ScenarioInfo.Title;
            Context.GetSettingsLibrary<AppSettings>().BrowserStackSettings.Build = "Find a course";
        }

        [BeforeScenario(Order = 4)]
        public void SetupWebDriver()
        {
            var settingsLibrary = Context.GetSettingsLibrary<AppSettings>();
            var webDriver = new WebDriverSupport<AppSettings>(settingsLibrary).Create();
            webDriver.Manage().Window.Maximize();
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(settingsLibrary.TestExecutionSettings.TimeoutSettings.PageNavigation);
            //webDriver.SwitchTo().Window(webDriver.CurrentWindowHandle);
            Context.SetWebDriver(webDriver);
        }

        [BeforeScenario(Order = 5)]
        public void SetUpHelpers()
        {
            var helperLibrary = new HelperLibrary<AppSettings>(Context.GetWebDriver(), Context.GetSettingsLibrary<AppSettings>());
            Context.SetHelperLibrary(helperLibrary);
        }
    }
}
