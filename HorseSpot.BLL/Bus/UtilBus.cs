using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Models.Enums;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HorseSpot.Models.Models;
using HorseSpot.Infrastructure.Validators;
using HorseSpot.BLL.Converters;
using HorseSpot.Infrastructure.MailService;
using System.Text.RegularExpressions;
using HorseSpot.Infrastructure.Constants;

namespace HorseSpot.BLL.Bus
{
    public class UtilBus : IUtilBus
    {
        #region Local Variables

        private IUtilAdDao _iUtilAdDao;
        private IUserDao _iUserDao;
        private IHorseAdDao _iHorseAdDao;
        private IMailerService _iMailerService;

        #endregion

        #region Constructor
        /// <summary>
        /// UtilBus Constructor
        /// </summary>
        /// <param name="iUtilAdDao">Util Dao Interface</param>
        /// <param name="iHorseAdDao">HorseAd Dao Interface</param>
        /// <param name="iUserDao">UserDao Interface</param>
        /// <param name="iMailerService">MailerService Interface</param>
        public UtilBus(IUtilAdDao iUtilAdDao, IHorseAdDao iHorseAdDao, IUserDao iUserDao, IMailerService iMailerService)
        {
            _iUtilAdDao = iUtilAdDao;
            _iHorseAdDao = iHorseAdDao;
            _iUserDao = iUserDao;
            _iMailerService = iMailerService;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets an image by id
        /// </summary>
        /// <param name="id">Image Id</param>
        /// <returns>Stream and Content Type or Exception if not found</returns>
        public Tuple<GridFSDownloadStream, string> GetImageById(string id)
        {
            Tuple<GridFSDownloadStream, string> result = _iUtilAdDao.GetImages(id);

            if (result == null)
            {
                throw new ResourceNotFoundException(Resources.InvalidPictureIdentifier);
            }

            return result;
        }

        /// <summary>
        /// Save images
        /// </summary>
        /// <param name="adId">Advertisment id</param>
        /// <param name="files">Posted Files</param>
        /// <param name="userId">User Id</param>
        public void SaveImages(string adId, List<HttpPostedFile> files, string userId)
        {
            if (files.Count > ApplicationConstants.MaximumFileToUpload)
            {
                throw new ValidationException(Resources.CannotUploadMoreThan5);
            }
            var horseAd = _iHorseAdDao.GetById(adId);

            if (horseAd.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }

            var advertismentsImages = horseAd.ImageIds;
            var totalImages = advertismentsImages.Count + files.Count;

            if (totalImages > ApplicationConstants.MaximumFileToUpload)
            {
                throw new ValidationException(Resources.CannotHaveMoreThan5PerAd);
            }

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).Replace(".", "");
                if (!Enum.IsDefined(typeof(SupportedImageExtensionEnum), extension.ToUpper()))
                {
                    throw new ValidationException(Resources.InvalidPictureFormat);
                }
            }
            
            foreach (var file in files)
            {
                var imageId = _iUtilAdDao.UploadOneImage(file);
                advertismentsImages.Add(imageId);
            }

            _iHorseAdDao.SetImages(adId, advertismentsImages.ToList());
        }

        /// <summary>
        /// Deletes an image by id
        /// </summary>
        /// <param name="adId">Advertisment Id</param>
        /// <param name="imageId">Image Id</param>
        /// <param name="userId">User Id</param>
        public void DeleteImage(string adId, string imageId, string userId)
        {
            var ad = _iHorseAdDao.GetById(adId);
            
            if (ad.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }

            if (!ad.ImageIds.Contains(imageId))
            {
                throw new ResourceNotFoundException(Resources.ImageNotFoundInAdImagesList);
            }

            ad.ImageIds.Remove(imageId);
            var newImageIdsList = ad.ImageIds.ToList();

            _iUtilAdDao.DeleteImage(imageId);

            _iHorseAdDao.SetImages(adId, newImageIdsList);
        }

