using Entities.Models;
using Entities.RequesFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EFCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public sealed class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        public BookRepository(RepositoryContext context) : base(context)
        {

        }
        //bu fonksiyonların ekstra burada yapılmasının sebebi ekstra burada book için fazladan tanım veya sorgu ekleyebilmemiz
        //mesela getAll yaparız ama ekstra bunları Title'a göre sıralayıp çağırabiliriz
        public void CreateOneBook(Book book) => Create(book);

        public void DeleteOneBook(Book book) => Delete(book);

        public async Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParameters, bool trackChanges)
            {
            var books = await FindAll(trackChanges)
                .FilterBook(bookParameters.MinPrice,bookParameters.MaxPrice)
                .Search(bookParameters.SearchTerms)
                .Sort(bookParameters.OrderBy)
                .ToListAsync();

            return PagedList<Book>.ToPagedList(books, bookParameters.PageNumber,bookParameters.PageSize);
            }
            

        public async Task<Book> GetOneBookByIdAsync(int id, bool trackChanges) =>
            await FindByCondition(x => x.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

        public void UpdateOneBook(Book book) => Update(book);
    }
}
