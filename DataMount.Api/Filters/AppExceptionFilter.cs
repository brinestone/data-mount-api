using AutoMapper;
using DataMount.Api.Payloads;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace DataMount.Api.Filters;

public class AppExceptionFilter(
    ILogger<AppExceptionFilter> logger,
    IStringLocalizer<AppExceptionFilter> stringLocalizer,
    IMapper mapper) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "An unhandled application exception occurred.");
        var payload = mapper.Map<ErrorMessagePayload>(context.Exception);
        context.Result = new ObjectResult(new
        {
            Error = stringLocalizer.GetString("unknown_error").Value
        })
        {
            StatusCode = payload.Status
        };
    }
}