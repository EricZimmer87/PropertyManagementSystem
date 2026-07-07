using System.Net;

namespace PropertyManagement.Api.Exceptions
{
    public sealed class ConflictException(string message)
        : AppException(message, HttpStatusCode.Conflict);

}
