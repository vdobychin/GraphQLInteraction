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
        /// ����������� Startup
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
            #region ��������� �����������
            services.AddLogging(loggingBuilder =>
            {
                //����� � �������
                loggingBuilder
                      .AddConsole()
                      //������� ������� SQL
                      .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information)
                      //������� � ���� �������
                      .AddDebug();
            });
            #endregion

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = System.IO.Path.Combine(AppContext.BaseDirectory, "gc.db") }.ToString())));

            //�������� ���� ������, ����� ��� ������� �������
            //services.AddTransient<AppDbContext>();
            
            // ���������� ����������� � ����������� ������
            services.AddMemoryCache();

            //�������� ������ � ����������� ������
            services.AddInMemorySubscriptions();

            //services.AddDbContext<AppDbContext>();
            services
                .AddInMemorySubscriptions()
                .AddGraphQLServer()
                //�������
                .AddQueryType<Queries>()
                //��������� �������
                .AddMutationType<Mutations>()
                //��������� ��������
                .AddSubscriptionType<Subscriptions>()
                //������� � �� ����� ������ ������ ����, �.�. ������ �� ������� ����������� � ������� (��� AddProjections ��� �� ���� select * from...)
                .AddProjections()
                //��������� ���������� ��������, � ������� ����� ������ where
                .AddFiltering()
                //��������� ���������� ���������� ������� (Order by)
                .AddSorting()
                //���������� ������ � ���� ������ �� ������
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

            //���������� WebSocket ��� ��������
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
