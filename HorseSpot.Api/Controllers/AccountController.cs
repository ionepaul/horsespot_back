using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HorseSpot.Api.Utils;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Models.Enums;
using HorseSpot.Models.Models;

namespace HorseSpot.Api.Controllers
{
    public class AccountController : ApiController
    {
        private readonly IUserBus _iUserBus;

        public AccountController(IUserBus iUserBus)
        {
            _iUserBus = iUserBus;
        }

        #region HttpGet

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/account/users")]
        public IEnumerable<UserViewModel> GetAll()
        {
            return _iUserBus.GetAllUsers();
        }

        [HttpGet]
        [Route("api/account/details/{id}")]
        public UserDTO GetUserDetails([FromUri] string id)
        {
            return _iUserBus.GetUserDetails(id);
        }

        [HttpGet]
        [Route("api/account/userhorseposts/{pageNumber}")]
        public GetHorseAdListResultsDTO GetAllHorseAdsForUser(int pageNumber, string userId)
        {
            return _iUserBus.GetAllForUser(pageNumber, userId);
        }

        [HttpGet]
        [Route("api/account/userhorsefavorites/{pageNumber}")]
        public GetHorseAdListResultsDTO GetHorseAdsFavoritesForUser(int pageNumber, string userId)
        {
            return _iUserBus.GetUserFavorites(pageNumber, userId);
        }

        [HttpGet]
        [Route("api/account/userreferences/{pageNumber}")]
        public GetHorseAdListResultsDTO GetHorseAdReferencesForUser(int pageNumber, string userId)
        {
            return _iUserBus.GetReferencesForUser(pageNumber, userId);
        }

        [HttpGet]
        [Route("api/account/isAdmin/{userId}")]
        public async Task<bool> CheckIfAdmin(string userId)
        {
            return await _iUserBus.CheckIfAdmin(userId);
        }

        #endregion

        #region HttpPost

        [HttpPost]
        public async Task<UserViewModel> Register([FromBody] UserViewModel userViewModel)
        {
            return await _iUserBus.RegisterUser(userViewModel);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<UserDTO> Edit([FromBody] EditProfileViewModel editProfile)
        {
            return await _iUserBus.EditProfile(UserIdExtractor.GetUserIdFromRequest(Request), editProfile);
        }

        [HttpPost]
        [Authorize]
        public async Task ChangePassword([FromBody] ChangePasswordViewModel changePassword)
        {
            await _iUserBus.ChangePassword(UserIdExtractor.GetUserIdFromRequest(Request), changePassword);
        }

        [HttpPost]
        [Route("api/account/forgotpassword")]
        public async Task ForgotPassword(string email)
        {
            await _iUserBus.ForgotPassword(email);
        }

        [HttpPost]
        [Route("api/account/newsletter")]
        public void SubscribeToNewsletter(string email)
        {
            _iUserBus.SubscribeToNewsletter(email);
        }

        [HttpPost]
        [Authorize]
        public async Task Delete()
        {
            await _iUserBus.Delete(UserIdExtractor.GetUserIdFromRequest(Request));
        }

        [HttpPost]
        [Route("api/user/profilephoto/upload/{id}")]
        public HttpResponseMessage UploadProfilePhoto([FromUri] string id)
        {
            var uploadFiles = HttpContext.Current.Request.Files;

            if (uploadFiles.Count > 0)
            {
                var profileImage = uploadFiles[0];
                CheckFormat(profileImage.FileName);

                var profilePicturesDir = ConfigurationManager.AppSettings["ProfilePicturesDirectory"];
                var serverPath = HttpContext.Current.Server.MapPath(profilePicturesDir);
                var imageName = Guid.NewGuid() + profileImage.FileName;
                var path = Path.Combine(serverPath, imageName);

                CreateDirectoryIfNotExist(serverPath);
                profileImage.SaveAs(path);

                _iUserBus.SetUserProfilePicture(imageName, id);

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, Resources.PleaseUpdateAtLeastOneImage);
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
