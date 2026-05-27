using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataMount.Api.Controllers;

[Authorize]
[Tags("Project")]
[ApiController]
[ApiVersion("1.0")]
[Route($"{Constants.ApiBasePath}/projects")]
public class ProjectsControllerV1 : ControllerBase
{
    [HttpGet]
    [EndpointName("findProjects")]
    [EndpointSummary("Find Projects")]
    [EndpointDescription("Gets the projects the current user can access")]
    public async Task<IActionResult> FindProjectsAsync()
    {
        return Ok();
    }
}