using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HorseSpot.Web
{
  public class Startup
  {

    public static void Main(string[] args)
    {
      var host = new WebHostBuilder()
          .UseKestrel()
          .UseContentRoot(Directory.GetCurrentDirectory())
          .UseIISIntegration()
          .UseStartup<Startup>()
          .Build();

      host.Run();
    }
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
          .AddEnvironmentVariables();
      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // Add framework services.
      services.AddMvc();
      services.AddNodeServices();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseStaticFiles();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();

        app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
        {
          HotModuleReplacement = true,
          HotModuleReplacementEndpoint = "/dist/__webpack_hmr"
        });

        app.UseMvc(routes =>
        {
          routes.MapSpaFallbackRoute(
              name: "spa-fallback",
              defaults: new { controller = "Home", action = "Index" });
        });
      }
      else
      {
        app.UseMvc(routes =>
        {
          routes.MapRoute(
           name: "default",
           template: "{controller=Home}/{action=Index}/{id?}");

          routes.MapRoute(
           "Sitemap",
           "sitemap.xml",
           new { controller = "Home", action = "SitemapXml" });

          routes.MapSpaFallbackRoute(
            name: "spa-fallback",
            defaults: new { controller = "Home", action = "Index" });

        });
        app.UseExceptionHandler("/Home/Error");
      }
    }
  }
}
