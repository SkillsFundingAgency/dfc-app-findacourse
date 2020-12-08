// <copyright file="FindACourseLandingPage.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.FindACourse.UI.FunctionalTests.Model;
using DFC.TestAutomation.UI.Extension;
using System;
using TechTalk.SpecFlow;

namespace DFC.App.FindACourse.UI.FunctionalTests.Pages
{
    internal class FindACourseLandingPage
    {
        public FindACourseLandingPage(ScenarioContext context)
        {
            this.Context = context;

            if (this.Context == null)
            {
                throw new NullReferenceException("The scenario context is null. The contact us home page cannot be initialised.");
            }
        }

        private ScenarioContext Context { get; set; }

        public FindACourseLandingPage NavigateToFindACoursePage()
        {
            this.Context.GetWebDriver().Url = this.Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl.ToString();
            return this;
        }
    }
}
