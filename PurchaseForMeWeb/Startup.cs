using System;
using System.IO;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using Akka.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PurchaseForMe.Core;
using PurchaseForMe.Core.Configuration;
using PurchaseForMe.Core.User;
using PurchaseForMeWeb.Actors;
using PurchaseForMeWeb.Data;
using PurchaseForMeWeb.Hubs;
using PurchaseForMeWeb.Project;

namespace PurchaseForMeWeb
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
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CLUSTER_DOCKER")))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection")));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnectionDocker")));
            }
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDefaultIdentity<PurchaseForMeUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddSignalR();
            services.AddHttpContextAccessor();
            services.AddSingleton<ActorSystem>(provider =>
            {
                Config configuration = ConfigurationFactory.ParseString(File.ReadAllText("akkaconf-web.txt"));
                string seedNodes = Environment.GetEnvironmentVariable("CLUSTER_SEED_NODES");
                string port = Environment.GetEnvironmentVariable("CLUSTER_PORT");
                string hostname = Environment.GetEnvironmentVariable("CLUSTER_HOSTNAME");
                string roles = Environment.GetEnvironmentVariable("CLUSTER_ROLES");
                if (!string.IsNullOrEmpty(seedNodes))
                {
                    configuration = configuration.WithFallback(
                        ConfigurationFactory.ParseString($"akka.cluster.seed-nodes = [{seedNodes}]"));
                }
                if (!string.IsNullOrEmpty(port))
                {
                    configuration = configuration.WithFallback(
                        ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port = {port}"));
                }
                if (!string.IsNullOrEmpty(hostname))
                {
                    configuration = configuration.WithFallback(
                        ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.hostname = {hostname}"));
                }
                if (!string.IsNullOrEmpty(roles))
                {
                    configuration = configuration.WithFallback(
                        ConfigurationFactory.ParseString($"akka.cluster.roles = [{roles}]"));
                }

                var actorSystem =  ActorSystem.Create("purchaseForMe", configuration);
                return actorSystem;
            });

            services.AddSingleton<PipelineSchedulingBusFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                ClusterRouterGroupSettings groupSettings =
                    new ClusterRouterGroupSettings(1000, new[] {"/user/pipelineSchedulingBus"}, false, "Pipeline");
                var pipelineSchedulingBusRouter =
                    actorSystem.ActorOf(Props.Empty.WithRouter(new ClusterRouterGroup(new RoundRobinGroup(), groupSettings)), "pipelineSchedulingBusRouter");
                return () => pipelineSchedulingBusRouter;
            });
            services.AddSingleton<TaskSchedulingBusFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                ClusterRouterGroupSettings groupSettings =
                    new ClusterRouterGroupSettings(1000, new[] { "/user/taskSchedulingBus" }, false, "Task");
                var taskSchedulingBusRouter =
                    actorSystem.ActorOf(Props.Empty.WithRouter(new ClusterRouterGroup(new RoundRobinGroup(), groupSettings)), "taskSchedulingBusRouter");
                return () => taskSchedulingBusRouter;
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
            services.AddSingleton<CodeMonitorSignalRFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                var hubContext = provider.GetRequiredService<IHubContext<CodeMonitorHub>>();
                var taskSchedulingBus = provider.GetRequiredService<TaskSchedulingBusFactory>();
                var pipelineSchedulingBus = provider.GetRequiredService<PipelineSchedulingBusFactory>();
                var projectManager = provider.GetRequiredService<ProjectManagerFactory>();
                IActorRef codeMonitorSignalR = actorSystem.ActorOf(Props.Create(() =>
                    new CodeMonitorSignalR(hubContext, taskSchedulingBus, pipelineSchedulingBus, projectManager, actorSystem)));
                return () => codeMonitorSignalR;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, ApplicationDbContext context)
        {
            context.Database.Migrate();
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
                endpoints.MapHub<CodeMonitorHub>("/codeMonitoring");
            });

            lifetime.ApplicationStarted.Register(() => app.ApplicationServices.GetRequiredService<ActorSystem>());
            lifetime.ApplicationStopping.Register(() =>
                app.ApplicationServices.GetRequiredService<ActorSystem>().Terminate().GetAwaiter().GetResult());
        }
    }
}