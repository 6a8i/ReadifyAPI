using Readify.Application.Features.Books.V1.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readify.Application.Features.Books.V1.Infrastructure.IRepositories
{
    public interface IBooksRepository
    {
        Task<Guid?> AddAsync(Book entity);
    }
}
