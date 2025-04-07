using FluentResults;
using Readify.Application.Features.Users.V1.Models.Requests;

namespace Readify.Application.Features.Users.V1
{
    public interface IUsersAppServices
    {
        Task<Result<Guid?>> CreateUserAsync(AddUserRequest request);
    }
}
