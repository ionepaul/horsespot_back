using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using HorseSpot.Infrastructure.Resources;
using System.Web;
using System.Web.Http;
using HorseSpot.BLL.Interfaces;
using System;
using System.Threading.Tasks;
using HorseSpot.Models.Models;
using HorseSpot.Api.Utils;

namespace HorseSpot.Api.Controllers
{
    public class UtilsController : ApiController
    {
        private IUtilBus _iUtilBus;

        public UtilsController(IUtilBus iUtilBus)
        {
            _iUtilBus = iUtilBus;
        }

        [HttpPost]
        [Authorize]
        [Route("api/horses/images/delete/{imageId}")]
        public void Delete([FromUri] int imageId)
        {
            var imageName = _iUtilBus.DeleteImage(imageId, UserIdExtractor.GetUserIdFromRequest(Request));

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
            _iUtilBus.SetHorseAdProfilePicture(imageId, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        [HttpPost]
        [Route("api/user/profilephoto/upload/{id}")]
        public HttpResponseMessage UploadProfilePhoto([FromUri] string id)
        {
            var uploadFiles = HttpContext.Current.Request.Files;

            if (uploadFiles.Count > 0)
            {
                var profileImage = uploadFiles[0];
                var profilePicturesDir = ConfigurationManager.AppSettings["ProfilePicturesDirectory"];
                var serverPath = HttpContext.Current.Server.MapPath(profilePicturesDir);
                var path = Path.Combine(serverPath, profileImage.FileName);

                CreateDirectoryIfNotExist(serverPath);
                profileImage.SaveAs(path);

                _iUtilBus.SetUserProfilePicture(path, id);

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, Resources.PleaseUpdateAtLeastOneImage);
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
