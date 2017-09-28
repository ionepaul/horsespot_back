using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Models.Models;

namespace HorseSpot.Api.Controllers
{
    public class UtilsController : ApiController
    {
        private readonly IUtilBus _iUtilBus;

        public UtilsController(IUtilBus iUtilBus)
        {
            _iUtilBus = iUtilBus;
        }

        #region HttpPost

        [HttpPost]
        [Authorize]
        [Route("api/images/delete")]
        public void DeleteImageByName(string imageName)
        {
            var horseAdvImageDir = ConfigurationManager.AppSettings["HorseAdsImgDirectory"];
            var serverPath = HttpContext.Current.Server.MapPath(horseAdvImageDir);

            if (Directory.Exists(Path.GetDirectoryName(serverPath)))
            {
                var path = Path.Combine(serverPath, imageName);
                File.Delete(path);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/horses/images/upload")]
        public HttpResponseMessage UploadHorseAdImage()
        {
            var uploadFiles = HttpContext.Current.Request.Files;

            if (uploadFiles.Count > 0)
            {
                var image = uploadFiles[0];
                _iUtilBus.CheckFormat(image.FileName);

                var horseAdvImageDir = ConfigurationManager.AppSettings["HorseAdsImgDirectory"];
                var serverPath = HttpContext.Current.Server.MapPath(horseAdvImageDir);
                var imageName = Guid.NewGuid() + image.FileName;
                var path = Path.Combine(serverPath, imageName);

                CreateDirectoryIfNotExist(serverPath);
                image.SaveAs(path);

                return Request.CreateResponse(HttpStatusCode.OK, imageName);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, Resources.PleaseUpdateAtLeastOneImage);
        }

        [HttpPost]
        [Route("api/sendemail")]
        public async Task SendMail([FromBody] EmailModelDTO emailModelDTO)
        {
            await _iUtilBus.EmailSendingBetweenUsers(emailModelDTO);
        }

        [HttpPost]
        [Route("api/contactformemail")]
        public async Task ReceiveEmailFromContact([FromBody] ContactPageEmailModel contactPageEmailModel)
        {
            await _iUtilBus.ReceiveEmailFromContactPage(contactPageEmailModel);
        }

        #endregion

        #region HttpGet

        [HttpGet]
        [Route("api/countries")]
        public IEnumerable<string> GetAllCountries()
        {
            return _iUtilBus.GetAllCountries();
        }

        [HttpGet]
        [Route("api/abilities")]
        public IEnumerable<HorseAbilityDTO> GetAllAbilities()
        {
            return _iUtilBus.GetAllAbilities();
        }

        [HttpGet]
        [Route("api/priceranges")]
        public IEnumerable<PriceRangeDTO> GetAllPriceRanges()
        {
            return _iUtilBus.GetAllPriceRanges();
        }

        [HttpGet]
        [Route("api/recommendedriders")]
        public IEnumerable<RecommendedRiderDTO> GetAllRecommendedRiders()
        {
            return _iUtilBus.GetAllRecommendedRiders();
        }

        #endregion

        #region Private Methods

        private void CreateDirectoryIfNotExist(string serverPath)
        {
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }
        }

        #endregion
    }
}
