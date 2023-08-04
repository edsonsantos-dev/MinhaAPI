using Elmah.Io.AspNetCore;
using System.Net;

namespace DevIO.API.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            ex.Ship(context);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await Task.CompletedTask;
        }
    }
}
