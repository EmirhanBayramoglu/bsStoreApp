using Entities.Models;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        public BookRepository(RepositoryContext context) : base(context)
        {
            
        }
        //bu fonksiyonların ekstra burada yapılmasının sebebi ekstra burada book için fazladan tanım veya sorgu ekleyebilmemiz
        //mesela getAll yaparız ama ekstra bunları Title'a göre sıralayıp çağırabiliriz
        public void CreateOneBook(Book book) => Create(book);

        public void DeleteOneBook(Book book) => Delete(book);

        public IQueryable<Book> GetAll(bool trackChanges) =>
            FindAll(trackChanges);

        public IQueryable<Book> GetOneBookById(int id, bool trackChanges) =>
            FindByCondition(x => x.Id.Equals(id), trackChanges);

        public void UpdateOneBook(Book book) => Update(book);
    }
}
