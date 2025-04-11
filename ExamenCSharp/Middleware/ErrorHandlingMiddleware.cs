using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ExamenCSharp.Middleware;
using System.ComponentModel.DataAnnotations;

namespace ExamenCSharp.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(UnauthorizedAccessException ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "https://example.com/probs/validation-error",
                    Title = "Unauthorized",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = ex.Message,
                    Instance = Guid.NewGuid().ToString()
                };

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var response = JsonConvert.SerializeObject(problemDetails);
                await context.Response.WriteAsync(response);
            }
            catch (ValidationException ex)
            {
                // Erreur de validation
                var problemDetails = new ProblemDetails
                {
                    Type = "https://example.com/probs/validation-error",
                    Title = "Validation Error",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = ex.Message,
                    Instance = Guid.NewGuid().ToString()
                };

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var response = JsonConvert.SerializeObject(problemDetails);
                await context.Response.WriteAsync(response);
            }
            catch (Exception ex)
            {
                // Erreur générique
                _logger.LogError(ex, "An unhandled exception occurred.");

                var problemDetails = new ProblemDetails
                {
                    Type = "https://example.com/probs/internal-server-error",
                    Title = "Internal Server Error",
                    Status = (int)HttpStatusCode.InternalServerError,
                    Detail = ex.Message,
                    Instance = Guid.NewGuid().ToString()
                };

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = JsonConvert.SerializeObject(problemDetails);
                await context.Response.WriteAsync(response);
            }
        }
    }
}
