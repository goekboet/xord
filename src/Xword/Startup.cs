using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xword.Middleware;
using Xword.Services;
using Serilog;
using Serilog.Sinks.RollingFile;
using System;

namespace Xword
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfigurationRoot Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<Models.MatchingOptions>(Configuration.GetSection("MatchingOptions"));

            services.AddScoped<IWordList, FileWordlist>();
            services.AddScoped<ISuggester, WordListSuggester>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .WithFilter(new FilterLoggerSettings
                {
                    {"Microsoft", LogLevel.Warning },
                    {"System", LogLevel.Warning},
                    {"RequestLogger", LogLevel.Information},
                    {"SuggestController", LogLevel.Warning}
                })
                .AddSerilog(
                new LoggerConfiguration()
                    .WriteTo.RollingFile(Environment.GetEnvironmentVariable("XORD_LOG"))
                    .CreateLogger());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseMiddleware<RequestLogger>();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
