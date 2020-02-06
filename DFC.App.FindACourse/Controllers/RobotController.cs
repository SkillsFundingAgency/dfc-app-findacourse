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
        private readonly IHostingEnvironment hostingEnvironment;

        public RobotController(ILogger<RobotController> logger, IHostingEnvironment hostingEnvironment)
        {
            this.logger = logger;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public ContentResult Robot()
        {
            try
            {
                logger.LogInformation("Generating Robots.txt");

                var robot = GenerateThisSiteRobot();

                logger.LogInformation("Generated Robots.txt");

                return Content(robot.Data, MediaTypeNames.Text.Plain);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(Robot)}: {ex.Message}");
            }

            // fall through from errors
            return Content(null, MediaTypeNames.Text.Plain);
        }

        private Robot GenerateThisSiteRobot()
        {
            var robot = new Robot();
            var robotsFilePath = System.IO.Path.Combine(hostingEnvironment.WebRootPath, "StaticRobots.txt");

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