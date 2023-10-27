using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public static class BookRepositoryExtensions
    {
        //IQuaryable sıralama filtreleme listeleme işlemlerinde kullanılabilinir
        public static IQueryable<Book> FilterBook(this IQueryable<Book> books,
            uint minPrice, uint maxPrice) =>
            books.Where(Book =>
            (Book.Price >= minPrice &&
            Book.Price <= maxPrice));
    }
}
