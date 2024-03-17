using System.Text.Json.Serialization;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GoodReads.Api.Controllers.Health
{
    public sealed record HealthCheckResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Status { get; init; }
        public List<HealthStatusCheck> Entities { get; init; }

        public HealthCheckResponse(HealthReport report)
        {
            Status = Enum.GetName(report.Status);
            Entities = new ();

            if (report.Entries is not null)
            {
                foreach (var entry in report.Entries)
                {
                    Entities.Add(
                        new HealthStatusCheck
                        {
                            Name = entry.Key,
                            Status = Enum.GetName(entry.Value.Status),
                            Exception = entry.Value.Exception?.Message
                        }
                    );
                }
            }
        }
    }

    public sealed record HealthStatusCheck
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Status { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Exception { get; init; }
    }
}