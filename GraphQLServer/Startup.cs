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

            /*
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

            */
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

            //���������� WebSocket ��� ��������
            app.UseWebSockets();
            
            app.UseGraphQLGraphiQL(); //Endpoints: https://localhost:5001/ui/graphiql  for use GraphiQL

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL("/graphql");
                endpoints.MapGraphQLGraphiQL("/ui/graphiql");
            });
        }
    }
}
