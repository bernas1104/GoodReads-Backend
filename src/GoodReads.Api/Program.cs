using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

using GoodReads.IOC;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers(x => x.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddEndpointsApiExplorer();

const int defaultApiVersion = 1;
builder.Services.AddApiVersioning(
    opt => {
        opt.DefaultApiVersion = new ApiVersion(defaultApiVersion, 0);
        opt.AssumeDefaultVersionWhenUnspecified = true;
        opt.ReportApiVersions = true;
        opt.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader()
        );
    }
);

builder.Services.AddVersionedApiExplorer(
    opt => {
        opt.GroupNameFormat = "'v'VVV";
        opt.SubstituteApiVersionInUrl = true;
    }
);

builder.Services.AddSwaggerGen();

builder.Services.RegisterBindings(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

[ExcludeFromCodeCoverage]
public static partial class Program
{ }
