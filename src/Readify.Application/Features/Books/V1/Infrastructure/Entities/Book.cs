using System.ComponentModel.DataAnnotations;

namespace Readify.Application.Features.Books.V1.Infrastructure.Entities
{
    public class Book
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public DateTime PublishDate { get; set; }
        public bool Status { get; set; }
    }
}
