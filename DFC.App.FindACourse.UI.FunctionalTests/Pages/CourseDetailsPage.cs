// <copyright file="FindACourseLandingPage.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.FindACourse.UI.FunctionalTests.Model;
using DFC.TestAutomation.UI;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;

namespace DFC.App.FindACourse.UI.FunctionalTests.Pages
{
    internal class CourseDetailsPage
    {
        public CourseDetailsPage(ScenarioContext context)
        {
            this.Context = context;

            if (this.Context == null)
            {
                throw new NullReferenceException("The scenario context is null. The course details cannot be initialised.");
            }
        }

        private ScenarioContext Context { get; set; }

        public CourseDetailsPage CourseDetailsPageDisplayed()
        {
            var pageHeadingLocator = By.ClassName("govuk-heading-l");
            this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToContainText(pageHeadingLocator, this.Context.Get<IObjectContext>().GetObject("FirstResult"));
            System.Threading.Thread.Sleep(5000);
            return this;
        }


    }
}
