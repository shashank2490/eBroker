using eBroker.Contracts;
using eBroker.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Repository;
using Repository.Entities;
using System;
using Microsoft.EntityFrameworkCore;

namespace eBroker
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
            services.AddDbContext<EBrokerDBContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DBConnectionString"), b => b.MigrationsAssembly("eBroker"));
            }, ServiceLifetime.Transient);

            services.AddSingleton<ISystemClock, SystemClock>();
            services.AddScoped<IGenericRepository<Account>, GenericRepository<Account>>();
            services.AddScoped<IGenericRepository<EquityHolding>, GenericRepository<EquityHolding>>();
            services.AddScoped<IGenericRepository<EquityTransaction>, GenericRepository<EquityTransaction>>();
            services.AddScoped<IFundService, FundService>();
            services.AddScoped<IEquityService, EquityService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "eBroker", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "eBroker v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
