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
        private const string SuccessMessage = "Document store is available";

        private readonly ILogger<HealthController> logger;
        private readonly IFindACourseService findACourseService;
        private readonly string resourceName;

        public HealthController(ILogger<HealthController> logger, IFindACourseService findACourseService)
        {
            this.logger = logger;
            this.findACourseService = findACourseService;
            this.resourceName = typeof(Program).Namespace;
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            this.logger.LogInformation($"{nameof(this.Health)} has been called");

            try
            {
                var isHealthy = this.findACourseService.PingAsync();
                if (isHealthy)
                {
                    this.logger.LogInformation($"{nameof(this.Health)} responded with: {this.resourceName} - {SuccessMessage}");

                    var viewModel = this.CreateHealthViewModel();

                    return this.NegotiateContentResult(viewModel);
                }

                this.logger.LogError($"{nameof(this.Health)}: Ping to {this.resourceName} has failed");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"{nameof(this.Health)}: {this.resourceName} exception: {ex.Message}");
            }

            return this.StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet]
        [Route("health/ping")]
        public IActionResult Ping()
        {
            this.logger.LogInformation($"{nameof(this.Ping)} has been called");

            return this.Ok();
        }

        private HealthViewModel CreateHealthViewModel()
        {
            return new HealthViewModel
            {
                HealthItems = new List<HealthItemViewModel>
                {
                    new HealthItemViewModel
                    {
                        Service = this.resourceName,
                        Message = SuccessMessage,
                    },
                },
            };
        }
    }
}