using System.Collections.Generic;
using System.Threading.Tasks;
using HorseSpot.DAL.Entities;
using HorseSpot.Models.Models;
using Microsoft.AspNet.Identity;

namespace HorseSpot.BLL.Interfaces
{
    public interface IAuthorizationBus
    {
        Task<UserViewModel> FindUser(string username, string password);
        ClientDTO FindClient(string clientId);
        Task<RefreshTokenDTO> FindRefreshToken(string hashedTokenId);
        Task<bool> RemoveRefreshToken(string hashedTokenId);
        Task<bool> AddRefreshToken(RefreshTokenDTO token);
        Task<IList<string>> UserRoles(string id);
        Task<UserModel> FindUserByLoginInfo(UserLoginInfo loginInfo);
        Task<IdentityResult> CreateExternalUser(string userName);
        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);
    }
}
