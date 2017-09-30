using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using HorseSpot.Infrastructure.Exceptions;

namespace HorseSpot.Api.Utils
{
    public class HorseSpotExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private const string TehnicalErrorMessage = "An unexpected error occured";

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            var request = actionExecutedContext.Request;

            if (exception is UnauthorizedException)
            {
                actionExecutedContext.Response = request.CreateErrorResponse(HttpStatusCode.Unauthorized, exception.Message);

                return;
            }

            if (exception is ForbiddenException)
            {
                actionExecutedContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, exception.Message);

                return;
            }

            if (exception is ResourceNotFoundException)
            {
                actionExecutedContext.Response = request.CreateErrorResponse(HttpStatusCode.NotFound, exception.Message);

                return;
            }

            if (exception is ValidationException)
            {
                actionExecutedContext.Response = request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);

                return;
            }

            if (exception is ConflictException)
            {
                actionExecutedContext.Response = request.CreateErrorResponse(HttpStatusCode.Conflict, exception.Message);

                return;
            }

            actionExecutedContext.Response = request.CreateErrorResponse(HttpStatusCode.InternalServerError, TehnicalErrorMessage);
        }
    }
}