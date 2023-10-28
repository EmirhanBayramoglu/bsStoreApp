using Repositories.EFCore;
using WebApi.Extensions;
using Microsoft.EntityFrameworkCore;
using NLog;
using Presentation;
using Services.Contract;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;

var builder = WebApplication.CreateBuilder(args);

//string.concat iki ifadenin string olarak birleştirilmesini sağlar
LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(),"/nlog.config"));

// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
})
    .AddXmlDataContractSerializerFormatters()
    .AddApplicationPart(typeof(AssemblyReference).Assembly)
    .AddNewtonsoftJson();

builder.Services.AddScoped<ValidationFilterAttribute>(); //IoC

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;

});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureActionFilter();
builder.Services.ConfigureCors();
builder.Services.ConfigureDataShapepr();


builder.Services.AddControllers().AddNewtonsoftJson();


var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHsts();

}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
