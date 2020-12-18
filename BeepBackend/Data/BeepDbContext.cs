﻿using System.Threading.Tasks;
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
        public DbSet<StockEntry> StockEntries { get; set; }
        public DbSet<ArticleUnit> ArticleUnits { get; set; }
        public DbSet<ArticleStore> ArticleStores { get; set; }
        public DbSet<StockEntryValue> StockEntryValues { get; set; }
        public DbSet<UserCamera> UserCameras { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<ActivityLogEntry> ActivityLogEntries { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<ShoppingListArticleEntry> ShoppingList { get; set; }
        public DbSet<ShoppingListGroupEntry> ShoppingListGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShoppingListArticleEntry>().HasNoKey().ToView("ShoppingList");

            modelBuilder.Entity<ShoppingListGroupEntry>().HasNoKey().ToView("ShoppingListGroups");

            modelBuilder.Entity<Article>(artSettings => { artSettings.Property(x => x.UnitId).HasDefaultValue(1); });

            modelBuilder.Entity<ArticleUserSetting>(artSettings =>
            {
                artSettings.HasOne(us => us.Article)
                    .WithMany(a => a.ArticleUserSettings)
                    .HasForeignKey(us => us.ArticleId);

                artSettings.HasOne(us => us.Environment)
                    .WithMany(e => e.ArticleUserSettings);

                artSettings.Property(ars => ars.ArticleGroupId).HasDefaultValue(1);
            });

            modelBuilder.Entity<ArticleGroup>(artGrp =>
            {
                artGrp.HasKey(ag => ag.Id);

                artGrp.HasMany(ag => ag.ArticleUserSettings)
                    .WithOne(aus => aus.ArticleGroup)
                    .HasForeignKey(aus => aus.ArticleGroupId)
                    .OnDelete(DeleteBehavior.Restrict);

                artGrp.HasOne(ag => ag.User)
                    .WithMany(env => env.ArticleGroups)
                    .HasForeignKey(ag => ag.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                artGrp.HasData(new ArticleGroup() { Id = 1, Name = "keine" });
            });

            modelBuilder.Entity<Store>(store =>
            {
                store.HasData(
                    new Store() { Id = 1, Name = "Migros" },
                    new Store() { Id = 2, Name = "Coop" },
                    new Store() { Id = 3, Name = "Denner" },
                    new Store() { Id = 4, Name = "Aldi" },
                    new Store() { Id = 5, Name = "Spar" },
                    new Store() { Id = 6, Name = "Müller" },
                    new Store() { Id = 7, Name = "Online" }
                    );
            });

            modelBuilder.Entity<ArticleStore>(artStore =>
            {
                artStore.HasKey(ast => new { ast.ArticleId, ast.StoreId });

                artStore.HasOne(ast => ast.Article)
                    .WithMany(a => a.ArticleStores)
                    .HasForeignKey(ast => ast.ArticleId);

                artStore.HasOne(ast => ast.Store)
                    .WithMany(s => s.ArticleStores)
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

            modelBuilder.Entity<RefreshToken>(rToken =>
            {
                rToken.HasKey(rt => rt.Id);

                rToken.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity<StockEntry>(stockEntry =>
            {
                stockEntry.HasKey(se => new { se.EnvironmentId, se.ArticleId });

                stockEntry.HasOne(se => se.Article)
                    .WithMany(a => a.StockEntries)
                    .HasForeignKey(se => se.ArticleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                stockEntry.HasOne(se => se.Environment)
                    .WithMany(be => be.StockEntries)
                    .HasForeignKey(se => se.EnvironmentId)
                    .IsRequired();
            });

            modelBuilder.Entity<ArticleUnit>(unit =>
            {
                unit.HasKey(u => u.Id);

                unit.HasMany(u => u.Articles)
                    .WithOne(a => a.Unit)
                    .HasForeignKey(a => a.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);

                unit.HasData(
                    new ArticleUnit() { Id = 1, Abbreviation = "Stk.", Name = "Stück" },
                    new ArticleUnit() { Id = 2, Abbreviation = "l", Name = "Liter" },
                    new ArticleUnit() { Id = 3, Abbreviation = "dl", Name = "Deziliter" },
                    new ArticleUnit() { Id = 4, Abbreviation = "cl", Name = "Centiliter" },
                    new ArticleUnit() { Id = 5, Abbreviation = "g", Name = "Gramm" },
                    new ArticleUnit() { Id = 6, Abbreviation = "kg", Name = "Kilogramm" },
                    new ArticleUnit() { Id = 7, Abbreviation = "ml", Name = "Milliliter" }
                    );
            });

            modelBuilder.Entity<StockEntryValue>(values =>
            {
                values.HasKey(v => v.Id);

                values.HasOne(v => v.StockEntry)
                    .WithMany(se => se.StockEntryValues)
                    .HasForeignKey(v => new { v.EnvironmentId, v.ArticleId })
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserCamera>(userCams =>
            {
                userCams.HasKey(uc => new { uc.UserId, uc.CameraId });

                userCams.HasOne(uc => uc.User)
                    .WithMany(u => u.Cameras)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                userCams.HasOne(uc => uc.Camera)
                    .WithMany(c => c.Users)
                    .HasForeignKey(uc => uc.CameraId)
                    .IsRequired();
            });

            modelBuilder.Entity<ActivityLogEntry>(logEntries =>
            {
                logEntries.HasKey(ale => ale.Id);

                logEntries.HasOne(ale => ale.Environment)
                    .WithMany(e => e.ActivityLogEntries)
                    .HasForeignKey(ale => ale.EnvironmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public async Task<bool> CreateDemoDataForUser(int userId)
        {
            int count = await this.Database.ExecuteSqlCommandAsync("EXEC CreateDemoDataForUser @p0", parameters: userId.ToString());

            return count > 0;
        }
    }
}
