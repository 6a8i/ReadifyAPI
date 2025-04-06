using Readify.Application.Features.Books.V1.Infrastructure.Entities;

namespace Readify.Application.Features.Books.V1.Models.Requests
{
    public class AddBookRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public DateTime PublishDate { get; set; }

        public static implicit operator Book(AddBookRequest model)
        {            
            return new Book {
                PublishDate = model.PublishDate,
                Author = model.Author,
                Genre = model.Genre,
                Title = model.Title
            };
        }
    }
}
