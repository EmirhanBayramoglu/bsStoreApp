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
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            
            if (book == null)
                return BadRequest(); //400

            _manager.BookService.CreateOneBook(book);

            return StatusCode(201, book);

        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] Book book)
        {
            var entity = _manager.BookService.GetOneBookById(id, true);

            //girilen id'ye denk gelen bir obje var mı kontrol ediyoruz
            if (entity == null)
                throw new BookNotFoundException(id);

            entity.Title = book.Title; //book kısmı yeni değerler yani postman yada swagger üzerinden girdiğimiz değerler
            entity.Price = book.Price;

            return Ok(book);
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
            , [FromBody] JsonPatchDocument<Book> bookPatch)
        {

            var entity = _manager.BookService.GetOneBookById(id, true);

            bookPatch.ApplyTo(entity);
            _manager.BookService.UpdateOneBook(id, entity, true);
            return NoContent(); //204

        }
    }
}
