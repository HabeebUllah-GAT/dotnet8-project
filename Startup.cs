using NLog.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using GATIntegrations.Resources;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Routing.Internal;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Razor;
using GATIntegrations.Data;

// using GAT.Controllers;
// using GAT.Data;
// using GAT.Domain;
// using GAT.Domain.Implementation;
// using GAT.Middleware;
// using GAT.Services;
// using GAT.Services.Implementation;
// using GAT.Integrations.Validations.Implementation;
// using GAT.Integrations.Services;
// using GAT.Integrations.Domain;
// using GAT.Integrations.Services.Implementation;
// using GAT.Integrations.Validations;
// using GAT.Integrations.Domain.Implementation;
// using GAT.Validations;
// using GAT.Validations.Implementation;
// using GAT.Settings.Services;
// using GAT.Settings.Services.Implementation;
// using GAT.Settings.Repository;
// using GAT.Settings.Repository.Implementation;
// using GAT.Settings.Helpers;

namespace GATIntegrations
{
    public class Startup
    {
        //public Startup(IConfiguration configuration, ILogger logger)
        //{
        //    //Configuration = configuration;        
        //}

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<CommonLocalization>();

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            services.AddMvc()
              .AddViewLocalization()
              .AddDataAnnotationsLocalization(options =>
              {
                  options.DataAnnotationLocalizerProvider = (type, factory) =>
                  {
                      var assemblyName = new AssemblyName(typeof(Resource).GetTypeInfo().Assembly.FullName);
                      return factory.Create(nameof(Resource), assemblyName.Name);
                  };
              });

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US")
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                //options.RequestCultureProviders = new[] { new RouteDataRequestCultureProvider { IndexOfCulture = 1, IndexofUICulture = 1 } };
            });

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ContentRootPath = System.IO.Directory.GetCurrentDirectory()
            });

            services.Configure<ApiBehaviorOptions>(obj =>
            {
                obj.InvalidModelStateResponseFactory = actionContext =>
                {
                    return new BadRequestObjectResult(new
                    {
                        Code = 400,
                        Message = string.Join(',', actionContext.ModelState.Values.SelectMany(x => x.Errors)
                            .Select(x => x.ErrorMessage))
                    });
                };
            });

            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // configure strongly typed settings object
            //services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            // services.AddScoped<IUserService, UserService>();

            //if clients need output as xml then uncomment below line
            //services.AddMvc().AddMvcOptions(o => { o.OutputFormatters.Add(new Microsoft.AspNetCore.Mvc.Formatters.XmlDataContractSerializerOutputFormatter()); });

            services.AddDbContext<GigAndTakeDbContext>();

            services.AddControllers();

            services.AddApiVersioning(opt =>
                                    {
                                        opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                                        opt.AssumeDefaultVersionWhenUnspecified = true;
                                        opt.ReportApiVersions = true;
                                        opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                                                        new HeaderApiVersionReader("x-api-version"),
                                                                                        new MediaTypeApiVersionReader("x-api-version"));
                                    });

            builder.Services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GATIntegrations", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GAT v1"));
                app.UseDeveloperExceptionPage();
            }

            //app.UseStatusCodePages(); //display status code as text            
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService < IOptions < RequestLocalizationOptions >> ().Value);

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            // custom jwt auth middleware
            // app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(cfg =>
            {
                cfg.MapControllers();
            });
        }
    }
}
