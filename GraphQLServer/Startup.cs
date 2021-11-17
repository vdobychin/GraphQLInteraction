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

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = System.IO.Path.Combine(AppContext.BaseDirectory, "gc.db") }.ToString())));

            //контекст базы данных, новый для каждого запроса
            //services.AddTransient<AppDbContext>();
            
            // добавление кэширования в оперативной памяти
            services.AddMemoryCache();

            //Подписки храним в оперативной памяти
            services.AddInMemorySubscriptions();

            //services.AddDbContext<AppDbContext>();
            services
                .AddInMemorySubscriptions()
                .AddGraphQLServer()
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
            
            //app.UseGraphQLGraphiQL(); //Endpoints: https://localhost:5001/ui/graphiql  for use GraphiQL
            //app.UseGraphQLVoyager();  //Endpoints: https://localhost:5001/graphql/voyager  for use GraphiQL

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL("/graphql");
                endpoints.MapGraphQLGraphiQL("/ui/graphiql");
                endpoints.MapGraphQLVoyager("/graphql/voyager");
            });
        }
    }
}
