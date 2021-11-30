using GraphQLServer.Data;
using GraphQLServer.GraphQL;
using GraphQLServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using HotChocolate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GraphQLServer
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration Configuration;
        /// <summary>
        /// Конструктор Startup
        /// </summary>
        /// <param name="configuration"></param>
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        /// <summary>
        /// Dependency Injection
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region Настройка логирования
            services.AddLogging(loggingBuilder =>
            {
                //Вывод в консоль
                loggingBuilder
                      .AddConsole()
                      //выводим команды SQL
                      .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information)
                      //Выводим в окно отладки
                      .AddDebug();
            });
            #endregion

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  //Схема авторизации
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, config =>
                {
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.FromSeconds(15),
                        ValidateAudience = true,            //Разрешает валидировать Audience (Name of API resource)
                        ValidAudience = "MyAuthClient",
                        ValidateIssuer = true,              //Разрешает валидировать issuer
                        ValidIssuer = "MyAuthServer",
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("mysupersecret_secretkey!123"))
                    };
                    config.RequireHttpsMetadata = false;    //Разрешает валидировать токен пришедший по http, а не по https
                    //config.Authority = Configuration.GetValue<string>("http://localhost:4700/Token/Sram");
                    //config.Authority = "http://localhost:4700/Token/Sram";
                });

            services.AddAuthorization();

            //services.AddHttpContextAccessor();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = System.IO.Path.Combine(AppContext.BaseDirectory, "gc.db") }.ToString())));

            //контекст базы данных, новый для каждого запроса
            //services.AddTransient<AppDbContext>();

            /*
            #region Региструруем CORS
            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        //.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            #endregion
            */

            // добавление кэширования в оперативной памяти
            services.AddMemoryCache();

            //Подписки храним в оперативной памяти
            services.AddInMemorySubscriptions();
            
            //services.AddDbContext<AppDbContext>();
            services
                .AddInMemorySubscriptions()
                .AddGraphQLServer()
                //Авторизация
                .AddAuthorization()
                //Запросы
                .AddQueryType<Queries>()
                //Добавляем мутации
                .AddMutationType<Mutations>()
                //Добавляем подписки
                .AddSubscriptionType<Subscriptions>()
                //Запросы к БД имеют только нужные поля, т.е. только те которые запрашиваем в запросе (без AddProjections что то типа select * from...)
                .AddProjections()
                //Добавляем фильтрацию запросов, в запроса можно писать where
                .AddFiltering()
                //Добавляем сортировку результата запроса (Order by)
                .AddSorting()
                //Возвращаем ошибки в теле ответа на запрос
                .AddErrorFilter(er => { return er; });
            //services.AddDataLoaderRegistry();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
                        
            //Подключаем WebSocket для подписок
            app.UseWebSockets();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseGraphQLGraphiQL(); //Endpoints: https://localhost:5001/ui/graphiql  for use GraphiQL
            //app.UseGraphQLVoyager();  //Endpoints: https://localhost:5001/graphql/voyager  for use GraphiQL

            //app.UseCors("MyPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL("/graphql");
                endpoints.MapGraphQLGraphiQL("/ui/graphiql");
                endpoints.MapGraphQLVoyager("/graphql/voyager");
            });
        }
    }
}
