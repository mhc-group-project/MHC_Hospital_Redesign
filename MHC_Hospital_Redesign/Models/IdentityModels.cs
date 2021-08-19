using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MHC_Hospital_Redesign.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        // add FirstName, LastName, ContactMethod and Address columns
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactMethod { get; set; }
        public string Address { get; set; }

        // add a M-M relationship with Listings
        public ICollection<Listing> Listings { get; set; }
        public ICollection<Department> Departments { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    public class ApplicationUserDto
    {
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
       /* public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
       *?

        /* AWS Connection*/
        public ApplicationDbContext()
           : base(AWSConnector.GetRDSConnectionString())
        {

        }

        public DbSet<Ecard> Ecards { get; set; }
        public DbSet<Template> Templates { get; set; }

        public DbSet<Faq> Faqs { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<FaqCategory> FaqCategories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FeedbackCategory> FeedbackCategories { get; set; }

        //add Listing entity to the system
        public DbSet<Listing> Listings { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Payment> Payments { get; set; }

        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
