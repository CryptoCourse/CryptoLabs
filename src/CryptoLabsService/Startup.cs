using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using CryptoLabsService.Labs.PaddingOracle;
using CryptoLabsService.Labs.StreamCipherIntegrity;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using CryptoLabsService.Labs.CbcIvIsKey;
using CryptoLabsService.Labs.CbcIvIsTime;
using CryptoLabsService.Labs.CmcMacFixedKey;
using CryptoLabsService.Labs.EncryptionModeOracle;

namespace CryptoLabsService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                // ignore nulls in json
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddScoped<BlockCipherOracleManager>();
            services.AddScoped<StreamCipherIntegrityManager>();
            services.AddScoped<CbcIvIsTimeManager>();
            services.AddScoped<CbcIvIsKeyManager>();
            services.AddScoped<CbcMacManager>();
            services.AddScoped<PaddingOracleManger>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CryptoLabs API", Version = "v3" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
