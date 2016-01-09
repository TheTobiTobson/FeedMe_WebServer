using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using WebServer.Models;

// E-Mail//
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;

namespace WebServer
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            manager.EmailService = new EmailServics();

            //UserManager can take way more options - T
            // manager.UserLockoutEnabledByDefault...
            // manager.MaxFailedAccessAttemptsBeforeLockout...

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }


        // Email Service //
        public class EmailServics : IIdentityMessageService
        {
            public async Task SendAsync(IdentityMessage message)
            {
                await configureEMail(message);
            }

            private async Task configureEMail(IdentityMessage message)
            {
                MailMessage mailMsg = new MailMessage();

                //to (Alles geht zu Testzwecken an t.rocket@... )
                //mailMsg.To.Add(message.Destination);              
                mailMsg.To.Add(new MailAddress("t.rocket@web.de", "TobiRakete"));            

                //from
                mailMsg.From = new MailAddress("tobitobinc@gmail.com", "TheProject");

                // Subject and multipart/alternative Body
                mailMsg.Subject = message.Subject;
                string text = message.Body;
                string html = message.Body;
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("tobitobinc@gmail.com", ")!8291IdoU=0");
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                                
                smtpClient.SendAsync(mailMsg, null);
            }
        }



    }
}
