using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SwishIdentity.Data.Models;

namespace SwishIdentity.Data
{
    public class SwishUser : IdentityUser
    {
        public virtual ManagerModel Manager { get; set; }
        public virtual ProfileModel Profile { get; set; }
    }
    
    public class SwishDbContext : IdentityDbContext
    {
        public SwishDbContext(DbContextOptions<SwishDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<ProfileModel> Profiles { get; set; }
        public DbSet<ManagerModel> Managers { get; set; }
        public DbSet<ProfileHistoryModel> ProfileHistories { get; set; }
        
        public DbSet<PiiClaimModel> PiiClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SwishUser>()
                .Property(p => p.Id)
                .HasColumnName("UserId");
                
        }
    }
}