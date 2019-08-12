using BeepBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BeepBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleUserSetting> ArticleUserSettings { get; set; }
        public DbSet<ArticleGroup> ArticleGroups { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserArticle> UserArticles { get; set; }
        public DbSet<BeepEnvironment> Environments { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleUserSetting>()
                .HasOne(us => us.Article)
                .WithMany(a => a.ArticleUserSettings)
                .HasForeignKey(us => us.ArticleFk);

            modelBuilder.Entity<ArticleUserSetting>()
                .HasOne(us => us.Environment)
                .WithMany(e => e.ArticleUserSettings);

            modelBuilder.Entity<ArticleGroup>()
                .HasMany(ag => ag.Articles)
                .WithOne(a => a.ArticleGroup)
                .HasForeignKey(a => a.ArticleGroupFk);


            modelBuilder.Entity<ArticleStore>()
                .HasKey(ast => new { ast.ArticleId, ast.StoreId });

            modelBuilder.Entity<ArticleStore>()
                .HasOne(ast => ast.Article)
                .WithMany(a => a.Stores)
                .HasForeignKey(ast => ast.ArticleId);

            modelBuilder.Entity<ArticleStore>()
                .HasOne(ast => ast.Store)
                .WithMany(s => s.Articles)
                .HasForeignKey(ast => ast.StoreId);


            modelBuilder.Entity<UserArticle>()
                .HasKey(ua => new { ua.UserId, ua.ArticleId });

            modelBuilder.Entity<UserArticle>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserArticles)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserArticle>()
                .HasOne(ua => ua.Article)
                .WithMany(a => a.UserArticles)
                .HasForeignKey(ua => ua.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<User>()
                .HasMany(u => u.Permissions)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
