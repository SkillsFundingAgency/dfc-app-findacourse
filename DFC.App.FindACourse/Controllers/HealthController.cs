using DFC.App.FindACourse.Extensions;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogger<HealthController> logger;
        private readonly IFindACourseService findACourseService;
        private readonly string resourceName;

        public HealthController(ILogger<HealthController> logger, IFindACourseService findACourseService)
        {
            this.logger = logger;
            this.findACourseService = findACourseService;
            resourceName = typeof(Program).Namespace;
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            logger.LogInformation($"{nameof(this.Health)} has been called");

            try
            {
                var isHealthy = findACourseService.PingAsync();
                if (isHealthy)
                {
                    const string message = "Document store is available";
                    logger.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                    var viewModel = CreateHealthViewModel(message);

                    return this.NegotiateContentResult(viewModel, viewModel.HealthItems);
                }

                logger.LogError($"{nameof(this.Health)}: Ping to {resourceName} has failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(this.Health)}: {resourceName} exception: {ex.Message}");
            }

            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet]
        [Route("health/ping")]
        public IActionResult Ping()
        {
            logger.LogInformation($"{nameof(this.Ping)} has been called");

            return Ok();
        }

        private HealthViewModel CreateHealthViewModel(string message)
        {
            logger.LogInformation($"{nameof(this.CreateHealthViewModel)} has been called");

            return new HealthViewModel
            {
                HealthItems = new List<HealthItemViewModel>
                {
                    new HealthItemViewModel
                    {
                        Service = resourceName,
                        Message = message,
                    },
                },
            };
        }
    }
}