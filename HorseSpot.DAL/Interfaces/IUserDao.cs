﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HorseSpot.DAL.Entities;

namespace HorseSpot.DAL.Interfaces
{
    public interface IUserDao
    {
        Task<UserModel> RegisterUser(UserModel userModel, string password);
        Task<UserModel> FindUser(string userName, string password);
        Task<IEnumerable<string>> UserRoles(string userId);
        UserModel FindUserById(string userId);
        Task<UserModel> UpdateUser(UserModel userModel);
        Task ChangeUserPassword(string userId, string newPassword);
        IEnumerable<UserModel> GetAllUsers();
        Task DeleteUser(UserModel user);
        UserModel FindUserByEmail(string email);
        void RegisterToNewsletter(Subscriber subscriber);
        Subscriber FindSubscriberByEmail(string email);
    }
}
