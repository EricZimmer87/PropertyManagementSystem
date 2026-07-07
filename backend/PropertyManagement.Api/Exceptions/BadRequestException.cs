using System.Net;

namespace PropertyManagement.Api.Exceptions
{
    public sealed class BadRequestException(string message)
        : AppException(message, HttpStatusCode.BadRequest);
}
