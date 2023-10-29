using Entities.DataTransferObjects;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Presentation.ActionFilters;
using Presentation.Controllers;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contract;

namespace WebApi.Extensions
{
    //Service oluşturma klasörü

    //Singleton ile Tanımlanma Özelliği
    //Dikkat edilmesi gereken bir nokta, "AddSingleton" ile kaydedilen nesnelerin uygulama başladığında oluşturulduğu
    //ve uygulama sonlandığında yok edildiği, bu nedenle gerektiğinde dikkatli bir şekilde kullanılmalıdır.

    //Scoped ile Tanımlanma Özelliği
    //AddScoped hizmetleri, bir HTTP isteği başladığında oluşturulur ve istek tamamlandığında yok edilir.

    public static class ServiceExtensions
    {
        //birşeyler yapılandırmaya, genişletmeye yarayan şeyler yazarız bu kısımda
        public static void ConfigureSqlContext(this IServiceCollection services,
            IConfiguration configuration) => services.AddDbContext<RepositoryContext>(options =>
                                                options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

        //alt kısımda Repositorylerin çözümlenmesindeki problemi çözdük (çözümleme için AddScope kullanılır genelde)
        public static void ConfigureRepositoryManager(this IServiceCollection service) =>
            service.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerService, LoggerManager>();

        public static void ConfigureActionFilter(this IServiceCollection services)
            {
            services.AddScoped<ValidationFilterAttribute>();
            services.AddSingleton<LogFilterAttribute>();
            services.AddScoped<ValidatorMediaTypeAttribute>();
            }

        //front'dan api istek için izin verme servisi
        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("X-Pagination")
                );
            });

        public static void ConfigureDataShaper(this IServiceCollection services)
        {
            services.AddScoped<IDataShaper<BookDto>, DataShaper<BookDto>>();
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemTextJsonOutputFormatter = config.OutputFormatters.OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();

                if (systemTextJsonOutputFormatter is not null)
                {
                    systemTextJsonOutputFormatter
                    .SupportedMediaTypes.Add("application/vnd.btkakademi.hateoas+json");

                    systemTextJsonOutputFormatter
                    .SupportedMediaTypes.Add("application/vnd.btkakademi.apiroot+json");
                }

                var xmlOutputFormatter = config
                .OutputFormatters.OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                if (xmlOutputFormatter is not null)
                {
                    xmlOutputFormatter
                    .SupportedMediaTypes.Add("application/vnd.btkakademi.hateoas+xml");
                    xmlOutputFormatter
                    .SupportedMediaTypes.Add("application/vnd.btkakademi.apiroot+xml");
                }
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true; //version bilgisini header'a ekleme
                opt.AssumeDefaultVersionWhenUnspecified = true; //version bilgisi talep edilmez ise defualt versionu ver
                opt.DefaultApiVersion = new ApiVersion(1, 0); // default versionu belirtiyoruz 1,0
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version"); //header kısmından versiyon girebilmemizi sağlar
                opt.Conventions.Controller<BooksController>()
                .HasApiVersion(new ApiVersion(1, 0));           //direkt controller'lara bu şekilde versiyon atayabiliriz
                opt.Conventions.Controller<BookV2Controller>()
                .HasDeprecatedApiVersion(new ApiVersion(2, 0));
            });
        }

        public static void ConfigureResponseCaching(this IServiceCollection services)
        {
            services.AddResponseCaching();
        }

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            //services.AddHttpCacheHeaders(); ///bu şekilde sabit olara 60 saniyelik cacheleme yapar
            
            services.AddHttpCacheHeaders(expirationOpt =>
            {
                expirationOpt.MaxAge = 90;
                expirationOpt.CacheLocation = CacheLocation.Public;
            },
            validationOpt =>
            {
                validationOpt.MustRevalidate = true;
            }
            );
        }
    }
}
