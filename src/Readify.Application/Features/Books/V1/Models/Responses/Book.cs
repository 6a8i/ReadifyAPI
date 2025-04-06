using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readify.Application.Features.Books.V1.Models.Responses
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public DateTime PublishDate { get; set; }
        public bool Status { get; set; }

        public static explicit operator Book(Infrastructure.Entities.Book v)
        {
            return new Book
            {
                Id = v.Id,
                Title = v.Title,
                Author = v.Author,
                Genre = v.Genre,
                PublishDate = v.PublishDate,
                Status = v.Status
            };
        }
    }
}
