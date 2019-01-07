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
using FrameDemo.Api.Models;
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
            services.AddMvc().AddJsonOptions(options =>
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

            services.AddScoped<ISampleRepository, SampleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddAutoMapper();

            services.AddTransient<IValidator<SampleAddResource>, SampleAddOrUpdateResourceValidator<SampleAddResource>>();
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

            app.UseStatusCodePages(context =>
            {
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    var message = JsonConvert.SerializeObject(new ReturnMessage()
                    {
                        Code = StatusCodes.Status404NotFound,
                        Msg = "控制器或是方法错误",
                        ErrorCode = ErrorCodeStatus.ErrorCode40000
                    }, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                    return context.HttpContext.Response.WriteAsync(message);
                }
                throw new Exception("Error, status code: " + context.HttpContext.Response.StatusCode);
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
