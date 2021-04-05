using Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Repository
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, 
                                            IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>,
                                            IdentityUserToken<string>>

    { 
        public DbSet<ApplicationUser> AppUsers { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        public ApplicationDbContext()
        { }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<ApplicationRole>()
                .HasMany(u => u.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<UserLike>().
                HasKey(k => new { k.UserLikingId, k.UserBeingLikedId });

            builder.Entity<UserLike>().
                HasOne(s => s.UserLiking).
                WithMany(l => l.LikedUsers).
                HasForeignKey(s => s.UserLikingId).
                OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserLike>().
                HasOne(s => s.UserBeingLiked).
                WithMany(l => l.LikedByUsers).
                HasForeignKey(s => s.UserBeingLikedId).
                OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Message>().
                HasOne(s => s.Recipient).
                WithMany(l => l.MessagesReceived).
                OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>().
                HasOne(s => s.Sender).
                WithMany(l => l.MessagesSent).
                OnDelete(DeleteBehavior.Restrict);
        }
    }
}
