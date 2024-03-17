using System.Net;

using GoodReads.Api.Controllers.Health;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GoodReads.Unit.Tests.Api.Controllers.Health
{
    public class HealthControllerTest
    {
        private readonly HealthCheckService _service;
        private readonly HealthController _controller;

        public HealthControllerTest()
        {
            _service = Substitute.For<HealthCheckService>();
            _controller = new (_service);
        }

        [Fact]
        public async Task GivenGetHealthAsync_WhenHealthy_ShouldReturnOk()
        {
            // arrange
            _service.CheckHealthAsync(Arg.Any<CancellationToken>())
                .Returns(
                    new HealthReport(
                        new Dictionary<string, HealthReportEntry>
                        {
                            {
                                "Foo",
                                new HealthReportEntry(
                                    HealthStatus.Healthy,
                                    null,
                                    TimeSpan.Zero,
                                    null,
                                    null
                                )
                            }
                        },
                        TimeSpan.Zero
                    )
                );

            // act
            var response = await _controller.GetHealthAsync(CancellationToken.None)
                as OkObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response!.Value.Should().BeOfType<HealthCheckResponse>();
        }

        [Fact]
        public async Task GivenGetHealthAsync_WhenUnhealthy_ShouldReturnServiceUnavailable()
        {
            // arrange
            _service.CheckHealthAsync(Arg.Any<CancellationToken>())
                .Returns(
                    new HealthReport(
                        new Dictionary<string, HealthReportEntry>
                        {
                            {
                                "Foo",
                                new HealthReportEntry(
                                    HealthStatus.Unhealthy,
                                    null,
                                    TimeSpan.Zero,
                                    new Exception("Some Error"),
                                    null
                                )
                            }
                        },
                        TimeSpan.Zero
                    )
                );

            // act
            var response = await _controller.GetHealthAsync(CancellationToken.None)
                as ObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.ServiceUnavailable);
            response!.Value.Should().BeOfType<HealthCheckResponse>();
        }
    }
}