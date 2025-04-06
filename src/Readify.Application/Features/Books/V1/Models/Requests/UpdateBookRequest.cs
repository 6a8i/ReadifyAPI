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

        public static implicit operator Book(UpdateBookRequest model)
        {
            return new Book
            {
                PublishDate = model.PublishDate ?? DateTime.UtcNow,
                Author = model.Author ?? string.Empty,
                Genre = model.Genre ?? string.Empty,
                Title = model.Title ?? string.Empty
            };
        }
    }
}
