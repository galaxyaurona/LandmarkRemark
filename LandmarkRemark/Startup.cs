using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using System;
using LandmarkRemark.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using LandmarkRemark.Services;
using LandmarkRemark.Repositories;

namespace LandmarkRemark
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // helper method to set up context options
        private void SetupContextOptions(DbContextOptionsBuilder options, string connectionString, bool useSQLite = false)
        {
            if (useSQLite)
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseSqlServer(connectionString);
            }

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            var useSQLite = isOSX || Configuration.GetValue("UseSQLite", false);

            // get connection string based on environment config and platform
            var connectionStringKey = useSQLite ? "LandmarkRemarkContextSQLite" : "LandmarkRemarkContext";
            var connectionString = Configuration.GetConnectionString(connectionStringKey);

            // add db context
            services.AddDbContext<LandmarkRemarkContext>(options => SetupContextOptions(options, connectionString, useSQLite));

            // dependency injection here
            //because EF dbContext default = scoped, so need to match the DI graph

            // repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
            //service
            services.AddScoped<IUserService, UserService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // global Exception catcher and return 500 with error message array
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    var response = new ErrorResponse();
                    response.Errors.Add(exceptionHandlerPathFeature?.Error.Message);
                    var responseString = JsonConvert.SerializeObject(response);
                    await context.Response.WriteAsync(responseString);
                });
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");

            });

            // return 404 not found for api, only redirect endpoint not goign to API
            app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase), builder =>
            {
               builder.UseSpa(spa =>
               {
                   spa.Options.SourcePath = "ClientApp";

                   if (env.IsDevelopment())
                   {
                       spa.UseReactDevelopmentServer(npmScript: "start");
                   }
               });
            });

        }
    }
}
