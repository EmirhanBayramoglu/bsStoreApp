using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contract;

namespace WebApi.Controllers.Extensions
{
    //Service oluşturma klasörü
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

    }
}
