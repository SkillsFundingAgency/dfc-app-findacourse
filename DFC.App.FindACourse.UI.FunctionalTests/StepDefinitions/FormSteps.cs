// <copyright file="FormSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.FindACourse.UI.FunctionalTests.Model;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Threading;
using TechTalk.SpecFlow;

namespace DFC.App.FindACourse.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class FormSteps
    {
        public FormSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [When(@"I enter the search term (.*) in the search box")]
        public void WhenIEnterTheSearchTermInTheSearchBox(string text)
        {
            var searchBoxes = this.Context.GetWebDriver().FindElements(By.Id("search-input"));

            if (!searchBoxes.Count.Equals(1))
            {
                throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The appropriate search box could not be located.");
            }

            var searchBox = searchBoxes[0];
            searchBox.SendKeys(text);
        }

        [When(@"I click the search button")]
        public void WhenIClickTheSearchButton()
        {
            var searchBoxes = this.Context.GetWebDriver().FindElements(By.Id("search-button"));

            if (!searchBoxes.Count.Equals(1))
            {
                throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The appropriate search button could not be located.");
            }

            searchBoxes[0].Click();
        }

        [When(@"I enter (.*) in the location filter")]
        public void WhenIEnterBirminghamInTheLocationFilter(string location)
        {
            var locationFilter = this.Context.GetWebDriver().FindElements(By.Id("location-input"));

            if (!locationFilter.Count.Equals(1))
            {
                throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The location filter could not be located.");
            }

            var locationFilters = locationFilter[0];
            locationFilters.SendKeys(location);
            locationFilters.SendKeys(Keys.Tab);
            Thread.Sleep(5000);
        }
    }
}
