using Readify.Application.Features.Authentications.V1.Models.Response;

namespace Readify.Application.Features.Authentications.V1.Models.Statics
{
    public static class AuthManager
    {
        private static readonly AsyncLocal<AuthResponse?> _context = new();

        public static AuthResponse? Context 
        {
            get => _context.Value;
            set => _context.Value = value;
        }

        public static Task ClearAsync()
        {
            return Task.FromResult(_context.Value = null);
        }
    }
}
