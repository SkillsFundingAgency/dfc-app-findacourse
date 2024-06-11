// <copyright file="FormSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.FindACourse.UI.FunctionalTests.Model;
using DFC.TestAutomation.UI.Extension;
using DFC.TestAutomation.UI.Helper;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
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
            this.formHelper = this.Context.GetHelperLibrary<AppSettings>().FormHelper;
        }

        private IFormHelper formHelper;

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
        public void WhenIEnterLocationInTheLocationFilter(string location)
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

        [When(@"I filter by (.*) miles")]
        public void WhenIFilterByMiles(string miles)
        {
            this.formHelper.SelectByText(By.Id("distance-select"), $"{miles} miles");
        }

        [When(@"I select (.*) in the start date filter")]
        public void WhenISelectDateFilter(string startDate)
        {
            var startDateFilter = this.Context.GetWebDriver().FindElement(By.Id("startdate-select"));

            if (!startDateFilter.Displayed)
            {
                throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The start date filter could not be located.");
            }

            var selectElement = new SelectElement(startDateFilter);
            selectElement.SelectByValue(startDate);
            startDateFilter.SendKeys(Keys.Tab);
            Thread.Sleep(5000);
        }

        [When(@"I select (.*) in the learning method filter")]
        public void WhenISelectLearningMethodFilter(string LearningMethod)
        {
            switch (LearningMethod)
            {
                case "Online":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.LearningMethod.SelectedIds[0]")).Click();
                    break;
                case "Classroom based":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.LearningMethod.SelectedIds[1]")).Click();
                    break;
                case "Work based":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.LearningMethod.SelectedIds[2]")).Click();
                    break;
                case "Blended learning":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.LearningMethod.SelectedIds[3]")).Click();
                    break;
                default:
                    throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The learning method is not found.");
            }

            Thread.Sleep(5000);
        }

        [When(@"I select (.*) in the course hours filter")]
        public void WhenISelectCourseHoursFilter(string courseHours)
        {
            switch (courseHours)
            {
                case "Full time":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.CourseHours.SelectedIds[0]")).Click();
                    break;
                case "Part time":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.CourseHours.SelectedIds[1]")).Click();
                    break;
                case "Flexible":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.CourseHours.SelectedIds[2]")).Click();
                    break;
                default:
                    throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The Course hours not listed.");
            }

            Thread.Sleep(5000);
        }

        [When(@"I select (.*) in the course time filter")]
        public void WhenISelectCourseTimeFilter(string courseTime)
        {
            switch (courseTime)
            {
                case "Daytime":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.CourseStudyTime.SelectedIds[0]")).Click();
                    break;
                case "Evening":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.CourseStudyTime.SelectedIds[1]")).Click();
                    break;
                case "Weekend":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.CourseStudyTime.SelectedIds[2]")).Click();
                    break;
                case "Day or block release":
                    this.Context.GetWebDriver().FindElement(By.Id("SideBar.CourseStudyTime.SelectedIds[3]")).Click();
                    break;
                default:
                    throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The Course time option is not listed.");
            }

            Thread.Sleep(5000);
        }

        [When(@"I select (.*) in the sort by filter")]
        public void WhenISelectSortFilter(string sortBy)
        {
            var sortByFilter = this.Context.GetWebDriver().FindElement(By.Id("orderBy-Input"));

            if (!sortByFilter.Displayed)
            {
                throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The sort by filter could not be located.");
            }

            var selectElement = new SelectElement(sortByFilter);
            selectElement.SelectByValue(sortBy);
            sortByFilter.SendKeys(Keys.Tab);
            Thread.Sleep(5000);
        }
    }
}
