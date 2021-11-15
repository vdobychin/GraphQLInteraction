using GraphQLServer.Data;
using GraphQLServer.GraphQL;
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
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        private readonly IConfiguration Configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            #region Настройка логирования
            services.AddLogging(loggingBuilder =>
            {
                //Вывод в консоль
                loggingBuilder
                      .AddConsole()
                      //выводим команды SQL
                      .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);

                //Выводим в окно отладки
                loggingBuilder
                      .AddDebug();
            });
            #endregion

            //контекст базы данных, новый для каждого запроса
            services.AddTransient<AppDbContext>();

            services.AddPooledDbContextFactory<AppDbContext>(ob =>
            {
                var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = System.IO.Path.Combine(AppContext.BaseDirectory, "gc.db") }.ToString());
                //var connection = new Npgsql.NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                connection.Open();
                ob.UseSqlite(connection)
                 //ob.UseNpgsql(connection)
                //.UseSnakeCaseNamingConvention()
                .EnableSensitiveDataLogging(true);

            });

            // добавление кэширования в оперативной памяти
            services.AddMemoryCache();

            //services.AddDbContext<AppDbContext>();
            services
                .AddInMemorySubscriptions()
                .AddGraphQLServer()
                //Запросы
                .AddQueryType<Query>()
                //Запросы к БД имеют только нужные поля, т.е. только те которые запрашиваем в запросе (без AddProjections что то типа select * from...)
                .AddProjections()
                //Добавляем фильтрацию запросов, в запроса можно писать where
                .AddFiltering()
                //Добавляем сортировку результата запроса (Order by)
                .AddSorting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseGraphQLGraphiQL(); //Endpoints: https://localhost:5001/ui/graphiql  for use GraphiQL

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL("/graphql");
            });
        }
    }
}
