using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GoodReads.Api.Controllers.Health
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public sealed class HealthController : ControllerBase
    {
        private readonly HealthCheckService _service;

        public HealthController(HealthCheckService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealthAsync(CancellationToken cancellationToken)
        {
            var report = await _service.CheckHealthAsync(cancellationToken);

            var response = new HealthCheckResponse(report);

            return report.Status == HealthStatus.Healthy ?
                Ok(response) :
                StatusCode(
                    (int)HttpStatusCode.ServiceUnavailable,
                    response
                );
        }
    }
}