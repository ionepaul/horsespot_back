using HorseSpot.Api.Utils;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace HorseSpot.Api.Controllers
{
    public class AccountController : ApiController
    {
        private IUserBus _iUserBus;
        private IHorseAdBus _iHorseAdBus;

        /// <summary>
        /// AccountController Construtor
        /// </summary>
        /// <param name="iUserBus">User Bussines Logic Interface</param>
        /// <param name="iHorseAdBus">Horse Advertisment Bussines Logic Interface</param>
        public AccountController(IUserBus iUserBus, IHorseAdBus iHorseAdBus)
        {
            _iUserBus = iUserBus;
            _iHorseAdBus = iHorseAdBus;
        }

        /// <summary>
        /// API Interface to register a user within the aplication
        /// </summary>
        /// <param name="userViewModel">User model required in order to register</param>
        /// <returns>The registered user model</returns>
        [HttpPost]
        public async Task<UserViewModel> Register([FromBody] UserViewModel userViewModel)
        {
            return await _iUserBus.RegisterUser(userViewModel);
        }

        /// <summary>
        /// API Interface to edit an user profile
        /// </summary>
        /// <param name="editProfile">Edit profile model containing modified data</param>
        /// <returns>The updated user model</returns>
        [HttpPut]
        [HttpPatch]
        [Authorize]
        public async Task<UserDTO> Edit([FromBody] EditProfileViewModel editProfile)
        {   
            return await _iUserBus.EditProfile(UserIdExtractor.GetUserIdFromRequest(Request), editProfile);
        }

        /// <summary>
        /// API Interface to change password to an account
        /// </summary>
        /// <param name="changePassword">Model containing change password required data</param>
        [HttpPut]
        [HttpPatch]
        [Authorize]
        public async Task ChangePassword([FromBody] ChangePasswordViewModel changePassword)
        {
            await _iUserBus.ChangePassword(UserIdExtractor.GetUserIdFromRequest(Request), changePassword);
        }

        /// <summary>
        /// API Interface to generate a new password for a registred email
        /// </summary>
        /// <param name="email">Application registred email to send new password to</param>
        [HttpPost]
        [Route("api/account/forgotpassword")]
        public async Task ForgotPassword(string email)
        {
            await _iUserBus.ForgotPassword(email);
        }

        /// <summary>
        /// API Interface to subscribe to application newsletter
        /// </summary>
        /// <param name="email">Subscribed email</param>
        [HttpPost]
        [Route("api/account/newsletter")]
        public void SubscribeToNewsletter(string email)
        {
            _iUserBus.SubscribeToNewsletter(email);
        }

        /// <summary>
        /// API Interface to show all users, only available for admin role
        /// </summary>
        /// <returns>Array of user model</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/account/users")]
        public IEnumerable<UserViewModel> GetAll()
        {
            return _iUserBus.GetAllUsers();
        }

        /// <summary>
        /// API Interface to get the user details
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User model</returns>
        [HttpGet]
        [Route("api/account/details/{id}")]
        public UserDTO GetUserDetails([FromUri] string id)
        {
            return _iUserBus.GetUserDetails(id);
        }

        /// <summary>
        /// API Interface to delete an user account
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task Delete()
        {
            await _iUserBus.Delete(UserIdExtractor.GetUserIdFromRequest(Request));
        }

        /// <summary>
        /// API Interface to retrive user's posts by page
        /// </summary>
        /// <param name="pageNumber">Indicates the page number</param>
        /// <returns>Total number of posts added by user and posts of the requested page</returns>
        [HttpGet]
        [Authorize]
        [Route("api/account/userhorseposts/{pageNumber}")]
        public GetHorseAdListResultsDTO GetAllHorseAdsForUser(int pageNumber)
        {
            return _iHorseAdBus.GetAllForUser(pageNumber, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        /// <summary>
        /// API Interface to retrive all user's wish list posts by page
        /// </summary>
        /// <param name="pageNumber">Indicates the page number</param>
        /// <returns>Total number of posts in wish list and posts of the requested page</returns>
        [HttpGet]
        [Authorize]
        [Route("api/account/userhorsefavorites/{pageNumber}")]
        public GetHorseAdListResultsDTO GetHorseAdsFavoritesForUser(int pageNumber)
        {
            return _iHorseAdBus.GetUserFavorites(pageNumber, UserIdExtractor.GetUserIdFromRequest(Request));
        }

        /// <summary>
        /// API Interface to check if a user is admin
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>True/False</returns>
        [HttpGet]
        [Route("api/account/isAdmin/{userId}")]
        public async Task<bool> CheckIfAdmin(string userId)
        {
            return await _iUserBus.CheckIfAdmin(userId);
        }
    }
}
