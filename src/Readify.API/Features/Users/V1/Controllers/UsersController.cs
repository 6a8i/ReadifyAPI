using Asp.Versioning;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readify.API.Common.Controllers;
using Readify.Application.Features.Authentications.V1;
using Readify.Application.Features.Authentications.V1.Models.Requests;
using Readify.Application.Features.Authentications.V1.Models.Response;
using Readify.Application.Features.Users.V1;
using Readify.Application.Features.Users.V1.Models.Requests;
using Readify.Application.Features.Users.V1.Models.Responses;

namespace Readify.API.Features.Users.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController(IUsersAppServices usersAppServices, IAuthenticationAppServices authenticationAppServices) : ControllerBase<UsersController>
    {
        private readonly IUsersAppServices _usersAppServices = usersAppServices;
        private readonly IAuthenticationAppServices _authenticationAppServices = authenticationAppServices;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            Result<GetUserResponse> result = await _usersAppServices.GetUserByIdAsync(id);

            if (result.IsFailed)
                return BadRequest(result.Errors.FirstOrDefault());

            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserRequest request)
        {
            Result<GetUserResponse> result = await _usersAppServices.UpdateUserByIdAsync(id, request);

            if (result.IsFailed)
                return BadRequest(result.Errors);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            Result<AuthResponse> result = await _authenticationAppServices.LoginAsync(request);
            
            if (result.IsFailed)
                return BadRequest(result.Errors.FirstOrDefault());
            
            return Ok(result);
        }
    }
}
