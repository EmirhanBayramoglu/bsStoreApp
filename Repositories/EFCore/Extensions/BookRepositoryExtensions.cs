using Entities.Models;
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

            var orderParams = orderByQueryString.Trim().Split(','); //trim fazla boşlukları atmak için split de peş peşe girilen sırlama tercihleri için aradaki virgülü
                                                                    //atıp array oluşturup ona göre sıralaması için (Title,id => array[0]=Title array[1]=id gibi)
            var propertyInfos = typeof(Book)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance); //book nesnesinin propertylerini alıyoruz (public ve instance olanları(newlenebilen))

            var orderQueryBuilder = new StringBuilder();

            foreach(var param in orderParams)
            {
                if(string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(' ')[0]; //desc yani azdan çoğa mı a dan z ye mi onu anlamamızı sağlayacak kısım burası

                var objectProperty = propertyInfos
                    .FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName,
                    StringComparison.InvariantCultureIgnoreCase));

                if(objectProperty is null)
                    continue;

                var direction = param.EndsWith(" desc") ? "descending" : "ascending";

                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction},");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

            if(orderQuery is null)
                return books.OrderBy(b => b.Id);

            return books.OrderBy(orderQuery);
        }
    }
}
