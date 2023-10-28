using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore.Extensions
{
    public static class OrderQueryBuilder
    {
        public static string CreatOrderQuery<T>(string orderByQueryString)
        {
            var orderParams = orderByQueryString.Trim().Split(','); //trim fazla boşlukları atmak için split de peş peşe girilen sırlama tercihleri için aradaki virgülü
                                                                    //atıp array oluşturup ona göre sıralaması için (Title,id => array[0]=Title array[1]=id gibi)

            var propertyInfos = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance); //book nesnesinin propertylerini alıyoruz (public ve instance olanları(newlenebilen))

            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(' ')[0]; //desc yani azdan çoğa mı a dan z ye mi onu anlamamızı sağlayacak kısım burası

                var objectProperty = propertyInfos
                    .FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName,
                    StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty is null)
                    continue;

                var direction = param.EndsWith(" desc") ? "descending" : "ascending";

                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction},");

                var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

                return orderQuery;
            }
        }
    }
}
