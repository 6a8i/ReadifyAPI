using Asp.Versioning;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Readify.API.Common.Controllers;
using Readify.Application.Features.Users.V1;
using Readify.Application.Features.Users.V1.Models.Requests;
using Readify.Application.Features.Users.V1.Models.Responses;

namespace Readify.API.Features.Users.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController(IUsersAppServices usersAppServices) : ControllerBase<UsersController>
    {
        private readonly IUsersAppServices _usersAppServices = usersAppServices;

        [HttpPost]
        public async Task<IActionResult> PostAUser(AddUserRequest request)
        {
            Result<Guid?> result = await _usersAppServices.CreateUserAsync(request);

            if (result.IsFailed)
                return BadRequest(result.Errors.FirstOrDefault());

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            Result<List<GetUserResponse>> result = await _usersAppServices.GetAllUsersAsync();

            if (result.IsFailed)
                return BadRequest(result.Errors.FirstOrDefault());

            return Ok(result);
        }
    }
}
