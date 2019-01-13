using FrameDemo.Core.Interfaces;
using FrameDemo.Infrastructure.Database;
using FrameDemo.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrameDemo.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using FrameDemo.Infrastructure.Resources;
using Newtonsoft.Json.Serialization;
using FluentValidation.AspNetCore;
using FrameDemo.Api.Helpers;
using FrameDemo.Core.Entities;
using FrameDemo.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;

namespace FrameDemo.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.ReturnHttpNotAcceptable = true;

                    var intputFormatter = options.InputFormatters.OfType<JsonInputFormatter>().FirstOrDefault();
                    if (intputFormatter != null)
                    {
                        intputFormatter.SupportedMediaTypes.Add("application/vnd.naoh.hateoas.create+json");
                        intputFormatter.SupportedMediaTypes.Add("application/vnd.naoh.hateoas.update+json");
                    }

                    var outputFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                    outputFormatter?.SupportedMediaTypes.Add("application/vnd.naoh.hateoas+json");

                }).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddFluentValidation();

            services.AddDbContext<MyContext>(
                options =>
                {
                    options.UseSqlite(Configuration.GetConnectionString("SqliteConnection"));
                });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 5001;
            });

            //services.AddScoped<ISampleRepository, SampleRepository>();
            services.AddScoped<IRepository<Sample, SampleParameters>, SampleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //注册需要创建uri的服务
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            //注册资源映射关系 MappingProfile
            services.AddAutoMapper();
            //校验资源
            services.AddTransient<IValidator<SampleAddResource>, SampleAddOrUpdateResourceValidator<SampleAddResource>>();
            services.AddTransient<IValidator<SampleUpdateResource>, SampleAddOrUpdateResourceValidator<SampleUpdateResource>>();

            //注册排序的映射关系 QueryParameters.OrderBy
            var propertyMappingContainer = new PropertyMappingContainer();
            propertyMappingContainer.Register<SamplePropertyMapping>();
            services.AddSingleton<IPropertyMappingContainer>(propertyMappingContainer);

            //注册塑形的映射关系 Fields
            services.AddTransient<ITypeHelperService, TypeHelperService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMyExceptionHandler(loggerFactory);
                app.UseHsts();
            }

            app.UseStatusCodeHandling();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
