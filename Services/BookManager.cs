using Entities.Models;
using Repositories.Contracts;
using Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BookManager : IBookService
    {
        private readonly IRepositoryManager _manager;

        public BookManager(IRepositoryManager manager)
        {
            _manager = manager;
        }

        public Book CreateOneBook(Book book)
        {
            _manager.Book.Create(book);
            _manager.Save();
            return book;
        }

        public void DeleteOneBook(int id, bool trackChange)
        {
            var entity = _manager.Book.GetOneBookById(id, trackChange);
            if (entity != null)
                throw new Exception($"Book with id:{id} could not found.");

            _manager.Book.DeleteOneBook(entity);
            _manager.Save();
        }

        public IEnumerable<Book> GetAllBooks(bool trackChanges)
        {
            return _manager.Book.GetAllBooks(trackChanges);
        }

        public Book GetOneBookById(int id, bool trackChange)
        {
            return _manager.Book.GetOneBookById(id,trackChange);
        }

        public void UpdateOneBook(int id, Book book, bool trackChanges)
        {
            var entity = _manager.Book.GetOneBookById(id, trackChanges);
            if (entity != null)
                throw new Exception($"Book with id:{id} could not found.");

            entity.Title = book.Title;
            entity.Price = book.Price;

            _manager.Book.UpdateOneBook(entity);
            _manager.Save();
        }
    }
}
