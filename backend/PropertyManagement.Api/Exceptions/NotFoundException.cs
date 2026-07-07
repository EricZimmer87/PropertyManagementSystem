using System.Net;

namespace PropertyManagement.Api.Exceptions
{
    public sealed class NotFoundException(string resourceName, object key)
        : AppException($"{resourceName} with identifier '{key}' was not found.", HttpStatusCode.NotFound);
}
