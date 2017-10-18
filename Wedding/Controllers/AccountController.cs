using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Wedding.Models;
using Domain.Models;
using RazorEngine.Templating;
using RazorEngine;
using System.IO;
using Facebook;
using System.Web.UI;

namespace Wedding.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]        
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "returnUrl")]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.HtmlSchema = "";

            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null && UserManager.CheckPassword(user, model.Password))
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Verify your email");

                    ViewBag.errorMessage = "Check your email & Confirm your account to login.";
                    ViewBag.HtmlSchema = "";

                    return View("Error");
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid username or password!");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]        
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]        
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult Register()
        {
            if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result;
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    result = await UserManager.AddPasswordAsync(user.Id, model.Password);
                }
                else
                {
                    user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    result = await UserManager.CreateAsync(user, model.Password);
                }

                if (result.Succeeded)
                {
                    UserManager.AddClaim(user.Id, new Claim(ClaimTypes.Role, "User"));

                    //  Comment the following line to prevent log in until the user is confirmed.
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Verify your email");

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your ZenZoy account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    ViewBag.Message = "Check your email & confirm your account to login";
                    ViewBag.HtmlSchema = "";

                    return View("Info");

                    //return RedirectToAction("Index", "Home", new { area = "" });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]        
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                if (user == null)
                {
                    //ModelState.AddModelError("Email", "No account with this email exists on ZenZoy!");

                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, , "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views/EmailTemplates", "ResetPassword.cshtml");
                var template = System.IO.File.ReadAllText(templatePath);
                var emailModel = new RegisterEmail(callbackUrl);
                var guid = Guid.NewGuid().ToString();
                string emailHtmlBody = Engine.Razor.RunCompile(template, guid, null, emailModel);

                await UserManager.SendEmailAsync(user.Id, "Reset Password", emailHtmlBody);

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]        
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]        
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]        
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]        
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]        
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    ViewBag.HtmlSchema = "";

                    var loginResult = await CreateAccountForUserWhenExternalLogin(loginInfo.Email);
                    if (loginResult)
                        return RedirectToAction("Index", "Home");
                    else
                        return View("ExternalLoginFailure");
            }
        }

        private async Task<bool> CreateAccountForUserWhenExternalLogin(string email)
        {
            if (User.Identity.IsAuthenticated)
            {
                return true;
            }

            // Get the information about the user from the external login provider
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return false;
            }

            ApplicationUser user;
            if (info.Login.LoginProvider == "Google" || info.Login.LoginProvider == "google")
            {
                user = await UserManager.FindByEmailAsync(email);
                if (user != null)
                {
                    await UserManager.AddLoginAsync(user.Id, info.Login);
                    this.Session["UserFirstName"] = user.FirstName;
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return true;
                }

                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = info.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName).Value.ToString(),
                    LastName = info.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname).Value.ToString()
                };
            }
            else if (info.Login.LoginProvider == "Facebook" || info.Login.LoginProvider == "facebook")
            {
                var nameStr = info.ExternalIdentity.GetUserName();
                var nameStrL = nameStr.Split(new char[1] { ' ' }, 2);

                var identity = AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
                var access_token = identity.FindFirstValue("FacebookAccessToken");
                var fb = new FacebookClient(access_token);
                dynamic myInfo = fb.Get("/me?fields=email"); // specify the email field

                user = await UserManager.FindByEmailAsync(myInfo.email);
                if (user != null)
                {
                    await UserManager.AddLoginAsync(user.Id, info.Login);
                    this.Session["UserFirstName"] = nameStrL.Length > 0 ? nameStrL[0] : user.FirstName;
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return true;
                }

                user = new ApplicationUser
                {
                    UserName = myInfo.email,
                    Email = myInfo.email,
                    FirstName = nameStrL.Length > 0 ? nameStrL[0] : "",
                    LastName = nameStrL.Length > 1 ? nameStrL[1] : ""
                };
            }
            else if (info.Login.LoginProvider == "Twitter" || info.Login.LoginProvider == "twitter")
            {
                user = new ApplicationUser
                {
                    UserName = Guid.NewGuid().ToString(),
                    Email = email,
                    FirstName = info.ExternalIdentity.Name,
                    LastName = ""
                };
            }
            else
            {
                return false;
            }

            var result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await UserManager.AddLoginAsync(user.Id, info.Login);
                if (result.Succeeded)
                {
                    this.Session["UserFirstName"] = user.FirstName;
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return true;
                }
                return false;
            }
            else
            {
                AddErrors(result);
                return false;
            }
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public ActionResult LogOff()
        {
            Session["CurrentTermSearched"] = null;
            Session["CurrentPage"] = null;
            Session["UserFirstName"] = null;

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]        
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userID, code = code }, protocol: Request.Url.Scheme);

            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views/EmailTemplates", "RegisterEmail.cshtml");
            var template = System.IO.File.ReadAllText(templatePath);
            var emailModel = new RegisterEmail(callbackUrl);
            var guid = Guid.NewGuid().ToString();
            string emailHtmlBody = Engine.Razor.RunCompile(template, guid, null, emailModel);

            await UserManager.SendEmailAsync(userID, subject, emailHtmlBody);

            return callbackUrl;
        }
        #endregion
    }
}