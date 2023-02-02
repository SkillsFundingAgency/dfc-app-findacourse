﻿// <copyright file="CourseResultsSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.FindACourse.UI.FunctionalTests.Model;
using DFC.App.FindACourse.UI.FunctionalTests.Support;
using DFC.TestAutomation.UI;
using DFC.TestAutomation.UI.Extension;
using DFC.TestAutomation.UI.Helper;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TechTalk.SpecFlow;
using Xunit;

namespace DFC.App.FindACourse.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class CourseResultsSteps
    {
        public CourseResultsSteps(ScenarioContext context)
        {
            this.Context = context;
            this.commonHelper = this.Context.GetHelperLibrary<AppSettings>().CommonActionHelper;
            this.formHelper = this.Context.GetHelperLibrary<AppSettings>().FormHelper;
        }

        private ICommonActionHelper commonHelper;
        private IFormHelper formHelper;

        private ScenarioContext Context { get; set; }

        [Then(@"search results are displayed")]
        public void ThenSearchResultsAreDisplayed()
        {
            // This needs to be updated. The nuget package needs an implicit wait for an object to go stale (the
            // object refreshed). You need to set the search results container in the object context before
            // clicking the search button. Then in this step you need to retrieve the container and carry out
            // a wait for the object to go stale. Once it's gone stale then the search results have refreshed.
            Thread.Sleep(5000);

            var results = this.Context.GetWebDriver().FindElements(By.CssSelector(".govuk-\\!-margin-top-6"));
            Assert.True(results.Count > 0);
            this.Context.Get<IObjectContext>().SetObject("SearchResultsCount", results.Count);
            var searchResults = new List<SearchResult>();

            foreach (var resultContainer in results)
            {
                var searchResult = new SearchResultSupport(this.Context, resultContainer).GetResult();
                searchResults.Add(searchResult);
            }

            // This should read like the following line. This is a bug with the nuget package:
            // this.Context.GetObjectContext().SetObject("SearchResults", searchResults);
            this.Context.Get<IObjectContext>().SetObject("SearchResults", searchResults);
        }

        [Then(@"search results are updated")]
        public void ThenSearchResultsAreUpdated()
        {
            Thread.Sleep(5000);

            var results = this.Context.GetWebDriver().FindElements(By.CssSelector(".govuk-\\!-margin-top-6"));
            Assert.True(results.Count > 0);
            Assert.True(results.Count <= int.Parse(Context.Get<IObjectContext>().GetObject("SearchResultsCount")));
            var searchResults = new List<SearchResult>();

            foreach (var resultContainer in results)
            {
                var searchResult = new SearchResultSupport(this.Context, resultContainer).GetResult();
                searchResults.Add(searchResult);
            }

            this.Context.Get<IObjectContext>().UpdateObject("SearchResults", searchResults);
        }

        [Then(@"next page of results are displayed")]
        public void ThenNextPageOfResultsAreDisplayed()
        {
            var results = this.Context.GetWebDriver().FindElements(By.CssSelector(".govuk-\\!-margin-top-6"));
            Assert.True(results.Count > 0);
        }

        [When(@"I click on the first search result")]
        public void WhenIClickTheFirstResult()
        {
            Thread.Sleep(2000);
            var results = this.Context.GetWebDriver().FindElements(By.ClassName("govuk-heading-m"));
            Assert.True(results.Count > 0);
            var firstResult = results[1];
            this.Context.Get<IObjectContext>().SetObject("FirstResult", firstResult.GetAttribute("innerText"));
            this.Context.GetWebDriver().FindElement(By.LinkText(this.Context.Get<IObjectContext>().GetObject("FirstResult"))).Click();
        }

        [When(@"i click on the previous page link")]
        [When(@"i click on the next page link")]
        public void WhenIClickOnTheNextPageLink()
        {
            Thread.Sleep(2000);
            var page = this.Context.GetWebDriver().FindElements(By.ClassName("pagination-label"));
            page[0].Click();
        }

        [Then(@"I am returned to same search results page")]
        public void ThenIAmReturnedToSameSearchResultsPage()
        {
            Thread.Sleep(5000);

            var results = this.Context.GetWebDriver().FindElements(By.ClassName("govuk-heading-m"));
            Assert.True(results.Count > 0);

            var firstResult = results[1];
            this.Context.Get<IObjectContext>().SetObject("BackToFirstResult", firstResult.GetAttribute("innerText"));

            if (!(this.Context.Get<IObjectContext>().GetObject("FirstResult").ToString() == this.Context.Get<IObjectContext>().GetObject("BackToFirstResult").ToString()))
            {
                throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. Unexpected results displayed on going back from course details page.");
            }
        }

        [Then(@"all results are under (.*) miles")]
        public void ThenAllResultsAreUnderMiles(int selectedMiles)
        {
            while (this.commonHelper.IsElementDisplayed(By.CssSelector("li[class=next]")))
            {
                this.Context.GetWebDriver().FindElement(By.CssSelector("span[class=pagination-label]")).Click();
            }

            var results = this.Context.GetWebDriver().FindElements(By.CssSelector(".govuk-\\!-margin-top-6"));
            var searchResults = new List<SearchResult>();

            foreach (var resultContainer in results)
            {
                var searchResult = new SearchResultSupport(this.Context, resultContainer).GetResult();
                searchResults.Add(searchResult);
            }


            foreach (var result in searchResults)
            {
                var miles = result.Location.Split('(', ')')[1];
                double milesNo;
                double.TryParse(miles, out milesNo);
                if (milesNo > selectedMiles)
                {
                    throw new Exception($"Check course results. Result contains course(s) outside of the distance selected");
                }
                else if (string.IsNullOrEmpty(result.Location))
                {
                    throw new Exception($"Check course results. Location is not displayed however should be shown");
                }
            }
        }
    }
}
