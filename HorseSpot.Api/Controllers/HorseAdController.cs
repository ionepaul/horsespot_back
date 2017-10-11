using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using HorseSpot.Api.Utils;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Models.Enums;
using HorseSpot.Models.Models;

namespace HorseSpot.Api.Controllers
{
    public class HorseAdController : ApiController
    {
        private readonly IHorseAdBus _iHorseAdBus;

        public HorseAdController(IHorseAdBus iHorseAdBus)
        {
            _iHorseAdBus = iHorseAdBus;
        }

        #region HttpGet

        [HttpGet]
        [Route("api/horses/get/{id}")]
        public HorseAdDTO Get([FromUri] int id)
        {
            return _iHorseAdBus.GetById(id);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/horses/unvalidated/{pageNumber}")]
        public GetHorseAdListResultsDTO GetAllForAdmin(int pageNumber)
        {
            return _iHorseAdBus.GetAllForAdmin(pageNumber);
        }

        [HttpGet]
        [Authorize]
        [Route("api/horses/ispostowner/{adId}")]
        public bool CheckPostOwner([FromUri] int adId)
        {
            return _iHorseAdBus.CheckPostOwner(adId, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        [HttpGet]
        [Route("api/horses/search")]
        public GetHorseAdListResultsDTO Search(string searchModel = "")
        {
            HorseAdSearchViewModel horseFilter = new HorseAdSearchViewModel();

            JavaScriptSerializer js = new JavaScriptSerializer();
            horseFilter = js.Deserialize<HorseAdSearchViewModel>(searchModel);

            if (horseFilter == null)
            {
                horseFilter = new HorseAdSearchViewModel();
            }

            return _iHorseAdBus.SearchHorses(horseFilter);
        }

        [HttpGet]
        [Route("api/horses/latest")]
        public LatestHorsesHomePageViewModel GetLatestForHomePage()
        {
            return _iHorseAdBus.GetLatestHorsesForHomePage();
        }

        #endregion

        #region HttpPost

        [HttpPost]
        [Authorize]
        [Route("api/horses/post")]
        public async Task Post([FromBody] HorseAdDTO horseAdDTO)
        {
            await _iHorseAdBus.Add(horseAdDTO, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        [HttpPost]
        [Authorize]
        [Route("api/horses/update/{id}")]
        public async Task Update([FromUri] int id, [FromBody] HorseAdDTO horseAdDTO)
        {
            await _iHorseAdBus.Update(id, horseAdDTO, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        [Authorize]
        [HttpPost]
        [Route("api/horses/delete/{id}/{isSold}")]
        public async Task Delete([FromUri] int id, bool isSold)
        {
            await _iHorseAdBus.Delete(id, UserIdExtractor.GetUserIdFromRequest(Request), isSold);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/horses/validate/{id}")]
        public async Task Validate([FromUri] int id)
        {
            await _iHorseAdBus.Validate(id);
        }

        [HttpPost]
        [Authorize]
        [Route("api/horses/favorite/{id}")]
        public void AddToFavorite([FromUri] int id)
        {
            _iHorseAdBus.AddToFavorite(id, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        [HttpPost]
        [Route("api/horses/views/{id}")]
        public async Task IncreaseViews([FromUri] int id)
        {
            await _iHorseAdBus.IncreaseViews(id);
        }

        [HttpPost]
        [Authorize]
        [Route("api/horses/images/save/{adId}/{imageName}")]
        public async Task SaveNewImage([FromUri] int adId, [FromUri] string imageName)
        {
            await _iHorseAdBus.SaveNewImage(adId, imageName, UserIdExtractor.GetUserIdFromRequest(Request));
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
                CheckFormat(image.FileName);

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
        [Authorize]
        [Route("api/horses/images/delete/{imageId}")]
        public void Delete([FromUri] int imageId)
        {
            var imageName = _iHorseAdBus.DeleteImage(imageId, UserIdExtractor.GetUserIdFromRequest(Request));

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
        [Route("api/horses/images/delete")]
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
        [Route("api/horses/images/profilepic/{imageId}")]
        public void SetAsAdProfilePicture([FromUri] int imageId)
        {
            _iHorseAdBus.SetHorseAdProfilePicture(imageId, UserIdExtractor.GetUserIdFromRequest(Request));
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

        public void CheckFormat(string path)
        {
            var extension = Path.GetExtension(path).Replace(".", "");

            if (!Enum.IsDefined(typeof(SupportedImageExtensionEnum), extension.ToUpper()))
            {
                throw new ValidationException(Resources.InvalidPictureFormat);
            }
        }

        #endregion

    }
}
