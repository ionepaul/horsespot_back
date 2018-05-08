using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Infrastructure.Constants;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HorseSpot.DAL.Dao
{
    public class UserDao : IUserDao, IDisposable
    {
        private HorseSpotDataContext _ctx;

        private UserManager<UserModel> _userManager;

        public UserDao()
        {
            _ctx = new HorseSpotDataContext();
            _userManager = new UserManager<UserModel>(new UserStore<UserModel>(_ctx));
            _userManager.UserValidator = new UserValidator<UserModel>(_userManager) { AllowOnlyAlphanumericUserNames = false };
        }

        #region Public Methods

        public async Task<UserModel> RegisterUser(UserModel userModel, string password)
        {
            var result = await _userManager.CreateAsync(userModel, password);

            AddUserRole(result, userModel.UserName);

            return userModel;
        }

        public async Task<IdentityResult> CreateAsync(UserModel user)
        {
            var result = await _userManager.CreateAsync(user);

            AddUserRole(result, user.UserName);

            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }

        public async Task<UserModel> UpdateUser(UserModel userModel)
        {
            var result = await _userManager.UpdateAsync(userModel);
            
            var user = _userManager.FindById(userModel.Id);
            
            return user;
        }

        public async Task<UserModel> FindUser(string userName, string password)
        {
            UserModel user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public async Task<UserModel> FindUserByLoginInfo(UserLoginInfo loginInfo)
        {
            UserModel user = await _userManager.FindAsync(loginInfo);
            
            return user;
        }

        public async Task ChangeUserPassword(string userId, string newPassword)
        {
            var user = _userManager.FindById(userId);

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(newPassword);
            
            await _userManager.UpdateAsync(user);
        }

        public UserModel FindUserById(string userId)
        {
            var user = _userManager.FindById(userId);

            return user;
        }

        public IEnumerable<UserModel> GetAllUsers()
        {
            return _userManager.Users.AsEnumerable();
        }

        public async Task DeleteUser(UserModel user)
        {
            await _userManager.DeleteAsync(user);
        }

        public UserModel FindUserByEmail(string email)
        {
            return _userManager.FindByEmail(email);
        }

        public void RegisterToNewsletter(Subscriber subscriber)
        {
            _ctx.Subscribers.Add(subscriber);
            _ctx.SaveChanges();
        }

        public Subscriber FindSubscriberByEmail(string email)
        {
            return _ctx.Subscribers.Where(x => x.Email == email).FirstOrDefault();
        }

        public async Task<IList<string>> UserRoles(string userId)
        {
            IEnumerable<string> roles = await _userManager.GetRolesAsync(userId);

            return roles.ToList();
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }

        #endregion

        #region Private Methods

        private void AddUserRole(IdentityResult result, string userName)
        {
            UserModel currentUser = null;

            if (result.Succeeded)
            {
                currentUser = _userManager.FindByName(userName);

                var roleresult = _userManager.AddToRole(currentUser.Id, ApplicationConstants.UserRole);
            }
        }

        #endregion
    }
}
