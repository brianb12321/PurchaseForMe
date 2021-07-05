using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PurchaseForMe.Data;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PurchaseForMe.Actors.Project;
using PurchaseForMe.Actors.TaskSystem;
using PurchaseForMe.Actors.User;
using PurchaseForMe.Actors.WebPipeline;
using PurchaseForMe.Configuration;
using PurchaseForMe.Core.User;
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
            
            services.AddSingleton<PipelineSchedulingBusFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                ILogger<PipelineSchedulingBus> logger = provider.GetService<ILogger<PipelineSchedulingBus>>();
                IActorRef pipelineBus = actorSystem.ActorOf(Props.Create(() => new PipelineSchedulingBus(logger)), "pipelineSchedulingBus");
                return () => pipelineBus;
            });
            services.AddSingleton<TaskSchedulingBusFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                ILogger<TaskSchedulingBus> logger = provider.GetService<ILogger<TaskSchedulingBus>>();
                PipelineSchedulingBusFactory pipelineBus = provider.GetRequiredService<PipelineSchedulingBusFactory>();
                IActorRef taskBus = actorSystem.ActorOf(Props.Create(() => new TaskSchedulingBus(logger, pipelineBus)),
                    "taskSchedulingBus");
                return () => taskBus;
            });
            services.AddSingleton<WebPipelineSignalRActorFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                var hubContext = provider.GetRequiredService<IHubContext<PipelineRunnerHub>>();
                var pipelineSchedulingBus = provider.GetRequiredService<PipelineSchedulingBusFactory>();
                IActorRef signalR =
                    actorSystem.ActorOf(Props.Create<WebPipelineSignalRActor>(hubContext, pipelineSchedulingBus), "webPipelineSignalR");
                return () => signalR;
            });
            services.AddSingleton<TaskSchedulingSignalRFactory>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                var hubContext = provider.GetRequiredService<IHubContext<TaskRunnerHub>>();
                var taskSchedulingBus = provider.GetRequiredService<TaskSchedulingBusFactory>();
                var projectManager = provider.GetRequiredService<ProjectManagerFactory>();
                IActorRef signalR =
                    actorSystem.ActorOf(Props.Create(() => new TaskSchedulingSignalR(hubContext, taskSchedulingBus, projectManager)), "taskRunnerSignalR");
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
                endpoints.MapHub<TaskRunnerHub>("/taskRunner");
            });

            lifetime.ApplicationStarted.Register(() => app.ApplicationServices.GetRequiredService<ActorSystem>());
            lifetime.ApplicationStopping.Register(() =>
                app.ApplicationServices.GetRequiredService<ActorSystem>().Terminate().GetAwaiter().GetResult());
        }
    }
}