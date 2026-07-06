using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {       
    //1. Generate a short correlationId(8)
    string correlationId = Guid.NewGuid().ToString("N")[..8];

    // Set the header before await
    context.Response.Headers["X-Correlation-Id"] = correlationId;


    //Start a stopwatch to measure elapsed time
    var stopwatch = Stopwatch.StartNew();
    

    // Log the entry line
    _logger.LogInformation("Entry: Method = {Method}, Path = {Path}, CorrelationId = {CorrelationId}",
    context.Request.Method, 
    context.Request.Path, correlationId);


    if(context.Request.Path == "/api/assessments/results")
        {
            context.Response.StatusCode = 401;
        }
        else
        {
            await _next(context);
        }

        stopwatch.Stop();

        _logger.LogInformation("Exit: StatusCode = {StatusCode}, ElapsedMs = {ElapsedMs}, CorrelationId = {CorrelatinoId}", context.Response.StatusCode, stopwatch.ElapsedMilliseconds, correlationId);
    }

    
}