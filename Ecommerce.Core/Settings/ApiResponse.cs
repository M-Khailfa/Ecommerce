using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Ecommerce.Core.Settings
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; private set; }
        public bool Succeeded { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public IReadOnlyList<string> Errors { get; private set; } = [];
        public object? Data { get; private set; }

        public ApiResponse() { }

        // ─── Factory Methods ────────────────────────────────────────────────────────

        public static ApiResponse Success(object data, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
            => new() { Succeeded = true, Data = data, Message = message, StatusCode = statusCode };

        public static ApiResponse Ok(string message)
            => new() { Succeeded = true, Message = message, StatusCode = HttpStatusCode.OK };

        public static ApiResponse Created(object data, string message = "")
            => new() { Succeeded = true, Data = data, Message = message, StatusCode = HttpStatusCode.Created };

        public static ApiResponse Deleted(string message = "Resource deleted successfully.")
            => new() { Succeeded = true, Message = message, StatusCode = HttpStatusCode.OK };

        public static ApiResponse Updated(string message = "Resource updated successfully.")
            => new() { Succeeded = true, Message = message, StatusCode = HttpStatusCode.OK };

        public static ApiResponse NotFound(string message = "Resource not found.")
            => new() { Succeeded = false, Message = message, StatusCode = HttpStatusCode.NotFound };

        public static ApiResponse BadRequest(IEnumerable<string> errors)
            => new() { Succeeded = false, Errors = errors.ToList(), StatusCode = HttpStatusCode.BadRequest };

        public static ApiResponse BadRequest(string error)
            => BadRequest([error]);

        public static ApiResponse Unauthorized(string error = "Unauthorized access.")
            => new() { Succeeded = false, Errors = [error], StatusCode = HttpStatusCode.Unauthorized };

        public static ApiResponse Forbidden(string error = "Access forbidden.")
            => new() { Succeeded = false, Errors = [error], StatusCode = HttpStatusCode.Forbidden };

        public static ApiResponse Conflict(string message)
            => new() { Succeeded = false, Message = message, StatusCode = HttpStatusCode.Conflict };

        public static ApiResponse InternalServerError(string message = "An unexpected error occurred.")
            => new() { Succeeded = false, Message = message, StatusCode = HttpStatusCode.InternalServerError };

        // ─── Fluent Helpers ─────────────────────────────────────────────────────────

        public ApiResponse WithMessage(string message)
        {
            Message = message;
            return this;
        }

        public ApiResponse WithData(object data)
        {
            Data = data;
            return this;
        }

        public ApiResponse WithErrors(IEnumerable<string> errors)
        {
            Errors = errors.ToList();
            return this;
        }
    }
}
