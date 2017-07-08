using HorseSpot.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
