﻿// <auto-generated />
using System;
using BeepBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeepBackend.Migrations
{
    [DbContext(typeof(BeepDbContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BeepBackend.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ArticleGroupFk");

                    b.Property<string>("Barcode");

                    b.Property<int>("ContentAmount");

                    b.Property<bool>("HasLifetime");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("Name");

                    b.Property<int>("UnitId")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.HasKey("Id");

                    b.HasIndex("ArticleGroupFk");

                    b.HasIndex("UnitId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("BeepBackend.Models.ArticleGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ArticleGroups");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "keine"
                        });
                });

            modelBuilder.Entity("BeepBackend.Models.ArticleStore", b =>
                {
                    b.Property<int>("ArticleId");

                    b.Property<int>("StoreId");

                    b.HasKey("ArticleId", "StoreId");

                    b.HasIndex("StoreId");

                    b.ToTable("ArticleStores");
                });

            modelBuilder.Entity("BeepBackend.Models.ArticleUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Abbreviation");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ArticleUnits");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Abbreviation = "Stk.",
                            Name = "Stück"
                        },
                        new
                        {
                            Id = 2,
                            Abbreviation = "l",
                            Name = "Liter"
                        },
                        new
                        {
                            Id = 3,
                            Abbreviation = "dl",
                            Name = "Deziliter"
                        },
                        new
                        {
                            Id = 4,
                            Abbreviation = "cl",
                            Name = "Centiliter"
                        },
                        new
                        {
                            Id = 5,
                            Abbreviation = "g",
                            Name = "Gramm"
                        },
                        new
                        {
                            Id = 6,
                            Abbreviation = "kg",
                            Name = "Kilogramm"
                        });
                });

            modelBuilder.Entity("BeepBackend.Models.ArticleUserSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ArticleFk");

                    b.Property<int>("EnvironmentId");

                    b.Property<int>("KeepStockAmount");

                    b.Property<int>("KeepStockMode");

                    b.Property<long>("UsualLifetime");

                    b.HasKey("Id");

                    b.HasIndex("ArticleFk");

                    b.HasIndex("EnvironmentId");

                    b.ToTable("ArticleUserSettings");
                });

            modelBuilder.Entity("BeepBackend.Models.BeepEnvironment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("DefaultEnvironment");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Environments");
                });

            modelBuilder.Entity("BeepBackend.Models.Invitation", b =>
                {
                    b.Property<int>("InviteeId");

                    b.Property<int>("EnvironmentId");

                    b.Property<DateTime>("AnsweredOn");

                    b.Property<DateTime>("IssuedAt");

                    b.Property<string>("Serial");

                    b.HasKey("InviteeId", "EnvironmentId");

                    b.HasIndex("EnvironmentId");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("BeepBackend.Models.Permission", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("EnvironmentId");

                    b.Property<bool>("CanScan");

                    b.Property<bool>("EditArticleSettings");

                    b.Property<bool>("IsOwner");

                    b.Property<bool>("ManageUsers");

                    b.Property<string>("Serial")
                        .IsRequired();

                    b.HasKey("UserId", "EnvironmentId");

                    b.HasIndex("EnvironmentId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("BeepBackend.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("BeepBackend.Models.StockEntry", b =>
                {
                    b.Property<int>("EnvironmentId");

                    b.Property<int>("ArticleId");

                    b.HasKey("EnvironmentId", "ArticleId");

                    b.HasIndex("ArticleId");

                    b.ToTable("StockEntries");
                });

            modelBuilder.Entity("BeepBackend.Models.StockEntryValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AmountOnStock");

                    b.Property<float>("AmountRemaining");

                    b.Property<int>("ArticleId");

                    b.Property<int>("EnvironmentId");

                    b.Property<DateTime>("ExpireDate");

                    b.Property<bool>("IsOpened");

                    b.Property<DateTime>("OpenedOn");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("EnvironmentId", "ArticleId");

                    b.ToTable("StockEntryValues");
                });

            modelBuilder.Entity("BeepBackend.Models.Store", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Stores");
                });

            modelBuilder.Entity("BeepBackend.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("BeepBackend.Models.UserArticle", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("ArticleId");

                    b.HasKey("UserId", "ArticleId");

                    b.HasIndex("ArticleId");

                    b.ToTable("UserArticles");
                });

            modelBuilder.Entity("BeepBackend.Models.UserRole", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("BeepBackend.Models.Article", b =>
                {
                    b.HasOne("BeepBackend.Models.ArticleGroup", "ArticleGroup")
                        .WithMany("Articles")
                        .HasForeignKey("ArticleGroupFk")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BeepBackend.Models.ArticleUnit", "Unit")
                        .WithMany("Articles")
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("BeepBackend.Models.ArticleStore", b =>
                {
                    b.HasOne("BeepBackend.Models.Article", "Article")
                        .WithMany("Stores")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BeepBackend.Models.Store", "Store")
                        .WithMany("Articles")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BeepBackend.Models.ArticleUserSetting", b =>
                {
                    b.HasOne("BeepBackend.Models.Article", "Article")
                        .WithMany("ArticleUserSettings")
                        .HasForeignKey("ArticleFk")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BeepBackend.Models.BeepEnvironment", "Environment")
                        .WithMany("ArticleUserSettings")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BeepBackend.Models.BeepEnvironment", b =>
                {
                    b.HasOne("BeepBackend.Models.User", "User")
                        .WithMany("Environments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BeepBackend.Models.Invitation", b =>
                {
                    b.HasOne("BeepBackend.Models.BeepEnvironment", "Environment")
                        .WithMany("Invitations")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BeepBackend.Models.User", "Invitee")
                        .WithMany("InvitedFrom")
                        .HasForeignKey("InviteeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("BeepBackend.Models.Permission", b =>
                {
                    b.HasOne("BeepBackend.Models.BeepEnvironment", "Environment")
                        .WithMany("Permissions")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BeepBackend.Models.User", "User")
                        .WithMany("Permissions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("BeepBackend.Models.StockEntry", b =>
                {
                    b.HasOne("BeepBackend.Models.Article", "Article")
                        .WithMany("StockEntries")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("BeepBackend.Models.BeepEnvironment", "Environment")
                        .WithMany("StockEntries")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BeepBackend.Models.StockEntryValue", b =>
                {
                    b.HasOne("BeepBackend.Models.Article", "Article")
                        .WithMany("StockEntryValues")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BeepBackend.Models.StockEntry", "StockEntry")
                        .WithMany("StockEntryValues")
                        .HasForeignKey("EnvironmentId", "ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BeepBackend.Models.UserArticle", b =>
                {
                    b.HasOne("BeepBackend.Models.Article", "Article")
                        .WithMany("UserArticles")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("BeepBackend.Models.User", "User")
                        .WithMany("UserArticles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BeepBackend.Models.UserRole", b =>
                {
                    b.HasOne("BeepBackend.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BeepBackend.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("BeepBackend.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("BeepBackend.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("BeepBackend.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("BeepBackend.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
