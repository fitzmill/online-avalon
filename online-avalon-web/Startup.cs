using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using online_avalon_web.Accessors;
using online_avalon_web.Core;
using online_avalon_web.Core.Interfaces.Accessors;
using online_avalon_web.Core.Interfaces.Engines;
using online_avalon_web.Core.Interfaces.Workers;
using online_avalon_web.Engines;
using online_avalon_web.Hubs;
using online_avalon_web.Workers;

namespace online_avalon_web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSpaStaticFiles(options => options.RootPath = "client-app/dist");

            services.AddDbContext<AvalonContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("avalon")));

            // Accessors
            services.AddTransient<IPlayerAccessor, PlayerAccessor>();
            services.AddTransient<IGameAccessor, GameAccessor>();
            services.AddTransient<IQuestAccessor, QuestAccessor>();

            // Engines
            services.AddTransient<IPlayerEngine, PlayerEngine>();
            services.AddTransient<IGameEngine, GameEngine>();

            // SignalR
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
            })
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            // Workers
            services.AddSingleton<IGameCleanupQueue, GameCleanupQueue>();
            services.AddHostedService<GameCleanupWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/hubs/game");
            });

            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client-app";
                if (env.IsDevelopment())
                {

                    spa.UseVueDevelopmentServer();
                }
            });
        }
    }
}
