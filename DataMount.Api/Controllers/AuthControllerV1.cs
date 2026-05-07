using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using BenScr.RandomNameGenerator;
using DataMount.Api.Options;
using DataMount.Api.Payloads;
using DataMount.App.Inputs;
using DataMount.App.Services.Contracts;
using DataMount.Domain.Exceptions;
using DataMount.Domain.Models.Identity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DataMount.Api.Controllers;

[ApiController]
[Route($"{Constants.ApiBasePath}/auth")]
[Tags("Auth")]
[ApiVersion("1.0")]
public class AuthControllerV1(ILogger<AuthControllerV1> logger, IMapper mapper) : ControllerBase
{
    [EndpointName("signOut")]
    [ValidateAntiForgeryToken]
    [EndpointSummary("Sign out")]
    [HttpPost("sign-out")]
    [Authorize(CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SignOutAsync()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }

    [Authorize]
    [HttpGet("aif-token")]
    [EndpointSummary("Obtain antiforgery token")]
    [EndpointName("getAntiforgeryToken")]
    [EndpointDescription("Generate a new anti-forgery token for the current session")]
    public IActionResult GetAntiforgeryToken([FromServices] IAntiforgery antiforgery)
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        // Send the token back in a cookie that JS can read
        Response.Cookies.Append(Constants.AntiForgeryHeaderName, tokens.RequestToken!,
            new CookieOptions { HttpOnly = false });

        return Ok();
    }

    [AllowAnonymous]
    [ApiVersion("1.0")]
    [HttpPost("sign-in/email")]
    [EndpointSummary("Email sign in")]
    [EndpointName("emailSignIn")]
    [ProducesResponseType(typeof(SessionDto<Guid>), StatusCodes.Status202Accepted, "application/json")]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Description = "The user is banned",
        Type = typeof(ErrorMessagePayload))]
    [EndpointDescription("Sign in a user using their email credentials")]
    public async Task<IActionResult> EmailSignIn(
        [FromBody] EmailSignInRequest dto,
        [FromServices] IAuthService<Guid> @is,
        [FromServices] IOptions<CookieAuthOptions> jwtOptions
    ) {
        try
        {
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
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = dto.StaySignedIn,
                    AllowRefresh = dto.StaySignedIn,
                    ExpiresUtc = expiration
                });
            Response.Cookies.Append(Constants.SessionIdCookie, result.Id.ToString(), new CookieOptions
            {
                HttpOnly = false,
                Path = Constants.ApiBasePath,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,
                Expires = expiration
            });
            return Accepted(result);
        }
        catch (Exception e)
        {
            var payload = mapper.Map<ErrorMessagePayload>(e);
            return Problem(statusCode: payload.Status, detail: payload.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("sign-up/email")]
    [EndpointSummary("Email sign up")]
    [EndpointName("emailSignUp")]
    [EndpointDescription("Create a new user account using email")]
    [ProducesResponseType(StatusCodes.Status409Conflict, Description = "The identifier is already in use",
        Type = typeof(ErrorMessagePayload))]
    public async Task<IActionResult> EmailSignUp([FromBody] EmailSignUpRequest dto,
        [FromServices] IAuthService<Guid> authService)
    {
        try
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

            logger.LogInformation("New email user created: {userId}", user.Id);

            return Created();
        }
        catch (Exception e)
        {
            var payload = mapper.Map<ErrorMessagePayload>(e);
            return Problem(statusCode: payload.Status, detail: payload.Message);
        }
    }
}