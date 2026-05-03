using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BenScr.RandomNameGenerator;
using DataMount.Api.Options;
using DataMount.Api.Payloads;
using DataMount.App.Inputs;
using DataMount.App.Services.Contracts;
using DataMount.Domain.Exceptions;
using DataMount.Domain.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DataMount.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Tags("Auth")]
public class AuthController(ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("sign-in/email")]
    [EndpointSummary("Email sign in")]
    [EndpointName("emailSignIn")]
    [ProducesResponseType(typeof(SessionDto<Guid>), StatusCodes.Status202Accepted, "application/json")]
    [EndpointDescription("Sign in a user using their email credentials")]
    public async Task<IActionResult> EmailSignIn(
        [FromBody] EmailSignInRequest dto,
        [FromServices] IAuthService<Guid> @is,
        [FromServices] IMapper mapper,
        [FromServices] IOptions<CookieAuthOptions> jwtOptions,
        [FromServices] IWebHostEnvironment env)
    {
        // TODO: Enforce ban policies.
        var input = mapper.Map<CreateCredentialSessionInput>(dto);
        input.Ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        input.UserAgent = Request.Headers.UserAgent!;
        var session = await @is.CreateUserCredentialSessionAsync(input);

        var result = mapper.Map<SessionDto<Guid>>(session);

        var expiration = DateTime.Now.AddMilliseconds(jwtOptions.Value.Lifetime);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, input.Identifier),
            new(Constants.Sub, session.Account!.OwnerId.ToString()!),
            new(Constants.PermissionsId, session.Account.OwnerId.ToString()!)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = dto.StaySignedIn,
                ExpiresUtc = expiration
            });

        return Accepted(result);
    }

    private static string GenerateJwt(Session<Guid> session, string secretKey, string issuer, DateTime expiration,
        string? audience)
    {
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, session.Account?.OwnerId.ToString() ?? ""),
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("sign-up/email")]
    [EndpointSummary("Email sign up")]
    [EndpointName("emailSignUp")]
    [EndpointDescription("Create a new user account using email")]
    [ProducesResponseType(StatusCodes.Status409Conflict, Description = "The identifier is already in use")]
    public async Task<IActionResult> EmailSignUp([FromBody] EmailSignUpRequest dto,
        [FromServices] IAuthService<Guid> authService)
    {
        var gen = new NameGenerator();
        var user = await authService.CreateUserFromCredentialAsync(new CreateUserWithCredentialInput
        (
            ContactType: ContactType.Email,
            FirstName: gen.FirstName(),
            Identifier: dto.Email!,
            LastName: gen.LastName(),
            Password: dto.Password
        ));

        logger.LogInformation("New email user created: {0}", user.Id);

        return Created();
    }
}