using Readify.API.Common.Controllers;
using Readify.Application.Features.Books.V1;

namespace Readify.API.Features.Books.V1.Controllers
{
    public class BooksController(IBooksAppServices appServices) : ControllerBase<BooksController>
    {
        private readonly IBooksAppServices _appServices = appServices;
    }
}
