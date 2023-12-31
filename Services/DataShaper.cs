﻿using Entities.Models;
using Services.Contract;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DataShaper<T> : IDataShaper<T>
        where T : class
    {
        public PropertyInfo[] Properties { get; set; } // bu kısım mesela book aldığımızda Title ID ve Price içerir yani
                                                       //  nesnenin özelliklerini içerir
        public DataShaper()
        {
            Properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance); //public ve newlenerek elde edilen propertiesleri ver
        }

        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldString)
        {
            var requiredFields = GetRequiredProperties(fieldString);
            return FetchData(entities, requiredFields);
        } //liste halinde shaped entities

        public ShapedEntity ShapeData(T entity, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);
            return FetchDataForEntity(entity, requiredProperties);
        } //tek bir shaped entity

        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            var requiredFiled = new List<PropertyInfo>();
            if(!string.IsNullOrWhiteSpace(fieldsString))
            {
                var fields = fieldsString.Split(',', 
                    StringSplitOptions.RemoveEmptyEntries);

                foreach( var field in fields)
                {
                    var property = Properties
                        .FirstOrDefault(pi => pi.Name.Equals(field.Trim(),
                        StringComparison.InvariantCultureIgnoreCase));//küçük büyük harf olayını ayarlar
                    if (property is null)
                        continue;
                    requiredFiled.Add(property);
                }
            }
            else
            {
                requiredFiled = Properties.ToList();
            }

            return requiredFiled;
        }

        private ShapedEntity FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ShapedEntity(); //run time da bu nesene üretileceği için ExpandoObject Yapıyoruz

            foreach( var property in requiredProperties) //çekilen nesnelerin property value'larını çekiyo
                                                         //id title ise onların değerlerini alıyo
            {
                var objectPropertyValue = property.GetValue(entity);//entity property değerlerini alıyo
                shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
            }

            var objectProperty = entity.GetType().GetProperty("Id"); //sekillendirilmiş objenin her türlü id'sini tutuyoruz ileride 
                                                                     //link olayları için kullanacağız
            shapedObject.Id = (int)objectProperty.GetValue(entity);

            return shapedObject;
        }

        private IEnumerable<ShapedEntity> FetchData(IEnumerable<T> entities,
            IEnumerable<PropertyInfo> requiredProperties) //üst metotda entityleri ayarlar bu metotda
                                                          //o ayarlanan entityleri listlememizi sağlar 
        {
            var shapedData = new List<ShapedEntity>();
            foreach(var entity in entities)
            {
                var shapedObject = FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }
            return shapedData;
        }
    }
}
