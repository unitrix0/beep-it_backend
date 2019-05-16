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
        //public DbSet<ArticleGroup> ArticleGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleUserSetting>()
                .HasOne(us => us.Article)
                .WithMany(a => a.ArticleUserSettings)
                .HasForeignKey(us => us.ArticleFk);
        }
    }
}
