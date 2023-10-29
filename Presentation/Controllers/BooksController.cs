using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.XPath;
using Entities.Models;
using Repositories.Contracts;
using Entities.Exceptions;
using Entities.DataTransferObjects;
using Presentation.ActionFilters;
using Entities.RequestFeatures;
using System.Text.Json;
using Entities.LinkModels;
using Entities.RequesFeatures;
using Services.Contract;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    //[ApiVersion("1.0")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/books")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    //[ResponseCache(CacheProfileName = "5mins")] //tüm controllerlara 5 dakikalık cache uygunaldı ("5mins" değeri program.cs içerisinde belirlendi)
    //[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 80)] //bu şekilde serviceExtensions içerisindeki değerlerin üzerine
    //başka değerler yazabilriiz belirli yerler için
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public BooksController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [Authorize] //Authentication koruması
        [HttpHead]
        [HttpGet(Name = "GetAllBooksAsync")]
        [ServiceFilter(typeof(ValidatorMediaTypeAttribute))]
        //[ResponseCache(Duration = 60)] //60 saniye boyunca Chachelenbilir (depolanabilir) 
        //bunu silmezsek bookcontroller classının üzerindeki 5 dakikalık cache kullanılmaz bu controller için
        public async Task<IActionResult> GetAllBooksAsync([FromQuery] BookParameters bookParamaters)
        {
            var linkParamaters = new LinkParameters()
            {
                BookParameters = bookParamaters,
                HttpContext = HttpContext
            };

            var result = await _serviceManager.BookService.GetAllBooksAsync(linkParamaters, false);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));
            return result.linkResponse.HasLink ? Ok(result.linkResponse.LinkedEntitites) : Ok(result.linkResponse.ShapedEntities);
        }
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBooksAsync([FromRoute(Name = "id")] int id)
        {
            var book = await _serviceManager.BookService.GetOneBookByIdAsync(id, false);

            return Ok(book); // 200
        }
        [Authorize(Roles = "Editor, Admin")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost(Name = "CreateOneBookAsync")]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
            var book = await _serviceManager.BookService.CreateOneBookAsync(bookDto);
            return StatusCode(201, book); // 201
        }
        [Authorize(Roles = "Editor, Admin")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate book)
        {
            await _serviceManager.BookService.UpdateOneBookAsync(id, book, false);
            return NoContent(); // 204
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {
            await _serviceManager.BookService.DeleteOneBookAsync(id, false);
            return NoContent(); // 204
        }

        //patch olayı swagger üzerinde biraz garip pdf bakarak değer verme olayını anlayabilirsin
        [Authorize(Roles = "Editor, Admin")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest(); // 400

            var result = await _serviceManager.BookService.GetOneBookForPatchAsync(id, false);

            TryValidateModel(result.bookDtoForUpdate);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState); // 422

            await _serviceManager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);


            return NoContent(); // 204
        }
        [Authorize]
        [HttpOptions]
        public IActionResult GetBookOptions()
        {
            Response.Headers.Add("Allow", "GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS");
            return Ok();
        }
    }
}
