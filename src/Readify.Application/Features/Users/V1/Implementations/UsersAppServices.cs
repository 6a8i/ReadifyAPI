﻿using FluentResults;
using Readify.Application.Features.Users.V1.Infrastructure.Entities;
using Readify.Application.Features.Users.V1.Infrastructure.IRepositories;
using Readify.Application.Features.Users.V1.Models.Requests;
using Readify.Application.Features.Users.V1.Models.Responses;

namespace Readify.Application.Features.Users.V1.Implementations
{
    public class UsersAppServices(IUsersRepository usersRepository) : IUsersAppServices
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        public async Task<Result<Guid?>> CreateUserAsync(AddUserRequest request)
        {
            if(request is null)
                return Result.Fail("The request cannot be null");

            if (string.IsNullOrEmpty(request.Name) || 
                string.IsNullOrEmpty(request.Email) || 
                string.IsNullOrEmpty(request.Password) || 
                request.BirthDate < DateTime.UtcNow.AddYears(-150) || 
                request.BirthDate > DateTime.UtcNow)
                return Result.Fail("All fields are required");

            User? entity = await _usersRepository.GetUserByEmailAsync(request.Email);

            if (entity is not null && entity.IsActive)
                return Result.Fail("User already exists!");

            else if (entity is not null && !entity.IsActive)
            {
                entity.IsActive = true;
                entity.Password = request.Password;
                bool result = await _usersRepository.UpdateAsync(entity);

                if (result)
                    return Result.Ok<Guid?>(entity.Id);
                else
                    return Result.Fail("Something went wrong! Try again later.");
            }
            else
            {
                entity = (User)request;

                Guid? id = await _usersRepository.AddAsync(entity);

                if (id is null)
                    return Result.Fail("Something went wrong! Try again later.");

                return Result.Ok(id);
            }
        }

        public async Task<Result<List<GetUserResponse>>> GetAllUsersAsync()
        {
            List<User> users = await _usersRepository.GetAllAsync();

            if (users is null || users.Count == 0)
                return Result.Fail("No users found!");

            return users.Select(user => (GetUserResponse)user).ToList().ToResult();
        }

        public async Task<Result<GetUserResponse>> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return Result.Fail<GetUserResponse>("The email cannot be empty.");

            User? user = await _usersRepository.GetUserByEmailAsync(email);

            if (user is null || !user.IsActive)
                return Result.Fail<GetUserResponse>("User not found!");

            return (GetUserResponse)user;
        }

        public async Task<Result<GetUserResponse>> GetUserByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return Result.Fail("The id cannot be empty.");

            User? user = await _usersRepository.GetUserByIdAsync(id);
            
            if (user is null)
                return Result.Fail("User not found!");

            return (GetUserResponse)user;
        }

        public async Task<Result<string>> GetUserPasswordAsync(Guid id)
        {
            if (id == Guid.Empty)
                return Result.Fail<string>("The id cannot be empty.");

            string? password = await _usersRepository.GetPasswordByEmailAsync(id);

            if (string.IsNullOrEmpty(password))
                return Result.Fail<string>("User not found!");

            return Result.Ok(password);
        }

        public async Task<Result<GetUserResponse>> UpdateUserByIdAsync(Guid id, UpdateUserRequest request)
        {
            if (id == Guid.Empty)
                return Result.Fail("The id cannot be empty.");

            User? user = await _usersRepository.GetUserByIdAsync(id);

            if (user is null)
                return Result.Fail("User not found!");

            if(!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            if (!string.IsNullOrEmpty(request.Name))
                user.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Password))
                user.Password = request.Password;

            if (request.IsActive is not null)
                user.IsActive = request.IsActive.Value;

            if (request.BirthDate is not null && request.BirthDate > DateTime.UtcNow.AddYears(-150) && request.BirthDate < DateTime.UtcNow)
                user.BirthDate = request.BirthDate.Value;

            bool result = await _usersRepository.UpdateAsync(user);

            if (!result)
                return Result.Fail("Something went wrong! Try again later.");
            
            return (GetUserResponse)user;
        }
    }
}
