using Entities.Models;
using Entities.RequesFeatures;
using Entities.RequestFeatures;
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
        public BookRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        //bu fonksiyonların ekstra burada yapılmasının sebebi ekstra burada book için fazladan tanım veya sorgu ekleyebilmemiz
        //mesela getAll yaparız ama ekstra bunları Title'a göre sıralayıp çağırabiliriz
        public void CreateOneBook(Book book)
            => Create(book);
        public void DeleteOneBook(Book book)
            => Delete(book);
        public void UpdateOneBook(Book book)
            => Update(book);

        public async Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParamaters, bool trackChanges)
        {
            var books = await FindAll(trackChanges).FilterBooks(bookParamaters.MinPrice, bookParamaters.MaxPrice)
                .Search(bookParamaters.SearchTerms).Sort(bookParamaters.OrderBy).ToListAsync();
            return PagedList<Book>.ToPagedList(books, bookParamaters.PageNumber, bookParamaters.PageSize);
        }

        public async Task<Book> GetOneBookByIdAsync(int id, bool trackChanges)
            => await FindByConditons(b => b.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

    }
}
