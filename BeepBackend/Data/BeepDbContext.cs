using BeepBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeepBackend.Data
{
    public class BeepDbContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole,
        IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public BeepDbContext(DbContextOptions<BeepDbContext> options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleUserSetting> ArticleUserSettings { get; set; }
        public DbSet<ArticleGroup> ArticleGroups { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<UserArticle> UserArticles { get; set; }
        public DbSet<BeepEnvironment> Environments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArticleUserSetting>(artSettings =>
            {
                artSettings.HasOne(us => us.Article)
                    .WithMany(a => a.ArticleUserSettings)
                    .HasForeignKey(us => us.ArticleFk);

                artSettings.HasOne(us => us.Environment)
                    .WithMany(e => e.ArticleUserSettings);
            });

            modelBuilder.Entity<ArticleGroup>(artGrp =>
            {
                artGrp.HasMany(ag => ag.Articles)
                    .WithOne(a => a.ArticleGroup)
                    .HasForeignKey(a => a.ArticleGroupFk);
            });

            modelBuilder.Entity<ArticleStore>(artStore =>
            {
                artStore.HasKey(ast => new { ast.ArticleId, ast.StoreId });

                artStore.HasOne(ast => ast.Article)
                    .WithMany(a => a.Stores)
                    .HasForeignKey(ast => ast.ArticleId);

                artStore.HasOne(ast => ast.Store)
                    .WithMany(s => s.Articles)
                    .HasForeignKey(ast => ast.StoreId);
            });

            modelBuilder.Entity<UserArticle>(usrArt =>
            {
                usrArt.HasKey(ua => new { ua.UserId, ua.ArticleId });

                usrArt.HasOne(ua => ua.User)
                    .WithMany(u => u.UserArticles)
                    .HasForeignKey(ua => ua.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                usrArt.HasOne(ua => ua.Article)
                    .WithMany(a => a.UserArticles)
                    .HasForeignKey(ua => ua.ArticleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(user =>
            {
                user.HasMany(u => u.Permissions)
                    .WithOne(p => p.User)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserRole>(urole =>
            {
                urole.HasKey(ur => new { ur.UserId, ur.RoleId });

                urole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                urole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            modelBuilder.Entity<Invitation>(inv =>
            {
                inv.HasKey(i => new { i.InviteeId, i.EnvironmentId });

                inv.HasOne(i => i.Invitee)
                    .WithMany(u => u.InvitedFrom)
                    .HasForeignKey(i => i.InviteeId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                inv.HasOne(i => i.Environment)
                    .WithMany(e => e.Invitations)
                    .HasForeignKey(i => i.EnvironmentId)
                    .IsRequired();
            });

            modelBuilder.Entity<Permission>(per =>
            {
                per.HasKey(p => new { p.UserId, p.EnvironmentId });

                per.HasOne(p => p.Environment)
                    .WithMany(be => be.Permissions)
                    .HasForeignKey(p => p.EnvironmentId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                per.HasOne(p => p.User)
                    .WithMany(u => u.Permissions)
                    .HasForeignKey(p => p.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
