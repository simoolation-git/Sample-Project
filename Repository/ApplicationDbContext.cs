using Domain.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Wedding.Repository
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<PostedItem> PostedItems { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Opinion> Opinions { get; set; }

        public DbSet<SearchedTerm> SearchedTerms { get; set; }

        public ApplicationDbContext()
            : base("ApplicationContext", throwIfV1Schema: false)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<PostedItem>().HasOptional(f => f.Photo).WithRequired(s => s.PostedItem);

            base.OnModelCreating(modelBuilder);
        }



        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}