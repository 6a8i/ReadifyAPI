using Asp.Versioning;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Readify.API.Common.Controllers;
using Readify.Application.Features.Books.V1;
using Readify.Application.Features.Books.V1.Models.Requests;
using Readify.Application.Features.Books.V1.Models.Responses;

namespace Readify.API.Features.Books.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BooksController(IBooksAppServices appServices) : ControllerBase<BooksController>
    {
        private readonly IBooksAppServices _appServices = appServices;

        [HttpPost]
        public async Task<IActionResult> PostABook(AddBookRequest request)
        {
            Result<Guid> result = await _appServices.CreateABookAsync(request);

            if(result.IsFailed)
                return BadRequest(result.Errors.FirstOrDefault());

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            Result<List<Book>> result = await _appServices.GetAllBooksAsync();

            if (result.IsFailed)
                return BadRequest(result.Errors.FirstOrDefault());

            return Ok(result);
        }

    }
}
