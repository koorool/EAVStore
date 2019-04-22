using EAVStore.Api.Services;
using EAVStore.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace EAVStore.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddDbServices(Configuration);

            services.AddScoped<IClinicService, ClinicService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(
                c => c.SwaggerDoc(
                    "v1",
                    new Info {
                        Version = "v1",
                        Title = "EAV Store"
                    }
                )
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            using (var scope = app.ApplicationServices.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<EavStoreDbContext>();

                dbContext.Database.Migrate();
            }

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });
        }
    }
}