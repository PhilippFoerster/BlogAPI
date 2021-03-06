using BlogAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;


namespace BlogAPI
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
            services.AddControllers(options => 
            {
                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => $"Das {x} Feld ist erforderlich");
                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => $"Das {x} Feld muss numerisch sein");
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => $"Das {x} Feld darf nicht null sein");
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("de") };
                options.DefaultRequestCulture = new RequestCulture("de", "de");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            services.AddDbContext<BlogContext>(options => options.UseSqlServer(Configuration.GetConnectionString("blog")));
            services.AddScoped<UserService>();
            services.AddScoped<ArticleService>();
            services.AddScoped<CommentService>();
            services.AddScoped<TopicService>();
            
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.User.RequireUniqueEmail = true)
                .AddEntityFrameworkStores<BlogContext>()
                .AddErrorDescriber<GermanIdentityErrorDescriber>();

            services.AddCors(x => x.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
            };

            services.AddScoped(_ => tokenValidationParameters);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogAPI", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Please enter your username and password",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" }
                        }, new List<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || true)
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogAPI v1"));
            }

            app.UseRouting();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
