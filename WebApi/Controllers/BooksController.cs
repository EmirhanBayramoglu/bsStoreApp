using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public BooksController(RepositoryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllBooks() 
        {
            try
            {
                var books = _context.Books.ToList();
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
                var book = _context.
                    Books.Where(x => x.Id.Equals(id)).SingleOrDefault();
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

                _context.Books.Add(book);
                _context.SaveChanges();
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
            var entity = _context.Books.Where(x => x.Id.Equals(id)).SingleOrDefault();

            //girilen id'ye denk gelen bir obje var mı kontrol ediyoruz
            if (entity == null)
                return NotFound(); //404

            if (id != entity.Id)
                return BadRequest(); //400

            entity.Title = book.Title; //book kısmı yeni değerler yani postman yada swagger üzerinden girdiğimiz değerler
            entity.Price = book.Price;

            _context.SaveChanges();

            return Ok(book);
        }

 


        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {

            try
            {
                var entity = _context.
                    Books.Where(x => x.Id.Equals(id)).SingleOrDefault();
                //SingleOrDefault LINQ içerisinde arama metodudur (sadece 1 tane olanı getir yada defualt değeri döndür demek)

                if (entity == null)
                    return NotFound(new
                    {
                        StatusCode = 404,
                        message = $"Book with id:{id} could not found."
                    }); //404

                _context.Books.Remove(entity);
                _context.SaveChanges();

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
                var entity = _context.Books.Where(x => x.Id.Equals(id)).SingleOrDefault();

                if (entity == null)
                    return BadRequest(); //400

                bookPatch.ApplyTo(entity);
                _context.SaveChanges();
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
