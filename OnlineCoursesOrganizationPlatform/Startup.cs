using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using OnlineCoursesOrganizationPlatform.Models;
using Microsoft.OpenApi.Models;
using OnlineCoursesOrganizationPlatform.Services;
using System.Reflection;
using System;
using System.IO;

namespace OnlineCoursesOrganizationPlatform
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(
                        new Version(8, 0, 28)
                        )
                    )
                );

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Платформа организации курсов",
                    Description = "Позволяет создать или изучить курс",
                    Contact = new OpenApiContact
                    {
                        Name = "Контакт:",
                        Url = new Uri("https://github.com/Sanik10/OnlineCoursesOrganizationPlatform")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Пример лицензии",
                        Url = new Uri("https://example.com/license")
                    }
                });

                c.IncludeXmlComments(xmlPath); // Добавляем XML комментарии в SwaggerGen
            });

            services.AddScoped<IActionService, ActionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<CourseService>();
            services.AddScoped<CourseMaterialService>();
            services.AddScoped<IJwtService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var secretKey = configuration["Jwt:SecretKey"];
                return new JwtService(secretKey);
            });
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<CourseRatingsService>();
            services.AddScoped<UserProgressService>();
            // Добавьте другие сервисы, если необходимо
        }

        private void CheckDatabaseConnection(IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                try
                {
                    context.Database.OpenConnection();
                    context.Database.CloseConnection();
                    Console.WriteLine("Database connection successful!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error connecting to the database: " + ex.Message);
                }
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger(); // Добавление Swagger в конвейер обработки запросов

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Online courses organization platform V1");
            }); // Настройка Swagger UI

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}