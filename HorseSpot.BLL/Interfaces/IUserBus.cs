using System.Collections.Generic;
using System.Threading.Tasks;
using HorseSpot.Models.Models;

namespace HorseSpot.BLL.Interfaces
{
    public interface IUserBus
    {
        Task<UserViewModel> RegisterUser(UserViewModel user);
        Task<UserDTO> EditProfile(string id, EditProfileViewModel editProfile);
        Task Delete(string userId);
        Task ChangePassword(string id, ChangePasswordViewModel changePassword);
        IEnumerable<UserViewModel> GetAllUsers();
        UserDTO GetUserDetails(string id);
        Task ForgotPassword(string email);
        void SubscribeToNewsletter(string email);
        Task<bool> CheckIfAdmin(string userId);
        GetHorseAdListResultsDTO GetAllForUser(int pageNumber, string userId);
        GetHorseAdListResultsDTO GetReferencesForUser(int pageNumber, string userId);
        GetHorseAdListResultsDTO GetUserFavorites(int pageNumber, string userId);
        Task SetUserProfilePicture(string path, string id);
        UserDTO FindUserByEmail(string email);
    }
}
