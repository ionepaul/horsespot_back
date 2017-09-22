using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using HorseSpot.Api.Utils;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Models.Models;

namespace HorseSpot.Api.Controllers
{
    public class AccountController : ApiController
    {
        private IUserBus _iUserBus;

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

        #endregion

    }
}
