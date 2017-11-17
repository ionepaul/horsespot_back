using System;
using System.Threading.Tasks;
using HorseSpot.Web.Server.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace HorseSpot.Web.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet]
    public async Task<IActionResult> Index()
    {
      var prerenderResult = await Request.BuildPrerender();

      ViewData["SpaHtml"] = prerenderResult.Html; // our <app> from Angular
      ViewData["Title"] = prerenderResult.Globals["title"]; // set our <title> from Angular
      ViewData["Styles"] = prerenderResult.Globals["styles"]; // put styles in the correct place
      ViewData["Scripts"] = prerenderResult.Globals["scripts"]; // scripts (that were in our header)
      ViewData["Meta"] = prerenderResult.Globals["meta"]; // set our <meta> SEO tags
      ViewData["Links"] = prerenderResult.Globals["links"]; // set our <link rel="canonical"> etc SEO tags
      ViewData["TransferData"] = prerenderResult.Globals["transferData"]; // our transfer data set to window.TRANSFER_CACHE = {};

      return View();
    }

    [HttpGet]
    [Route("sitemap.xml")]
    public async Task<IActionResult> SitemapXml()
    {
      String xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

      xml += "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">";
      xml += "<url>";
      xml += "<loc>https://horse-spot.com/home</loc>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>https://horse-spot.com/horses-for-sale/showjumping/1</loc>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>https://horse-spot.com/horses-for-sale/dressage/1</loc>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>https://horse-spot.com/horses-for-sale/eventing/1</loc>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>https://horse-spot.com/horses-for-sale/endurance/1</loc>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>https://horse-spot.com/horses-for-sale/driving/1</loc>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>https://horse-spot.com/horses-for-sale/foals/1</loc>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>https://horse-spot.com/horses-for-sale/leisure/1</loc>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "</urlset>";

      return Content(xml, "text/xml");

    }

    public IActionResult Error()
    {
      return View();
    }
  }
}
