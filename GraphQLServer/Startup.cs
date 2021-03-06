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
        /// ??????????? Startup
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
            #region ????????? ???????????
            services.AddLogging(loggingBuilder =>
            {
                //????? ? ???????
                loggingBuilder
                      .AddConsole()
                      //??????? ??????? SQL
                      .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information)
                      //??????? ? ???? ???????
                      .AddDebug();
            });
            #endregion

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  //????? ???????????
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, config =>
                {
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.FromSeconds(15),
                        ValidateAudience = true,            //????????? ???????????? Audience (Name of API resource)
                        ValidAudience = "MyAuthClient",
                        ValidateIssuer = true,              //????????? ???????????? issuer
                        ValidIssuer = "MyAuthServer",
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("mysupersecret_secretkey!123"))
                    };
                    config.RequireHttpsMetadata = false;    //????????? ???????????? ????? ????????? ?? http, ? ?? ?? https
                    //config.Authority = Configuration.GetValue<string>("http://localhost:4700/Token/Sram");
                    //config.Authority = "http://localhost:4700/Token/Sram";
                });

            services.AddAuthorization();

            //services.AddHttpContextAccessor();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = System.IO.Path.Combine(AppContext.BaseDirectory, "gc.db") }.ToString())));

            //???????? ???? ??????, ????? ??? ??????? ???????
            //services.AddTransient<AppDbContext>();

            /*
            #region ???????????? CORS
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

            // ?????????? ??????????? ? ??????????? ??????
            services.AddMemoryCache();

            //???????? ?????? ? ??????????? ??????
            services.AddInMemorySubscriptions();
            
            //services.AddDbContext<AppDbContext>();
            services
                .AddInMemorySubscriptions()
                .AddGraphQLServer()
                //???????????
                .AddAuthorization()
                //???????
                .AddQueryType<Queries>()
                //????????? ???????
                .AddMutationType<Mutations>()
                //????????? ????????
                .AddSubscriptionType<Subscriptions>()
                //??????? ? ?? ????? ?????? ?????? ????, ?.?. ?????? ?? ??????? ??????????? ? ??????? (??? AddProjections ??? ?? ???? select * from...)
                .AddProjections()
                //????????? ?????????? ????????, ? ??????? ????? ?????? where
                .AddFiltering()
                //????????? ?????????? ?????????? ??????? (Order by)
                .AddSorting()
                //?????????? ?????? ? ???? ?????? ?? ??????
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
                        
            //?????????? WebSocket ??? ????????
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
