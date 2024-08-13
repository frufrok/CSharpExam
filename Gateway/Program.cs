using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.OpenApi.Models;

namespace Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Решение: Добавляем сервисы, необходимые для SwaggerForOcelot
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddMvc();
            builder.Services.AddSwaggerGen();

            // Решение: Конфигурируем Ocelot
            var config = new ConfigurationBuilder()
                .AddJsonFile("ocelot.json")
                .Build();
            builder.Services.AddOcelot(config);

            // Решение: Добавляем сервис SwaggerForOcelot
            builder.Services.AddSwaggerForOcelot(config);
            
            var app = builder.Build();

            // Решение: Регистрируем Swagger MiddleWare 
            app.UseSwagger();

            // Решение: Регистрируем Ocelot MiddleWare
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseOcelot().Wait();

            //  Решение: Регистрируем Https Redirection Middleware
            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
