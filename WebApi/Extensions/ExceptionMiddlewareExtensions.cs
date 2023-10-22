using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Services.Contract;
using System.Net;

namespace WebApi.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app, ILoggerService logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {

                    //hatayı gösterme türümüz
                    context.Response.ContentType = "application/json";

                    //hatanın türünü bul
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    //hata boş değilse(hata var ise) aşağıdaki if içerisine gir
                    if (contextFeature != null)
                    {
                        //alt kısımda error kodunu seçmesini sağlıyo hata türüne göre 
                        //NotFoundException ise 404 döndür diğerleri ise defualt 500 döndür
                        //her virgülden sonra başka bir seçenek olur
                        context.Response.StatusCode = contextFeature.Error switch
                        {
                            NotFoundException => StatusCodes.Status404NotFound,
                            _ => StatusCodes.Status500InternalServerError
                        };

                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetails { 
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message
                        }.ToString());
                    }

                });
            });
        }
    }
}
