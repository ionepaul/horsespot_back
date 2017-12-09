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
      xml += "<loc>http://www.horse-spot.com/home</loc>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"de\" href=\"https://www.horse-spot.com/home?lang=de\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"fr\" href=\"https://www.horse-spot.com/home?lang=fr\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"ro\" href=\"https://www.horse-spot.com/home?lang=ro\"/>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/horses-for-sale/showjumping/1</loc>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"de\" href=\"https://www.horse-spot.com/horses-for-sale/showjumping/1?lang=de\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"fr\" href=\"https://www.horse-spot.com/horses-for-sale/showjumping/1?lang=fr\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"ro\" href=\"https://www.horse-spot.com/horses-for-sale/showjumping/1?lang=ro\"/>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/horses-for-sale/dressage/1</loc>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"de\" href=\"https://www.horse-spot.com/horses-for-sale/dressage/1?lang=de\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"fr\" href=\"https://www.horse-spot.com/horses-for-sale/dressage/1?lang=fr\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"ro\" href=\"https://www.horse-spot.com/horses-for-sale/dressage/1?lang=ro\"/>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/horses-for-sale/eventing/1</loc>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"de\" href=\"https://www.horse-spot.com/horses-for-sale/eventing/1?lang=de\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"fr\" href=\"https://www.horse-spot.com/horses-for-sale/eventing/1?lang=fr\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"ro\" href=\"https://www.horse-spot.com/horses-for-sale/eventing/1?lang=ro\"/>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/horses-for-sale/endurance/1</loc>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"de\" href=\"https://www.horse-spot.com/horses-for-sale/endurance/1?lang=de\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"fr\" href=\"https://www.horse-spot.com/horses-for-sale/endurance/1?lang=fr\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"ro\" href=\"https://www.horse-spot.com/horses-for-sale/endurance/1?lang=ro\"/>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/horses-for-sale/driving/1</loc>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"de\" href=\"https://www.horse-spot.com/horses-for-sale/driving/1?lang=de\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"fr\" href=\"https://www.horse-spot.com/horses-for-sale/driving/1?lang=fr\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"ro\" href=\"https://www.horse-spot.com/horses-for-sale/driving/1?lang=ro\"/>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/horses-for-sale/foals/1</loc>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"de\" href=\"https://www.horse-spot.com/horses-for-sale/foals/1?lang=de\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"fr\" href=\"https://www.horse-spot.com/horses-for-sale/foals/1?lang=fr\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"ro\" href=\"https://www.horse-spot.com/horses-for-sale/foals/1?lang=ro\"/>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/horses-for-sale/leisure/1</loc>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"de\" href=\"https://www.horse-spot.com/horses-for-sale/leisure/1?lang=de\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"fr\" href=\"https://www.horse-spot.com/horses-for-sale/leisure/1?lang=fr\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"ro\" href=\"https://www.horse-spot.com/horses-for-sale/leisure/1?lang=ro\"/>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/horses/add</loc>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"de\" href=\"https://www.horse-spot.com/horses/add?lang=de\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"fr\" href=\"https://www.horse-spot.com/horses/add?lang=fr\"/>";
      xml += "<xhtml:link rel=\"alternate\" hreflang=\"ro\" href=\"https://www.horse-spot.com/horses/add?lang=ro\"/>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/advertise</loc>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/hire-us</loc>";
      xml += "<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>";
      xml += "</url>";
      xml += "<url>";
      xml += "<loc>http://www.horse-spot.com/contact</loc>";
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
