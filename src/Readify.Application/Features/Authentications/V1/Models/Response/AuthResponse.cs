using Readify.Application.Features.Authentications.V1.Infrastructure.Entities;

namespace Readify.Application.Features.Authentications.V1.Models.Response
{
    public class AuthResponse
    {
        public Guid Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime TokenCreatedAt { get; set; }
        public DateTime TokenExpiresAt { get; set; }
        public bool TokenHasExpired { get; set; }

        public static explicit operator AuthResponse(Token v)
        {
            return new AuthResponse
            {
                Token = v.Id,
                UserId = v.UserId,
                TokenCreatedAt = v.CreatedAt,
                TokenExpiresAt = v.ExpiresAt,
                TokenHasExpired = v.HasExpired
            };
        }
    }
}
