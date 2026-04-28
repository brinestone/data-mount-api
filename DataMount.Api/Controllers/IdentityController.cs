using AutoMapper;
using DataMount.Api.Dto;
using DataMount.App.Inputs;
using DataMount.App.Services.Contracts;
using DataMount.Domain.Models.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DataMount.Api.Controllers;

[ApiController]
[Route("api/identity")]
[Tags("Identity")]
public class IdentityController(ILogger<IdentityController> logger, IMapper mapper) : ControllerBase
{
    [HttpPost("sign-up/email")]
    [EndpointSummary("Email sign up")]
    [EndpointDescription("Create a new user account using email")]
    public async Task<IActionResult> EmailSignUp([FromBody] EmailSignUpRequest dto,
        [FromServices] IIdentityService<Guid> identityService)
    {
        var user = await identityService.CreateUserFromCredentialAsync(new CreateUserWithCredentialInput
        (
            ContactType: ContactType.Email,
            FirstName: dto.FirstName,
            Identifier: dto.Email!,
            LastName: dto.LastName,
            Password: dto.Password
        ));

        logger.LogInformation("New email user created: {0}", user.Id);

        return Created();
    }
}