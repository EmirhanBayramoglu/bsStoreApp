using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Repositories.Contracts;
using Repositories.EFCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IRepositoryManager _manager;

        public BooksController(IRepositoryManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAllBooks() 
        {
            try
            {
                var books = _manager.Book.GetAllBooks(false);
                return Ok(books);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name = "id")] int id)
        {

            try
            {
                var book = _manager.
                    Book.GetOneBookById(id, false);
                //SingleOrDefault LINQ içerisinde arama metodudur (sadece 1 tane olanı getir yada defualt değeri döndür demek)

                if (book == null)
                {
                    return NotFound(); //404
                } 
                return Ok(book);
}
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            try
            {
                if (book == null)
                    return BadRequest(); //400

                _manager.Book.Create(book);
                _manager.Save();
                return StatusCode(201, book);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] Book book)
        {
            var entity = _manager.Book.GetOneBookById(id, false);

            //girilen id'ye denk gelen bir obje var mı kontrol ediyoruz
            if (entity == null)
                return NotFound(); //404

            if (id != entity.Id)
                return BadRequest(); //400

            entity.Title = book.Title; //book kısmı yeni değerler yani postman yada swagger üzerinden girdiğimiz değerler
            entity.Price = book.Price;

            _manager.Save();

            return Ok(book);
        }

 


        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {

            try
            {
                var entity = _manager.Book.GetOneBookById(id, false);
                //SingleOrDefault LINQ içerisinde arama metodudur (sadece 1 tane olanı getir yada defualt değeri döndür demek)

                if (entity == null)
                    return NotFound(new
                    {
                        StatusCode = 404,
                        message = $"Book with id:{id} could not found."
                    }); //404

                _manager.Book.Delete(entity);
                _manager.Save();

                return NoContent(); //204
            }
            catch (Exception)
            {

                throw;
            }

            
        }

        //patch olayı swagger üzerinde biraz garip pdf bakarak değer verme olayını anlayabilirsin
        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id
            , [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            try
            {
                var entity = _manager.Book.GetOneBookById(id, false);

                if (entity == null)
                    return BadRequest(); //400

                bookPatch.ApplyTo(entity);
                _manager.Book.Update(entity);
                _manager.Save();
                return NoContent(); //204
            }
            catch (Exception)
            {

                throw;
            }
            //check entity
            

        }

        
    }
}
