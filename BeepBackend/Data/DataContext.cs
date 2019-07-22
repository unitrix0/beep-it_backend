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
        public DbSet<Environment> Environments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleUserSetting>()
                .HasOne(us => us.Article)
                .WithMany(a => a.ArticleUserSettings)
                .HasForeignKey(us => us.ArticleFk);

            modelBuilder.Entity<ArticleUserSetting>()
                .HasOne(us => us.Environment)
                .WithMany(e => e.ArticleUserSettings)
                .HasForeignKey(us => us.EnvironmentFk);


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


            modelBuilder.Entity<UserEnvironment>()
                .HasKey(ue => new { ue.EnvironmentId, ue.UserId });

            modelBuilder.Entity<UserEnvironment>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.Environments)
                .HasForeignKey(ue => ue.UserId);

            modelBuilder.Entity<UserEnvironment>()
                .HasOne(ue => ue.Environment)
                .WithMany(e => e.Users)
                .HasForeignKey(ue => ue.EnvironmentId);


            modelBuilder.Entity<UserArticle>()
                .HasKey(ua => new {ua.ArticleId, ua.UserId});

            modelBuilder.Entity<UserArticle>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserArticles)
                .HasForeignKey(ua => ua.UserId);

            modelBuilder.Entity<UserArticle>()
                .HasOne(ua => ua.Article)
                .WithMany(a => a.UserArticles)
                .HasForeignKey(ua => ua.ArticleId);
        }
    }
}