        /// <summary>
        /// Sets the profile picture for an advertisment
        /// </summary>
        /// <param name="adId">Advertisment Id</param>
        /// <param name="imageId">Image Id</param>
        /// <param name="userId">User Id</param>
        public void SetHorseAdProfilePicture(string adId, string imageId, string userId)
        {
            var ad = _iHorseAdDao.GetById(adId);

            if (ad.UserId != userId)
            {
                throw new ForbiddenException(Resources.ActionRequiresAdditionalRights);
            }

            if (!ad.ImageIds.Contains(imageId))
            {
                throw new ResourceNotFoundException(Resources.ImageNotFoundInAdImagesList);
            }

            var imageIndex = ad.ImageIds.ToList().FindIndex(imgId => imgId == imageId);
            var aux = ad.ImageIds[imageIndex];
            ad.ImageIds[imageIndex] = ad.ImageIds[0];
            ad.ImageIds[0] = aux;

            var newImageIdsList = ad.ImageIds.ToList();
            _iHorseAdDao.SetImages(adId, newImageIdsList);
        }

        /// <summary>
        /// Set the user profile picture
        /// </summary>
        /// <param name="path">Image path</param>
        /// <param name="id">User Id</param>
        /// <returns>Task</returns>
        public async Task SetUserProfilePicture(string path, string id)
        {
            var user = _iUserDao.FindUserById(id);

            if (user == null)
            {
                throw new ValidationException(Resources.InvalidUserIdentifier);
            }

            if (user.ImagePath == ApplicationConstants.DefaultProfilePhoto)
            {
                CheckFormat(path);
                user.ImagePath = path;
            }
            else
            {
                CheckFormat(path);

                var currentImagePath = user.ImagePath;
                if (File.Exists(currentImagePath))
                {
                    File.Delete(currentImagePath);
                }

                user.ImagePath = path;
            }

            await _iUserDao.UpdateUser(user);
        }

        /// <summary>
        /// Gets the user profile picutre path
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>The Path</returns>
        public string GetUserPicturePath(string userId)
        {
            var user = _iUserDao.FindUserById(userId);

            if (user == null)
            {
                throw new ValidationException(Resources.InvalidUserIdentifier);
            }

            return user.ImagePath;
        }

        /// <summary>
        /// Sends email to an user with the message from another user
        /// </summary>
        /// <param name="emailModelDTO">Email Model </param>
        /// <returns>Task</returns>
        public async Task EmailSendingBetweenUsers(EmailModelDTO emailModelDTO)
        {
            if (emailModelDTO == null)
            {
                throw new ValidationException(Resources.InvalidSendEmailRequest);
            }

            ValidationHelper.ValidateModelAttributes<EmailModelDTO>(emailModelDTO);

            Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

            if (!emailRegex.IsMatch(emailModelDTO.Sender))
            {
                throw new ValidationException(Resources.InvalidEmailFormat);
            }

            if (emailModelDTO.Message.Length == 0) {
                throw new ValidationException(Resources.InvalidMessageFormat);
            }

            EmailModel emailModel = EmailSendingConverter.FromEmailModelDTOTOEmailModel(emailModelDTO);

            await _iMailerService.SendMail(emailModel);
        }

        /// <summary>
        /// Send email to HorseSpot with the message from contact page
        /// </summary>
        /// <param name="contactPageEmailModel">ContactPageEmail Model</param>
        /// <returns>Task</returns>
        public async Task ReceiveEmailFromContactPage(ContactPageEmailModel contactPageEmailModel)
        {
            if (contactPageEmailModel == null)
            {
                throw new ValidationException(Resources.InvalidSendEmailRequest);
            }

            ValidationHelper.ValidateModelAttributes<ContactPageEmailModel>(contactPageEmailModel);

            Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

            if (!emailRegex.IsMatch(contactPageEmailModel.Sender))
            {
                throw new ValidationException(Resources.InvalidEmailFormat);
            }

            if (contactPageEmailModel.Message.Length == 0)
            {
                throw new ValidationException(Resources.InvalidMessageFormat);
            }

            EmailModel emailModel = EmailSendingConverter.FromContactPageEmailModelTOEmailModel(contactPageEmailModel);

            await _iMailerService.SendMail(emailModel);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check Image format
        /// </summary>
        /// <param name="path">Image path</param>
        private void CheckFormat(string path)
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
