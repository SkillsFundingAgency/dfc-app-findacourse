// <copyright file="RobotController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.App.FindACourse.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mime;

namespace DFC.App.FindACourse.Controllers
{
    public class RobotController : Controller
    {
        private readonly ILogger<RobotController> logger;
        private readonly IWebHostEnvironment hostingEnvironment;

        public RobotController(ILogger<RobotController> logger, IWebHostEnvironment hostingEnvironment)
        {
            this.logger = logger;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public ContentResult Robot()
        {
            try
            {
                this.logger.LogInformation("Generating Robots.txt");

                var robot = this.GenerateThisSiteRobot();

                this.logger.LogInformation("Generated Robots.txt");

                return this.Content(robot.Data, MediaTypeNames.Text.Plain);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"{nameof(this.Robot)}: {ex.Message}");
            }

            // fall through from errors
            return this.Content(null, MediaTypeNames.Text.Plain);
        }

        private Robot GenerateThisSiteRobot()
        {
            var robot = new Robot();
            var robotsFilePath = System.IO.Path.Combine(this.hostingEnvironment.WebRootPath, "StaticRobots.txt");

            if (!System.IO.File.Exists(robotsFilePath))
            {
                return robot;
            }

            // output the composite UI default (static) robots data from the StaticRobots.txt file
            var staticRobotsText = System.IO.File.ReadAllText(robotsFilePath);

            if (!string.IsNullOrWhiteSpace(staticRobotsText))
            {
                robot.Add(staticRobotsText);
            }

            return robot;
        }
    }
}