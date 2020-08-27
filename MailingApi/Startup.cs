using System;
using System.Linq;
using MailingApi.DataAccess.Mappers;
using MailingApi.DataAccess.Repositories;
using MailingApi.Domain.Models;
using MailingApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace MailingApi
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
            services.AddControllers();
            services.AddDbContext<DomainContext>(options => 
                options.UseInMemoryDatabase("MailingApiDatabase"), ServiceLifetime.Transient);
            services.AddScoped<Func<DomainContext>>(serviceProvider => serviceProvider.GetRequiredService<DomainContext>);
            services.AddTransient<IEmailsRepository, EmailsRepository>();
            services.AddTransient<IEmailsMapper, EmailsMapper>();
            services.AddTransient<IEmailSenderService>(sp => new EmailSenderService(
                Configuration["Smtp:Hostname"], 
                int.Parse(Configuration["Smtp:Port"]), 
                Configuration["Smtp:Username"], 
                Configuration["Smtp:Password"],
                bool.Parse(Configuration["Smtp:EnableSsl"])));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mailing API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mailing API V1");
            });
        }
    }
}
