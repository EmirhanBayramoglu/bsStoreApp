using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace WebApi.Utilities.AutoMapper
{
    //AutoMapper profil sınıfı
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //veri kaynağı ve dönüşeceği bir obje girilir
            CreateMap<BookDtoForUpdate, Book>();
        }
    }
}
