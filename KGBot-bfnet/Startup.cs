using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KudaBot;
using KudaBot.KGBot;
using KudaBot.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KudaBot_bfnet
{
    public class Startup
    {
        
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => Configuration);
            services.AddBot<KGBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);
                options.Middleware.Add(new ShowTypingMiddleware());
                options.Middleware.Add(new UserState<KGBState>(new MemoryStorage()));
                options.Middleware.Add(new NormalizerMiddleware());
                options.Middleware.Add(new UtteranceQueueMiddleware<KGBState>(5));
                options.Middleware.Add(new KeywordRecognizerMiddleware(new KGBActions()));
                options.Middleware.Add(new DeepPavlovMiddleware("http://lnsigo.mipt.ru:6063/kudago"));
                // options.Middleware.Add(new DialogFlowMiddleware("696d3e14d2f44abcaf4d957d13e537a2", new KGBActions()));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseBotFramework();
        }
    }
}
