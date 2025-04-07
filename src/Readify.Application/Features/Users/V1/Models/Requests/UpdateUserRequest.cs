namespace Readify.Application.Features.Users.V1.Models.Requests
{
    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
