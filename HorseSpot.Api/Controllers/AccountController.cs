using HorseSpot.Api.Utils;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Infrastructure.Constants;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Models.Enums;
using HorseSpot.Models.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HorseSpot.Api.Controllers
{
    public class AccountController : ApiController
    {
        private readonly IUserBus _iUserBus;
        private readonly IAuthorizationBus _iAuthorizationBus;

        public AccountController(IUserBus iUserBus, IAuthorizationBus iAuthorizationBus)
        {
            _iUserBus = iUserBus;
            _iAuthorizationBus = iAuthorizationBus;
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
        [Authorize]
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

        [HttpGet]
        [Route("api/account/fullProfile/{userId}")]
        public UserFullProfile GetUserFullProfile([FromUri] string userId)
        {
            return _iUserBus.GetUserFullProfile(userId);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/account/obtainLocalAccessToken")]
        public async Task<JObject> ObtainLocalAccessToken(string provider, string externalAccessToken, string clientId)
        {
            var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalAccessToken);

            if (verifiedAccessToken == null)
            {
                throw new ValidationException(Resources.InvalidProviderOrExternalToken);
            }

            var user = await _iAuthorizationBus.FindUserByLoginInfo(new UserLoginInfo(provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                throw new ConflictException(Resources.ExternalUserIsNotRegistred);
            }

            var accessTokenResponse = await GenerateLocalAccessTokenResponse(user.Email, clientId);

            return accessTokenResponse;
        }

        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("api/account/ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;

            if (error != null)
            {
                throw new ValidationException(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                throw new ValidationException(redirectUriValidationResult);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                throw new ValidationException(Resources.InvalidExternalLoginRequest);
            }

            if (externalLogin.LoginProvider != provider)
            {
                Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                return new ChallengeResult(provider, this);
            }

            var user = await _iAuthorizationBus.FindUserByLoginInfo(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                return await RegisterExternalUser(externalLogin, redirectUri);
            }

            redirectUri = CreateRedirectUri(redirectUri, false, externalLogin.ExternalAccessToken, externalLogin.LoginProvider, false, "");

            return Redirect(redirectUri);
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
        public async Task Delete(string provider, string externalToken)
        {
            var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalToken);

            if (verifiedAccessToken == null)
            {
                throw new ValidationException(Resources.InvalidProviderOrExternalToken);
            }

            var user = await _iAuthorizationBus.FindUserByLoginInfo(new UserLoginInfo(provider, verifiedAccessToken.user_id));

            await _iUserBus.Delete(user);
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

                return Request.CreateResponse(HttpStatusCode.OK, imageName);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, Resources.PleaseUpdateAtLeastOneImage);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/account/updateExternalUser")]
        public async Task<JObject> AddPhoneNumberToExternalUser(string provider, string externalToken, string clientId, [FromBody] EditProfileViewModel externalUpdates)
        {
            var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalToken);

            if (verifiedAccessToken == null)
            {
                throw new ValidationException(Resources.InvalidProviderOrExternalToken);
            }

            var user = await _iAuthorizationBus.FindUserByLoginInfo(new UserLoginInfo(provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                await _iUserBus.EditProfile(user.Id, externalUpdates);
            }

            var accessTokenResponse = await GenerateLocalAccessTokenResponse(user.Email, clientId);

            return accessTokenResponse;
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

        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {
            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                throw new ValidationException(Resources.InvalidRedirectUri);
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                throw new ValidationException(Resources.InvalidRedirectUri);
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ValidationException(Resources.ClientIdIsRequired);
            }

            var client = _iAuthorizationBus.FindClient(clientId);

            if (client == null)
            {
                throw new ForbiddenException(string.Format(Resources.ClientIdNotRegistred, clientId));
            }

            if (!string.Equals(client.AllowedOrigin, "*") && !string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException(string.Format(Resources.ClientIdDoesNotHaveEnoughRight, clientId));
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;
        }

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ValidationException(Resources.InvalidProviderOrExternalToken);
            }

            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (string.Equals(provider, AuthConstants.Providers.Facebook))
            {
                var appToken = ConfigurationManager.AppSettings["FacebookAppToken"];
                verifyTokenEndPoint = string.Format(AuthConstants.Providers.FacebookVerifyTokenEndPoint, accessToken, appToken);
            }
            else if (string.Equals(provider, AuthConstants.Providers.Google))
            {
                verifyTokenEndPoint = string.Format(AuthConstants.Providers.GoogleVerifyTokenEndPoint, accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ParsedExternalAccessToken();

                if (string.Equals(provider, AuthConstants.Providers.Facebook))
                {
                    parsedToken.user_id = jObj["data"]["user_id"];
                    parsedToken.app_id = jObj["data"]["app_id"];

                    if (!string.Equals(Startup.FacebookAuthOptions.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
            }

            return parsedToken;
        }

        private async Task<JObject> GenerateLocalAccessTokenResponse(string email, string clientId)
        {
            var tokenExpiration = TimeSpan.FromHours(6);

            var user = _iUserBus.FindUserByEmail(email);
            var userRoles = await _iAuthorizationBus.UserRoles(user.Id);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, email));
            identity.AddClaim(new Claim(AuthConstants.CustomClaims.UserId, user.Id));

            foreach (var role in userRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var client = _iAuthorizationBus.FindClient(clientId);

            if (client == null)
            {
                throw new ForbiddenException(string.Format(Resources.ClientIdNotRegistred, clientId));
            }

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            AuthenticationTokenCreateContext context = new AuthenticationTokenCreateContext(Request.GetOwinContext(), Startup.OAuthServerOptions.RefreshTokenFormat, ticket);
            context.Ticket.Properties.Dictionary.Add("as:client_id", client.Id);

            context.OwinContext.Set("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            await Startup.OAuthServerOptions.RefreshTokenProvider.CreateAsync(context);;

            JObject tokenResponse = new JObject(new JProperty(AuthConstants.CustomAuthProps.UserName, email),
                                                new JProperty("access_token", accessToken),
                                                new JProperty("refresh_token", context.Token),
                                                new JProperty("token_type", "bearer"),
                                                new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                                new JProperty(".issued", ticket.Properties.IssuedUtc.Value.ToUniversalTime().ToString("o")),
                                                new JProperty(".expires", DateTime.UtcNow.Add(tokenExpiration).ToUniversalTime().ToString("o")),
                                                new JProperty(AuthConstants.CustomAuthProps.UserId, user.Id),
                                                new JProperty(AuthConstants.CustomAuthProps.FullName, user.FirstName),
                                                new JProperty(AuthConstants.CustomAuthProps.ProfilePic, user.ImagePath),
                                                new JProperty(AuthConstants.CustomAuthProps.IsAdmin, (userRoles.Contains(ApplicationConstants.ADMIN)) ? "true" : "false"));

            return tokenResponse;
        }

        private async Task<IHttpActionResult> RegisterExternalUser(ExternalLoginData externalLogin, string redirectUri)
        {
            var user = _iUserBus.FindUserByEmail(externalLogin.Email);

            if (user != null)
            {
                redirectUri = CreateRedirectUri(redirectUri, true, "", "", true, externalLogin.Email);

                return Redirect(redirectUri);
            }

            var createdUser = await _iAuthorizationBus.CreateExternalUser(FromExternalDataToRegisterBindingModel(externalLogin));

            var info = new ExternalLoginInfo()
            {
                DefaultUserName = externalLogin.UserName,
                Login = new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey)
            };

            var result = await _iAuthorizationBus.AddLoginAsync(createdUser.Id, info.Login);

            if (!result.Succeeded)
            {
                throw new Exception(Resources.CannotSaveExternalLogin);
            }

            redirectUri = CreateRedirectUri(redirectUri, true, externalLogin.ExternalAccessToken, externalLogin.LoginProvider, false, "");

            return Redirect(redirectUri);
        }

        private RegisterExternalBindingModel FromExternalDataToRegisterBindingModel(ExternalLoginData externalLogin)
        {
            if (externalLogin == null)
            {
                return null;
            }

            return new RegisterExternalBindingModel
            {
                Email = externalLogin.Email,
                ExternalAccessToken = externalLogin.ExternalAccessToken,
                FirstName = externalLogin.FirstName,
                ImageUrl = externalLogin.ImageUrl,
                LastName = externalLogin.LastName,
                Provider = externalLogin.LoginProvider,
                UserName = externalLogin.Email
            };
        }

        private string CreateRedirectUri(string redirectUri, bool firstReg, string externalToken, string provider, bool externalEmailAlreadyRegistred, string localEmail)
        {
            return string.Format("{0}?first_reg={1}&external_token={2}&provider={3}&haslocalaccount={4}&email={5}",
                                 redirectUri,
                                 firstReg.ToString(),
                                 externalToken,
                                 provider,
                                 externalEmailAlreadyRegistred.ToString(),
                                 localEmail);
        }

        #endregion
    }
}
