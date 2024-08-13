
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CSharpExamUserAPI.Models.Context;
using CSharpExamUserAPI.Repository;

namespace CSharpExamUserAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Решение: Добавлен сервис AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Решение: Добавлена фабрика сервисов Autofac
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            // Решение: Добавлено чтение конфигурационного файла.
            var config = new ConfigurationBuilder();
            config.AddJsonFile("appsettings.json");
            var configRoot = config.Build();

            // Решение: Зарегистрирован компонент, предоставляющий контекст базы данных.
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                containerBuilder
                    .Register(c => new UsersDbContext(configRoot.GetConnectionString("db")))
                    .InstancePerDependency();
            });

            // Добавлен сервис, предоставляющий синглтон репозитория.
            builder.Services.AddSingleton<IUsersRepository, UsersRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
