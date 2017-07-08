using HorseSpot.Api.Utils;
using HorseSpot.BLL.Interfaces;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using HorseSpot.Infrastructure.Resources;
using System.Web;
using System.Web.Http;
using HorseSpot.Models.Models;
using System.Threading.Tasks;

namespace HorseSpot.Api.Controllers
{
    public class UtilsController : ApiController
    {
        private IUtilBus _iUtilBus;

        /// <summary>
        /// Utils Controller Constructor 
        /// </summary>
        /// <param name="iUtilBus">UtilBus Bussines Logic Interface</param>
        public UtilsController(IUtilBus iUtilBus)
        {
            _iUtilBus = iUtilBus;
        }

        /// <summary>
        /// API Interface to upload images related to a horse advertisment
        /// </summary>
        /// <param name="adId">Advertisment Id</param>
        /// <returns>Response message</returns>
        [HttpPost]
        [Authorize]
        [Route("api/images/upload/{adId}")]
        public HttpResponseMessage UploadImages([FromUri] string adId)
        {
            var imagesToSave = new List<HttpPostedFile>();
            var uploadFiles = HttpContext.Current.Request.Files;

            if (uploadFiles.Count > 0)
            {
                foreach (string file in uploadFiles)
                {
                    var postedFile = uploadFiles[file];
                    imagesToSave.Add(postedFile);
                }

                _iUtilBus.SaveImages(adId, imagesToSave, UserIdExtractor.GetUserIdFromRequest(Request));

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Resources.PleaseUpdateAtLeastOneImage);
            }
        }

        /// <summary>
        /// API Interaface to get an image by id
        /// </summary>
        /// <param name="id">Image Id</param>
        /// <returns>Response message</returns>
        [HttpGet]
        [Route("api/images/get/{id}")]
        public HttpResponseMessage GetImageById([FromUri] string id)
        {
            Tuple<GridFSDownloadStream, string> stream = _iUtilBus.GetImageById(id);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            result.Content = new StreamContent(stream.Item1);

            result.Content.Headers.ContentType = new MediaTypeHeaderValue(stream.Item2);

            return result;
        }

        /// <summary>
        /// API Interface to delete image by id 
        /// </summary>
        /// <param name="adId">Advertisment Id</param>
        /// <param name="imageId">Image Id</param>
        [HttpDelete]
        [Authorize]
        [Route("api/images/delete/{adId}/{imageId}")]
        public void Delete([FromUri] string adId, [FromUri] string imageId)
        {
            _iUtilBus.DeleteImage(adId, imageId, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        /// <summary>
        /// API Interface to set an horse advertisment profile picture
        /// </summary>
        /// <param name="adId">Advertisment Id</param>
        /// <param name="imageId">Image Id</param>
        [HttpPut]
        [Authorize]
        [Route("api/horsead/profilepic/{adId}/{imageId}")]
        public void SetAsAdProfilePicture([FromUri] string adId, [FromUri] string imageId)
        {
            _iUtilBus.SetHorseAdProfilePicture(adId, imageId, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        /// <summary>
        /// API Interface to upload user profile picture
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Response Message</returns>
        [HttpPost]
        [Route("api/user/profilephoto/upload/{id}")]
        public HttpResponseMessage UploadProfilePhoto([FromUri] string id)
        {
            var files = HttpContext.Current.Request.Files;

            if (files.Count > 0)
            {
                var postedFile = files[0];
                var serverpath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ProfilePicturesDirectory"]);
                var path = Path.Combine(serverpath, postedFile.FileName);
                postedFile.SaveAs(path);

                _iUtilBus.SetUserProfilePicture(path, id);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Resources.PleaseUpdateAtLeastOneImage);
            }
        }

        /// <summary>
        /// API Interface to get user profile picture
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Response Message</returns>
        [HttpGet]
        [Route("api/user/profilephoto/get/{userId}")]
        public HttpResponseMessage GetProfilePicture([FromUri] string userId)
        {
            string path = _iUtilBus.GetUserPicturePath(userId);

            string fileName = Path.GetFileName(path);
            string contentType = MimeMapping.GetMimeMapping(fileName);

            var fileStream = new FileStream(path, FileMode.Open);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            result.Content = new StreamContent(fileStream);

            result.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return result;
        }

        /// <summary>
        /// API Interface to send email to an user
        /// </summary>
        /// <param name="emailModelDTO">EmailModel DTO</param>
        /// <returns>Task</returns>
        [HttpPost]
        [Route("api/sendemail")]
        public async Task SendMail([FromBody] EmailModelDTO emailModelDTO)
        {
            await _iUtilBus.EmailSendingBetweenUsers(emailModelDTO);
        }

        /// <summary>
        /// API Interface to send email to horsespot from contact page
        /// </summary>
        /// <param name="contactPageEmailModel">Conact Page Email Model</param>
        /// <returns>Task</returns>
        [HttpPost]
        [Route("api/contactformemail")]
        public async Task ReceiveEmailFromContact([FromBody] ContactPageEmailModel contactPageEmailModel)
        {
            await _iUtilBus.ReceiveEmailFromContactPage(contactPageEmailModel);
        }
    }
}
