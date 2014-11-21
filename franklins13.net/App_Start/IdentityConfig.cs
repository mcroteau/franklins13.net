using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using franklins13.net.Models;
using franklins13.net.Common;

namespace IdentitySample.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {


        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }


        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
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
                RequiredLength = 5,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = false;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }



    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole,string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }


    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext> 
    {
        
        private const string MOCK_USERNAME1 = "mock123";
        private const string MOCK_USERNAME2 = "mock456";

        protected override void Seed(ApplicationDbContext context) {
            InitializeIdentityForEF(context);
            InitializeMockUsers(context);
            InitializeMockEntries(context);
            base.Seed(context);
        }


        private void InitializeMockUsers(ApplicationDbContext db)
        {
            CheckCreateMockUser(MOCK_USERNAME1);
            CheckCreateMockUser(MOCK_USERNAME2);
        }


        private void CheckCreateMockUser(string username)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var mockUser = userManager.FindByName(username);
            if (mockUser == null)
            {
                mockUser = new ApplicationUser { UserName = username, Email = username + "@email.com" };
                var result = userManager.Create(mockUser, username);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create ApplicationUser");
                }
            }
        }



        public static void InitializeMockEntries(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var mockUser = userManager.FindByName(MOCK_USERNAME1);

            var entry = new Entry();
            var today = DateTime.Now;

            for (int m = 0; m < 30; m++)
            {
                Random random = new Random();
                int temperance = random.Next(-10, 10);
                int silence = random.Next(-10, 10);
                int order = random.Next(-10, 10);
                int resolution = random.Next(-10, 10);
                int frugality = random.Next(-10, 10);
                int industry = random.Next(-10, 10);
                int sincerity = random.Next(-10, 10);
                int justice = random.Next(-10, 10);
                int moderation = random.Next(-10, 10);
                int cleanliness = random.Next(-10, 10);
                int tranquility = random.Next(-10, 10);
                int chastity = random.Next(-10, 10);
                int humility = random.Next(-10, 10);

                entry.EntryDate = today.AddDays(-m);

                entry.Temperance = temperance;
                entry.Silence = silence;
                entry.Order = order;
                entry.Resolution = resolution;
                entry.Frugality = frugality;
                entry.Industry = industry;
                entry.Sincerity = sincerity;
                entry.Justice = justice;
                entry.Moderation = moderation;
                entry.Cleanliness = cleanliness;
                entry.Tranquility = tranquility;
                entry.Chastity = chastity;
                entry.Humility = humility;


                int total = entry.Temperance + entry.Silence + entry.Order
                    + entry.Resolution + entry.Frugality + entry.Industry
                    + entry.Sincerity + entry.Justice + entry.Moderation
                    + entry.Cleanliness + entry.Tranquility + entry.Chastity
                    + entry.Humility;

                entry.Total = total;

                entry.UserID = mockUser.Id;

                db.Entries.Add(entry);
                db.SaveChanges();


                var permission = new AccountPermission();
                permission.UserID = mockUser.Id;
                permission.Permission = ApplicationConstants.EDIT_ENTRY_PERMISSION + entry.Id;
                db.AccountPermissions.Add(permission);               
                db.SaveChanges();
            }
        }


        private int TallyTotal(Entry entry){
            return 0;
        }



  
        public static void InitializeIdentityForEF(ApplicationDbContext db) {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "admin@example.com";
            const string password = "admin123";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null) {
                role = new IdentityRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null) {
                user = new ApplicationUser { UserName = name, Email = name };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name)) {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }



    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : 
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}