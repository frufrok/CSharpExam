
using Autofac;
using Autofac.Extensions.DependencyInjection;
using UserAPI.Models.Context;
using UserAPI.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text;
using UserAPI.Authentication;
using UserAPI.Models.DTO;
using UserAPI.rsa;

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
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "Token",
                    Scheme = "bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });
            });

            // �������: �������� ������ AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // �������: ��������� ������� �������� Autofac
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            // �������: ��������� ������ ����������������� �����.
            var config = new ConfigurationBuilder();
            config.AddJsonFile("appsettings.json");
            var configRoot = config.Build();

            // �������: ��������������� ���������, ��������������� �������� ���� ������.
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                containerBuilder
                    .Register(c => new UsersDbContext(configRoot.GetConnectionString("db")))
                    .InstancePerDependency();
            });

            // �������: ����������� �������, ���������������� ������.
            builder.Services.AddSingleton<ITokenSource, RSATokenSource>();
            
            // �������: ��������������� ������, ��������������� �������� �����������.
            builder.Services.AddSingleton<IUsersRepository, UsersRepository>();

            // �������: ��������������� ������ ��������������.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new RsaSecurityKey(RSATokenSource.GetPublicKey())
                    };
                });

            //builder.Services.AddScoped<IUsersRepository, UsersRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
