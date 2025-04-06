using Readify.Application.Features.Books.V1.Infrastructure.Entities;

namespace Readify.Application.Features.Books.V1.Models.Requests
{
    public class UpdateBookRequest
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool? Status { get; set; }
    }
}
