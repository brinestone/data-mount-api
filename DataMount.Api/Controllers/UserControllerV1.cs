using Asp.Versioning;
using AutoMapper;
using DataMount.Api.Payloads;
using DataMount.App.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataMount.Api.Controllers;

[Tags("Users")]
[ApiController]
[ApiVersion("1.0")]
[Route($"{Constants.ApiBasePath}/users")]
public class UserControllerV1 : ControllerBase
{
    [Authorize]
    [EndpointName("getMe")]
    [EndpointSummary("Get current user information")]
    [HttpGet("me")]
    [ProducesDefaultResponseType(typeof(UserDto<Guid>), Description = "A user's information")]
    public async Task<IActionResult> GetMe(
        [FromServices] IUserService<Guid> userService,
        [FromServices] IMapper mapper
    )
    {
        try
        {
            Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == Constants.Sub)?.Value, out var userId);
            var user = await userService.FindUserByIdAsync(userId);
            var userDto = mapper.Map<UserDto<Guid>>(user);
            return Ok(userDto);
        }
        catch (Exception e)
        {
            var payload = mapper.Map<ErrorMessagePayload>(e);
            return Problem(statusCode: payload.Status, detail: payload.Message);
        }
    }
}