using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameDemo.Api.Messages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FrameDemo.Api.Extensions
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        public IConfiguration Configuration { get; }

        public ConfigureJwtBearerOptions(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(string.Empty, options);
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name == JwtBearerDefaults.AuthenticationScheme)
            {
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Query["access_token"];
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        string message = context.Exception.Message;
                        if (message.Contains("IDX12723") || message.Contains("IDX12729") ||
                            message.Contains("IDX10503"))
                        {
                            context.HttpContext.Response.Headers.Add("X-Error",
                                ErrorCodeStatus.ErrorCode40009.ToString());
                            return Task.CompletedTask;
                        }

                        if (message.Contains("IDX10223"))
                        {
                            context.HttpContext.Response.Headers.Add("X-Error",
                                ErrorCodeStatus.ErrorCode40010.ToString());
                            return Task.CompletedTask;
                        }
                        return Task.CompletedTask;
                    }
                };

                var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:ServerSecret"]));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = serverSecret
                };
            }
        }
    }
}