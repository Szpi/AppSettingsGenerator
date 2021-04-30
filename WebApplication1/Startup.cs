using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
//using Configuration.Generated;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //var test = new AppSettings();
            //ThisAssembly.Strings.aaa;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //services.Configure<Testooooo>(Configuration.GetSection("Testooooo"));
            //services.Configure<AppSettings>(Configuration);
            services.Configure<WebApplication1.MySettings>(Configuration.GetSection("My.Settings"));
            //services.ConfigureTestooooo(Configuration);
            //services.ConfigureAllSections(Configuration);
            //services.ConfigureTestooooo(Configuration);
            //services.ConfigureAllSections(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var a = Configuration.GetValue<string>("My.Settings:My.Test");
            //var test = app.ApplicationServices.GetService<IOptions<AppSettings>>();
           // var test1 = app.ApplicationServices.GetService<IOptions<MySettings>>();
            //var sdas = test1.Value.MyTest;
            //var sdas1 = sdas.my;
            //var b = test.Value.GetTestEnum();
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
        }
    }
}
