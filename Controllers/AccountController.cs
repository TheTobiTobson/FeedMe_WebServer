using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
// Attribute Routing
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebServer.Models;
using WebServer.Providers;
using WebServer.Results;
// ResponseTypeAttribute
using System.Web.Http.Description;

// Linq Queries
using System.Linq;


namespace WebServer.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        /*** General Account Controller Code ***/
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        
        // DbContext to access database
        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager feedMeUserManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = feedMeUserManager;
            AccessTokenFormat = accessTokenFormat;
        }

        // Accessor: Defines how to read and write UserManager //
        public ApplicationUserManager UserManager
        {
            get
            {
                // Der Operator ?? wird NULL-Sammeloperator genannt. 
                // Der linke Operand wird zurückgegeben, falls dieser nicht NULL ist. 
                // Andernfalls wird der rechte Operand zurückgegeben.
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        
        /****** FeedMe Specific API ******/
        
        
        //////////////////////////
        // Url:.../api/Feedbacksession
        // Method: GET
        // Parameter: none
        // Result: List of Feedbacksessions
        // Description:
        //     API is called when user requests Feedbacksession overview
        //////////////////////////
        
        [Route("~/api/Feedbacksession")]
        [HttpGet]
        public async Task<IHttpActionResult> Get_FeedbackSessions()
        {
            // Get user id
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var list_of_FeedbackSessions = (from x in db.FBS_FeedbackSessions
                                                where x.FBS_ApplicationUser_Id == user.Id
                                                select x).ToList<FBS_FeedbackSessions>();


            if (!list_of_FeedbackSessions.Any())
            {
                return NotFound();
            }

            return Ok(list_of_FeedbackSessions);
        }



        //////////////////////////
        // Url:.../api/Feedbackquestions/{FBS_id:int}
        // Method: GET
        // Parameter: Primary key of Feedbacksession (int FBS_id)
        // Result: List of Questions
        // Description:
        //     API is called when user requestes questions of a Feedbacksession
        //////////////////////////

        [Route("~/api/Feedbackquestions/{FBS_id:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get_FeedbackQuestions(int FBS_id)
        {
            // Get user id
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            // Check if requested Feedbacksession is available
            FBS_FeedbackSessions fBS_FeedbackSessions = await db.FBS_FeedbackSessions.FindAsync(FBS_id);
            if (fBS_FeedbackSessions == null)
            {
                return NotFound();
            }

            // Check if requested Feedbacksession is owned by user
            if (fBS_FeedbackSessions.FBS_ApplicationUser_Id != user.Id)
            {
                return BadRequest("Requested Feedbacksession is not owned by this user");    
            }

            var list_of_FeedbackQuestions = (from x in db.QUE_FeedbackQuestions
                                             where x.QUE_FBS_id == FBS_id                                                                                        
                                             select x).ToList<QUE_FeedbackQuestions>();

            // Hier hab ich vor Mamas besuch gestoppt //

            if (!list_of_FeedbackQuestions.Any())
            {
                return NotFound();
            }

            return Ok(list_of_FeedbackQuestions);
        }
        
        
        /*** Code for Account API ***/


        //////////////////////////
        // Url:.../api/Account/AccountConfirmation
        // Method: POST
        // Parameter: AccountConfirmationModel (userID and Token from email)
        // Result: HTTP 200 (ok), HTTP 400(Bad Request)
        // Description:
        //     API is called when user clicks on Account Confirmation Link
        //     which is sent via email.
        //////////////////////////
    
        [AllowAnonymous]
        [HttpPost]
        [Route("AccountConfirmation", Name = "AccountConfirmationRoute")]
        public async Task<IHttpActionResult> AccountConfirmation(AccountConfirmationModel model)
        {
            //throw new System.NotImplementedException();

            if (!ModelState.IsValid)
            {
                return BadRequest("UserID or Confirmation Token is wrong.");
            }

            IdentityResult result = await UserManager.ConfirmEmailAsync(model.userID, model.ConfirmationToken);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();

        }

        //////////////////////////
        // Url:.../api/Account/Register
        // Method: POST
        // Parameter: RegisterBindingModel (Email, Password, Password Confirmation)
        // Result: HTTP 200 (ok), HTTP 400(Bad Request)
        // Description:
        //     API is called when user wants to register an account
        //////////////////////////
             
        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create the user data structure // 
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            // User Manager gets user to creates a new user. Password is a parameter cause the user manager does password hashing //
            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //** Confirm EMail Address**//
            // Generate Token
            string ConfirmationToken = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            //Generate URI for E-Mail  
            string uriWithToken = Url.Link("AccountConfirmationRoute", null);
            //string uriWithToken = Url.Link()

            // Send E-Mail with UserManager
            //DO NOT DELETE > Message for E-Mail Functionality//
            //await UserManager.SendEmailAsync(user.Id, "AccountConfirmation", "<!DOCTYPE html><html><head><title>Account Confirmation</title></head><body><h1>Welcome to FeedMe</h1><p>Please verify your Account by clicking on following link</p><p><a href=\"" + uriWithToken + "\">CONFIRM ACCOUNT</a></p></body></html>");

            //Message for testing
            await UserManager.SendEmailAsync(user.Id, "AccountConfirmation", "<!DOCTYPE html><html><head><title>Account Confirmation</title></head><body><h1>Welcome to FeedMe</h1><p>UserID:" + user.Id + "</p><p>Token:" + ConfirmationToken + "</p></body></html>");
            //** END - Confirm EMail Address**//


            return Ok();
        }
       


        
        
        
        
        
        
        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                
                 ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

      



        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }

        /*** General Account Controller Code ***/
        // Free ressources utilized by this class - T
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
