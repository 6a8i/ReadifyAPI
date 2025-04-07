using FluentResults;
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

        public async Task<Result<GetUserResponse>> GetUserByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return Result.Fail("The id cannot be empty.");

            User? user = await _usersRepository.GetUserByIdAsync(id);
            
            if (user is null)
                return Result.Fail("User not found!");

            return (GetUserResponse)user;
        }
    }
}
