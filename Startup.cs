using DataFlowExample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFlowExample
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
            services.AddControllersWithViews();

            // Setup the DataFlow
            
            // CSV File Setup
            if (!File.Exists("entries.csv"))
            {
                // If it doesn't exist, write the header line
                File.AppendAllLines("entries.csv", new[] { $"Email,Answer,IPAddress,Created(UTC)" });
            }

            // The Consumer Block
            var fileWriter = new ActionBlock<CompetitionEntry>((entry) =>
            {
                // When the object is received, put the new csv line together
                string newCsvLine = $"{entry.Email},{entry.Answer},{entry.IPAddress},{entry.Created:s}";
                File.AppendAllLines("entries.csv", new[] { newCsvLine });
            });

            // A simple buffer, link this to the consumer above to create a "pipeline"
            var producer = new BufferBlock<CompetitionEntry>();
            producer.LinkTo(fileWriter);

            // Allow the controller to have the buffer injected
            services.AddSingleton(producer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
