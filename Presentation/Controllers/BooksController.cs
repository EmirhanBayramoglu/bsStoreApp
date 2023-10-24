using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var books = _manager.BookService.GetAllBooks(true);
            return Ok(books);

        }

        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name = "id")] int id)
        {

            var book = _manager.
                BookService.GetOneBookById(id, true);

            return Ok(book);
          
        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] BookDtoForInsertion bookDto)
        {
            
            if (bookDto == null)
                return BadRequest(); //400

            //bu işlem  error mesajlarını bastırma işleminden kaçındıktan sonra sadece bizim yazdığımız
            //error mesajlarının görünmesini sağlar "program.cs" içerisindeki services.configure olayı bastıma işleminin içerir
            if(!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var book = _manager.BookService.CreateOneBook(bookDto);

            return StatusCode(201, book); //Created at route

        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate bookDto)
        {
            if (bookDto is null)
                return BadRequest(); //400

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            _manager.BookService.UpdateOneBook(id, bookDto, false);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            _manager.BookService.DeleteOneBook(id, true);
            return NoContent(); //204
        }

        //patch olayı swagger üzerinde biraz garip pdf bakarak değer verme olayını anlayabilirsin
        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id
            , [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
            if(bookPatch is null)
                return BadRequest();

            var result = _manager.BookService.GetOneBookForPatch(id, false);

            var bookDto = _manager.BookService.GetOneBookById(id, true);

            bookPatch.ApplyTo(result.bookDtoForUpdate, ModelState);

            TryValidateModel(result.bookDtoForUpdate);

            if(!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            _manager.BookService.SaveChangesForPatch(result.bookDtoForUpdate,result.book);
            return NoContent(); //204

        }
    }
}
