// <copyright file="CourseResultsSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.FindACourse.UI.FunctionalTests.Model;
using DFC.App.FindACourse.UI.FunctionalTests.Support;
using DFC.TestAutomation.UI;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
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
        }

        private ScenarioContext Context { get; set; }

        [Then(@"search results are displayed")]
        public void ThenSearchResultsAreDisplayed()
        {
            // This needs to be updated. The nuget package needs an implicit wait for an object to go stale (the
            // object refreshed). You need to set the search results container in the object context before
            // clicking the search button. Then in this step you need to retrieve the container and carry out
            // a wait for the object to go stale. Once it's gone stale then the search results have refreshed.
            Thread.Sleep(5000);

            var results = this.Context.GetWebDriver().FindElements(By.ClassName("govuk-!-margin-top-6"));
            Assert.True(results.Count > 0);
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
    }
}
