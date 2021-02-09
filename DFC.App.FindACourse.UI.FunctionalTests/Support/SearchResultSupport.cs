// <copyright file="SearchResultSupport.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.FindACourse.UI.FunctionalTests.Model;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Globalization;
using TechTalk.SpecFlow;

namespace DFC.App.FindACourse.UI.FunctionalTests.Support
{
    public class SearchResultSupport
    {
        public SearchResultSupport(ScenarioContext context, IWebElement searchResultContainer)
        {
            this.Context = context;
            this.SearchResultContainer = searchResultContainer;

            if (this.SearchResultContainer == null)
            {
                throw new ArgumentException("The search result container cannot be null. This needs to be the containing IWebElement for a single course search result.");
            }
        }

        private ScenarioContext Context { get; set; }

        private IWebElement SearchResultContainer { get; set; }

        internal SearchResult GetResult()
        {
            var names = this.SearchResultContainer.FindElements(By.ClassName("govuk-heading-m"));

            if (!names.Count.Equals(1))
            {
                throw new OperationCanceledException("The search result container does not contain a course name. The container is not recognised.");
            }

            var searchResult = new SearchResult();
            searchResult.Name = names[0].Text;

            var paragraphs = this.SearchResultContainer.FindElements(By.TagName("p"));

            if (paragraphs.Count.Equals(0))
            {
                throw new OperationCanceledException("The search result container does not contain a course summary. The container is not recognised.");
            }

            searchResult.Summary = this.SearchResultContainer.FindElements(By.TagName("p"))[0].Text;

            var details = this.SearchResultContainer.FindElements(By.ClassName("govuk-secondary-colour"));

            var startDateFound = false;
            foreach (var detail in details)
            {
                if (detail.Text.ToLower(CultureInfo.CurrentCulture).Contains("start date"))
                {
                    var parentNode = this.Context.GetHelperLibrary<AppSettings>().JavaScriptHelper.GetParentElement(detail);
                    var startDateValue = parentNode.Text.Replace("Start date:", string.Empty).Trim();
                    searchResult.StartDate = startDateValue;
                    startDateFound = true;
                    break;
                }
            }

            if (!startDateFound)
            {
                throw new OperationCanceledException("No start date data was found within the search result container. As all results should have a start date, the container is not recognised.");
            }

            var costFound = false;
            foreach (var detail in details)
            {
                if (detail.Text.ToLower(CultureInfo.CurrentCulture).Contains("cost"))
                {
                    var parentNode = this.Context.GetHelperLibrary<AppSettings>().JavaScriptHelper.GetParentElement(detail);
                    var costValue = parentNode.Text.Replace("Cost:", string.Empty).Trim();
                    searchResult.Cost = costValue;
                    costFound = true;
                    break;
                }
            }

            if (!costFound)
            {
                throw new OperationCanceledException("No cost data was found within the search result container. As all results should have a cost value, the container is not recognised.");
            }

            var providerFound = false;
            foreach (var detail in details)
            {
                if (detail.Text.ToLower(CultureInfo.CurrentCulture).Contains("provider"))
                {
                    var parentNode = this.Context.GetHelperLibrary<AppSettings>().JavaScriptHelper.GetParentElement(detail);
                    var providerValue = parentNode.Text.Replace("Provider:", string.Empty).Trim();
                    searchResult.Provider = providerValue;
                    providerFound = true;
                    break;
                }
            }

            if (!providerFound)
            {
                throw new OperationCanceledException("No provider data was found within the search result container. As all results should have a provider value, the container is not recognised.");
            }

            var courseTypeFound = false;
            foreach (var detail in details)
            {
                var tester = detail.Text;
                if (detail.Text.ToLower(CultureInfo.CurrentCulture).Contains("course type"))
                {
                    var parentNode = this.Context.GetHelperLibrary<AppSettings>().JavaScriptHelper.GetParentElement(detail);
                    var courseTypeValue = parentNode.Text.Replace("Course type:", string.Empty).Trim();
                    searchResult.CourseType = courseTypeValue;
                    courseTypeFound = true;
                    break;
                }
            }

            if (!courseTypeFound)
            {
                throw new OperationCanceledException("No course type data was found within the search result container. As all results should have a course type value, the container is not recognised.");
            }

            foreach (var detail in details)
            {
                if (detail.Text.ToLower(CultureInfo.CurrentCulture).Contains("location"))
                {
                    var parentNode = this.Context.GetHelperLibrary<AppSettings>().JavaScriptHelper.GetParentElement(detail);
                    var locationValue = parentNode.Text.Replace("Location:", string.Empty).Trim();
                    searchResult.Location = locationValue;
                    break;
                }
            }

            foreach (var detail in details)
            {
                if (detail.Text.ToLower(CultureInfo.CurrentCulture).Contains("time"))
                {
                    var parentNode = this.Context.GetHelperLibrary<AppSettings>().JavaScriptHelper.GetParentElement(detail);
                    var timeValue = parentNode.Text.Replace("Time:", string.Empty).Trim();
                    searchResult.Time = timeValue;
                    break;
                }
            }

            foreach (var detail in details)
            {
                if (detail.Text.ToLower(CultureInfo.CurrentCulture).Contains("hours"))
                {
                    var parentNode = this.Context.GetHelperLibrary<AppSettings>().JavaScriptHelper.GetParentElement(detail);
                    var hoursValue = parentNode.Text.Replace("Hours:", string.Empty).Trim();
                    searchResult.Hours = hoursValue;
                    break;
                }
            }

            return searchResult;
        }
    }
}
