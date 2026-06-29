using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PropertyManagement.Api.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        // Adapted from ASP.NET Core Identity's IdentityApiEndpointRouteBuilderExtensions.
        // Converts IdentityResult errors into a ValidationProblemDetails response.
        // https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs
        protected ActionResult IdentityValidationProblem(IdentityResult result)
        {
            // We expect a single error code and description in the normal case.
            // This could be golfed with GroupBy and ToDictionary, but perf! :P
            Debug.Assert(!result.Succeeded);
            var errorDictionary = new Dictionary<string, string[]>(1);

            foreach (var error in result.Errors)
            {
                string[] newDescriptions;

                if (errorDictionary.TryGetValue(error.Code, out var descriptions))
                {
                    newDescriptions = new string[descriptions.Length + 1];
                    Array.Copy(descriptions, newDescriptions, descriptions.Length);
                    newDescriptions[descriptions.Length] = error.Description;
                }
                else
                {
                    newDescriptions = [error.Description];
                }

                errorDictionary[error.Code] = newDescriptions;
            }

            return ValidationProblem(new ValidationProblemDetails(errorDictionary));
        }
    }
}
