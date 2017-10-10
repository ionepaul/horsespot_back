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
using HorseSpot.Api.Utils;
using HorseSpot.BLL.Interfaces;
using HorseSpot.Infrastructure.Exceptions;
using HorseSpot.Infrastructure.Resources;
using HorseSpot.Models.Enums;
using HorseSpot.Models.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        [AllowAnonymous]
        [Route("api/account/obtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {

            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
            {
                throw new ValidationException("Provider or external access token is not sent");
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalAccessToken);

            if (verifiedAccessToken == null)
            {
                throw new ValidationException("Invalid Provider or External Access Token");
            }

            var user = await _iAuthorizationBus.FindUserByLoginInfo(new UserLoginInfo(provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                throw new ConflictException("External user is not registered");
            }

            var accessTokenResponse = await GenerateLocalAccessTokenResponse(user.Email);

            return Ok(accessTokenResponse);
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

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, Resources.PleaseUpdateAtLeastOneImage);
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
                throw new ValidationException("Invalid external login request.");
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
                var externalBinding = new RegisterExternalBindingModel
                {
                    FirstName = externalLogin.FirstName,
                    Email = externalLogin.Email,
                    LastName = externalLogin.LastName,
                    ImageUrl = externalLogin.ImageUrl,
                    UserName = externalLogin.UserName
                };

                var createdUser = await _iAuthorizationBus.CreateExternalUser(externalBinding);

                var verifiedAccessToken = await VerifyExternalAccessToken(externalLogin.LoginProvider, externalLogin.ExternalAccessToken);

                var info = new ExternalLoginInfo()
                {
                    DefaultUserName = externalLogin.UserName,
                    Login = new UserLoginInfo(externalLogin.LoginProvider, verifiedAccessToken.app_id)
                };

                var result = await _iAuthorizationBus.AddLoginAsync(createdUser.Id, info.Login);

                redirectUri = string.Format("{0}?firstRegistration={1}&q={2}",
                                           redirectUri,
                                           true,
                                           createdUser.Id);

                return Redirect(redirectUri);
            }

            redirectUri = string.Format("{0}?haslocalaccount={1}&q={2}",
                                            redirectUri,
                                            false,
                                            externalLogin.Email);

            return Redirect(redirectUri);

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/account/RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                throw new ValidationException("Invalid register external binding model.");
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);

            if (verifiedAccessToken == null)
            {
                throw new ValidationException("Invalid Provider or External Access Token");
            }

            var user = await _iAuthorizationBus.FindUserByLoginInfo(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                throw new ConflictException("External user is already registered");
            }
            
            user = await _iAuthorizationBus.CreateExternalUser(model);

            if (user == null)
            {
                throw new Exception("Cannot create user from external provider.");
            }

            var info = new ExternalLoginInfo()
            {
                DefaultUserName = model.UserName,
                Login = new UserLoginInfo(model.Provider, verifiedAccessToken.app_id)
            };

            var result = await _iAuthorizationBus.AddLoginAsync(user.Id, info.Login);

            if (!result.Succeeded)
            {
                throw new Exception("Cannot save external login.");
            }

            var accessTokenResponse = await GenerateLocalAccessTokenResponse(model.Email);

            return Ok(accessTokenResponse);
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
                throw new ValidationException("redirect_uri is required");
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                throw new ValidationException("redirect_uri is invalid");
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ValidationException("client_Id is required");
            }

            var client = _iAuthorizationBus.FindClient(clientId);

            if (client == null)
            {
                throw new ValidationException(string.Format("Client_id '{0}' is not registered in the system.", clientId));
            }

            if (!string.Equals(client.AllowedOrigin, "*") && !string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException(string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId));
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
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook

                var appToken = "xxx";
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
            }
            else if (provider == "Google")
            {
                verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
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

                if (provider == "Facebook")
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

        private async Task<JObject> GenerateLocalAccessTokenResponse(string email)
        {
            var tokenExpiration = TimeSpan.FromHours(6);
            var user = _iUserBus.FindUserByEmail(email);
            var userRoles = await _iAuthorizationBus.UserRoles(user.Id);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, email));
            identity.AddClaim(new Claim("UserId", user.Id));

            foreach (var role in userRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(new JProperty("username", email),
                                                new JProperty("access_token", accessToken),
                                                new JProperty("token_type", "bearer"),
                                                new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                                new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                                new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString()),
                                                new JProperty("userId", user.Id),
                                                new JProperty("firstName", user.FirstName + " " + user.LastName),
                                                new JProperty("profilePic", user.ImagePath),
                                                new JProperty("isAdmin", (userRoles.Contains("Admin")) ? "true" : "false"));

            return tokenResponse;
        }

        #endregion
    }
}
