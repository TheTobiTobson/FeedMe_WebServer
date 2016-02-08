using System;
// Entity State (Modified,...)
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
//HttpStatusCode
using System.Net;
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
        // Authorization Required: YES
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
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

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
        // Authorization Required: YES
        // Parameter: Primary key of Feedbacksession (int FBS_id)
        // Result: List of Questions
        // Description:
        //     API is called when user requestes all questions of a Feedbacksession
        //////////////////////////
                
        [Route("~/api/Feedbackquestions/{FBS_id:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get_FeedbackQuestions(int FBS_id)
        {
            // Get user id
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

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

            //var list_of_FeedbackQuestions = (from x in db.QUE_FeedbackQuestions
            //                                 where x.QUE_FBS_id == FBS_id                                             
            //                                 select x).ToList<QUE_FeedbackQuestions>();

            var list_of_FeedbackQuestions = from x in db.QUE_FeedbackQuestions
                                             where x.QUE_FBS_id == FBS_id
                                             select new QUE_FeedbackQuestionsDTO()
                                             { QUE_id = x.QUE_id,
                                               QUE_position = x.QUE_position,
                                               QUE_text = x.QUE_text,
                                               QUE_answerRadioButton = x.QUE_answerRadioButton,
                                               QUE_title = x.QUE_title,
                                               QUE_type = x.QUE_type,
                                               QUE_showQuestionInFeedback = x.QUE_showQuestionInFeedback,
                                               QUE_FBS_id = x.QUE_FBS_id,
                                               QUE_creationDate = x.QUE_creationDate
                                             };



            if (!list_of_FeedbackQuestions.Any())
            {
                return NotFound();
            }

            return Ok(list_of_FeedbackQuestions);
        }

        //////////////////////////
        // Url:.../api/Feedbackquestion/{QUE_id:int}
        // Method: PUT
        // Authorization Required: YES
        // Parameter: Primary key of Feedbackquestion (int QUE_id)
        // Result: HTTP 200 (ok), HTTP 400(Bad Request), HTTP 404(Not Found)
        // Description:
        //     API updates the Feedbackquestion specified in QUE_id
        //////////////////////////

        [Route("~/api/Feedbackquestion")]
        [HttpPut]
        public async Task<IHttpActionResult> Update_FeedbackQuestion(QUE_FeedbackQuestions qUE_FeedbackQuestions)
        {

            // Get user id
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());          

            if (qUE_FeedbackQuestions == null)
            {
                return BadRequest("Requsts is missing a question model ");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Get question that is supposed to get updated
            var QuestionToGetUpdated = await db.QUE_FeedbackQuestions.AsNoTracking()
                .Include(b => b.FBS_FeedbackSessions)
                .SingleOrDefaultAsync(x => x.QUE_id == qUE_FeedbackQuestions.QUE_id);
              
            // Check if Question exists //
            if(QuestionToGetUpdated == null)
            {
                return NotFound();
            }

            // Check if question is part of a feedbacksession that is owned by this user //
            if (QuestionToGetUpdated.FBS_FeedbackSessions.FBS_ApplicationUser_Id != user.Id)
            {
                return BadRequest("Requested Feedbackquestion is not owned by this user");
            }

            // Check if FBS is manipulated //
            if (QuestionToGetUpdated.QUE_FBS_id != qUE_FeedbackQuestions.QUE_FBS_id)
            {
                return BadRequest("Seems like the Session ID is wrong");
            }

            // Client sends no information regarding to which FBS the question belongs //
            // This has to be added serverside //
            //qUE_FeedbackQuestions.QUE_FBS_id = QuestionToGetUpdated.QUE_FBS_id;

            // When you change the state to Modified all the properties of the entity will be marked 
            // as modified and all the property values will be sent to the database when SaveChanges is called. 
            db.Entry(qUE_FeedbackQuestions).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QUE_FeedbackQuestionsExists(qUE_FeedbackQuestions.QUE_id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /*** Code for Account API ***/
        
        //////////////////////////
        // Url:.../api/Account/Register
        // Method: POST
        // Authorization Required: NO
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

        //////////////////////////
        // Url:.../api/Account/AccountConfirmation
        // Method: POST
        // Authorization Required: NO
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
        // Url:.../api/Account/ForgotPassword
        // Method: POST
        // Authorization Required: NO
        // Parameter: ForgotPasswordViewModel (Email)
        // Result: HTTP 200 (ok), HTTP 400(Bad Request)
        // Description:
        //     API is called when user wants to reset password
        //////////////////////////

        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user based on email address //
            ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null || !(user.EmailConfirmed))
            {
                // User was not found or has no confirmed email address //
                // Don't let anyone this > send OK //
                return Ok();
            }

            //*** Send ResetToken ***//
            string ResetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
           
            //Generate URI for E-Mail  
            string uriWithToken = Url.Link("ResetPasswordRoute", null);

            //Message for testing
            await UserManager.SendEmailAsync(user.Id, "Password Reset", "<!DOCTYPE html><html><head><title>Account Confirmation</title></head><body><h1>Welcome to FeedMe</h1><p>UserID:" + user.Id + "</p><p>PW Reset Token:" + ResetToken + "</p></body></html>");
            
            // everything is good //
            return Ok();
        }

          //////////////////////////
        // Url:.../api/Account/ResetPassword
        // Method: POST
        // Authorization Required: NO
        // Parameter: ResetPasswordViewModel (Email, Password, Confirm Password)
        // Result: HTTP 200 (ok), HTTP 400(Bad Request)
        // Description:
        //     API is called when user clicks on link in password reset email
        //////////////////////////
        
        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword", Name = "ResetPasswordRoute")]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(model.userID);
          //  UserManager.find

            if (user == null)
            {
                // Specified user does not exist //
                // Don't let anyone this > send OK //
                return Ok();
            }

            //*** Reset Password ***//
            IdentityResult PasswordReset = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            if (!PasswordReset.Succeeded)
            {
                return BadRequest();
            }

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
        // Free ressources utilized by this class //
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        // Check if a QUE_Feedbacksession is existing //
        private bool QUE_FeedbackQuestionsExists(int id)
        {
            return db.QUE_FeedbackQuestions.Count(e => e.QUE_id == id) > 0;
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
