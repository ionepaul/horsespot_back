using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Interfaces;
using HorseSpot.DAL.Models;
using HorseSpot.Infrastructure.Constants;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorseSpot.DAL.Dao
{
    public class UserDao : IUserDao, IDisposable
    {
        private HorseSpotDataContext _ctx;

        private UserManager<UserModel> _userManager;

        /// <summary>
        /// UserDao Constructor
        /// </summary>
        public UserDao()
        {
            _ctx = new HorseSpotDataContext();
            _userManager = new UserManager<UserModel>(new UserStore<UserModel>(_ctx));
            _userManager.UserValidator = new UserValidator<UserModel>(_userManager) { AllowOnlyAlphanumericUserNames = false };
        }

        /// <summary>
        /// Creates user entity in the database
        /// </summary>
        /// <param name="userModel">User Model</param>
        /// <param name="password">User Passowrd</param>
        /// <returns>Created Entity</returns>
        public async Task<UserModel> RegisterUser(UserModel userModel, string password)
        {
            var result = await _userManager.CreateAsync(userModel, password);

            UserModel currentUser = null;

            if (result.Succeeded)
            {
                currentUser = _userManager.FindByName(userModel.UserName);

                var roleresult = _userManager.AddToRole(currentUser.Id, ApplicationConstants.UserRole);
            }

            return currentUser;
        }

        /// <summary>
        /// Updates an user entity in the database
        /// </summary>
        /// <param name="userModel">User Model</param>
        /// <returns>Updated User Model</returns>
        public async Task<UserModel> UpdateUser(UserModel userModel)
        {
            var result = await _userManager.UpdateAsync(userModel);

            var user = _userManager.FindById(userModel.Id);
            
            return user;
        }

        /// <summary>
        /// Find user in the database by username and password
        /// </summary>
        /// <param name="userName">User's username</param>
        /// <param name="password">User's password</param>
        /// <returns>User Model</returns>
        public async Task<UserModel> FindUser(string userName, string password)
        {
            UserModel user = await _userManager.FindAsync(userName, password);

            return user;
        }

        /// <summary>
        /// Change an account password in the database
        /// </summary>
        /// <param name="userId">User id to change password for</param>
        /// <param name="newPassword">The new password</param>
        /// <returns>Task</returns>
        public async Task ChangeUserPassword(string userId, string newPassword)
        {
            var user = _userManager.FindById(userId);

            var newPasswordHashed = _userManager.PasswordHasher.HashPassword(newPassword);

            user.PasswordHash = newPasswordHashed;
            
            await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Finds user in the database by id
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>User Model</returns>
        public UserModel FindUserById(string userId)
        {
            var user = _userManager.FindById(userId);

            return user;
        }

        /// <summary>
        /// Gets all users from database
        /// </summary>
        /// <returns>IEnumerable of User Model</returns>
        public IEnumerable<UserModel> GetAllUsers()
        {
            return _userManager.Users.AsEnumerable();
        }

        /// <summary>
        /// Deletes user from database
        /// </summary>
        /// <param name="user">User Model to delete</param>
        /// <returns>Task</returns>
        public async Task DeleteUser(UserModel user)
        {
            await _userManager.DeleteAsync(user);
        }

        /// <summary>
        /// Find user by email in database
        /// </summary>
        /// <param name="email">Email to look for</param>
        /// <returns>User Model</returns>
        public UserModel FindUserByEmail(string email)
        {
            return _userManager.FindByEmail(email);
        }

        /// <summary>
        /// Add subscriber model in the database subscriber's list
        /// </summary>
        /// <param name="subscriber">SubscriberModel</param>
        public void RegisterToNewsletter(Subscriber subscriber)
        {
            _ctx.Subscribers.Add(subscriber);
            _ctx.SaveChanges();
        }

        /// <summary>
        /// Find subscriber by email
        /// </summary>
        /// <param name="email">Subscriber's email</param>
        /// <returns>Subscriber model</returns>
        public Subscriber FindSubscriberByEmail(string email)
        {
            return _ctx.Subscribers.Where(x => x.Email == email).FirstOrDefault();
        }

        /// <summary>
        /// Gets the user roles from database for a user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>List of roles</returns>
        public async Task<IList<string>> UserRoles(string userId)
        {
            IList<string> roles = await _userManager.GetRolesAsync(userId);

            return roles;
        }

        /// <summary>
        /// Dispose class
        /// </summary>
        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}
