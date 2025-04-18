﻿using FluentResults;
using Readify.Application.Features.Users.V1.Models.Requests;
using Readify.Application.Features.Users.V1.Models.Responses;

namespace Readify.Application.Features.Users.V1
{
    public interface IUsersAppServices
    {
        Task<Result<Guid?>> CreateUserAsync(AddUserRequest request);
        Task<Result<List<GetUserResponse>>> GetAllUsersAsync();
        Task<Result<GetUserResponse>> GetUserByEmailAsync(string email);
        Task<Result<GetUserResponse>> GetUserByIdAsync(Guid id);
        Task<Result<string>> GetUserPasswordAsync(Guid id);
        Task<Result<GetUserResponse>> UpdateUserByIdAsync(Guid id, UpdateUserRequest request);
    }
}
