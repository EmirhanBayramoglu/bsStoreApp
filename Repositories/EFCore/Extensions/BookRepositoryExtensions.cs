﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Dynamic.Core; //en son returndeki OrderBy için

namespace Repositories.EFCore.Extensions
{
    public static class BookRepositoryExtensions
    {
        //IQuaryable sıralama filtreleme listeleme işlemlerinde kullanılabilinir
        public static IQueryable<Book> FilterBook(this IQueryable<Book> books,
            uint minPrice, uint maxPrice) =>
            books.Where(Book =>
            Book.Price >= minPrice &&
            Book.Price <= maxPrice);

        public static IQueryable<Book> Search(this IQueryable<Book> books,
            string searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm))
                return books;

            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return books
                .Where(b => b.Title
                .ToLower()
                .Contains(searchTerm));
        }

        public static IQueryable<Book> Sort(this IQueryable<Book> books,
            string orderByQueryString)
        {
            if(string.IsNullOrWhiteSpace(orderByQueryString))
                return books.OrderBy(b => b.Id);
            
            var orderQuery = OrderQueryBuilder.CreatOrderQuery<Book>(orderByQueryString);

            if(orderQuery is null)
                return books.OrderBy(b => b.Id);

            return books.OrderBy(orderQuery);
        }
    }
}
