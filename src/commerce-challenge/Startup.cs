using System;
using commerce_challenge.Core.Http;
using commerce_challenge.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace commerce_challenge
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // UseHsts excludes the following loopback hosts: localhost, 127.0.0.1, [::1]
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseCors();

            app.UseHealthChecks("/health-api",
                new HealthCheckOptions
                {
                    Predicate = _ => true
                });

            app.UseStaticFiles();
            app.UseEndpoints(endpoint => endpoint.MapControllers());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkInMemoryDatabase();
            services.AddDbContextFactory<CommerceDbContext>(opt =>
            {
                opt.EnableDetailedErrors();
                opt.EnableSensitiveDataLogging();
            });

            services.AddDistributedMemoryCache();
            services.AddLogging();

            services.AddHealthChecks();

            services.AddTransient<ProductImportService>();
            services.AddHostedService<ProductImportService>();

            services.AddControllers()
                .AddJsonOptions(options => { });

            services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(builder => builder.AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader());
            });

            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(120); // defaults to 30 days
            });

            services.AddSingleton<AuthenticationHandler>();
            services.AddHttpClient("commerce")
                .AddHttpMessageHandler<AuthenticationHandler>();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
            });
        }
    }
}
