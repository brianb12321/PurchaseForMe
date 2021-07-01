using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PurchaseForMe.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PurchaseForMe.Actors.Project;
using PurchaseForMe.Actors.User;
using PurchaseForMe.Actors.WebPipeline;
using PurchaseForMe.Configuration;
using PurchaseForMe.Core.User;
using PurchaseForMe.Core.WebPipeline;
using PurchaseForMe.Hubs;

namespace PurchaseForMe
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDefaultIdentity<PurchaseForMeUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddSignalR();
            services.AddHttpContextAccessor();
            services.AddSingleton<ActorSystem>(provider => ActorSystem.Create("purchaseForMe", ConfigurationFactory.ParseString(@"
akka {  
    stdout-loglevel = DEBUG
    loglevel = DEBUG
    log-config-on-start = on        
    actor {                
        debug {  
              receive = on 
              autoreceive = on
              lifecycle = on
              event-stream = on
              unhandled = on
        }
    }  
")));
            
            services.AddSingleton<WebPipelineActorFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                IActorRef pipeline = actorSystem.ActorOf(Props.Create<WebPipelineActor>(), "pipeline");
                return () => pipeline;
            });
            services.AddSingleton<WebPipelineSignalRActorFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                var hubContext = provider.GetRequiredService<IHubContext<PipelineRunnerHub>>();
                var pipelineActorFactory = provider.GetRequiredService<WebPipelineActorFactory>();
                IActorRef signalR =
                    actorSystem.ActorOf(Props.Create<WebPipelineSignalRActor>(hubContext, pipelineActorFactory), "signalR");
                return () => signalR;
            });
            services.AddSingleton<UserManagerActorFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                var scopedProvider = provider.CreateScope().ServiceProvider;
                IActorRef userManager = actorSystem.ActorOf(Props.Create(() =>
                    new UserManagerActor(scopedProvider.GetService<UserManager<PurchaseForMeUser>>(),
                        scopedProvider.GetService<IHttpContextAccessor>())), "userManager");
                return () => userManager;
            });
            services.Configure<ProjectSettings>(Configuration.GetSection("Project"));
            services.AddSingleton<ProjectManagerFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                IActorRef projectManager = actorSystem.ActorOf(Props.Create(() => new ProjectManager(
                    provider.GetService<IOptions<ProjectSettings>>(),
                    provider.GetService<UserManagerActorFactory>())), "projectManager");
                return () => projectManager;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<PipelineRunnerHub>("/pipelineRunner");
            });

            lifetime.ApplicationStarted.Register(() => app.ApplicationServices.GetRequiredService<ActorSystem>());
            lifetime.ApplicationStopping.Register(() =>
                app.ApplicationServices.GetRequiredService<ActorSystem>().Terminate().GetAwaiter().GetResult());
        }
    }
}
