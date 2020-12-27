// <copyright file="NavigationSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.FindACourse.UI.FunctionalTests.Model;
using DFC.App.FindACourse.UI.FunctionalTests.Pages;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Globalization;
using TechTalk.SpecFlow;

namespace DFC.App.FindACourse.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class NavigationSteps
    {
        public NavigationSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [Given(@"I am on the (.*) page")]
        [Then(@"the (.*) page is displayed")]
        public void GivenIAmOnThePage(string pageName)
        {
            switch (pageName.ToLower(CultureInfo.CurrentCulture))
            {
                case "find a course landing":
                    var findACourseHomePage = new FindACourseLandingPage(this.Context);
                    findACourseHomePage.NavigateToFindACoursePage();
                    var pageHeadingLocator = By.CssSelector("h1.govuk-heading-xl");
                    this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToContainText(pageHeadingLocator, "Find a course");
                    break;
                case "course details":
                    var courseDetailsPage = new CourseDetailsPage(this.Context);
                    courseDetailsPage.CourseDetailsPageDisplayed();
                    break;

                default:
                    throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The page name provided was not recognised.");
            }
        }
    }
}
