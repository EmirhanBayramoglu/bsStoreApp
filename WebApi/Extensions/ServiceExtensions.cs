using Microsoft.EntityFrameworkCore;
using Presentation.ActionFilters;
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
            }

    }
}
