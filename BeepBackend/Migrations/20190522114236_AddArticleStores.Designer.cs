﻿// <auto-generated />
using System;
using BeepBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeepBackend.Migrations
{
    [DbContext(typeof(BeepDbContext))]
    [Migration("20190522114236_AddArticleStores")]
    partial class AddArticleStores
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BeepBackend.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ArticleGroupFk");

                    b.Property<string>("Barcode");

                    b.Property<bool>("HasLifetime");

                    b.Property<string>("Name");

                    b.Property<int>("TypicalLifetime");

                    b.HasKey("Id");

                    b.HasIndex("ArticleGroupFk");

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
                });

            modelBuilder.Entity("BeepBackend.Models.ArticleStore", b =>
                {
                    b.Property<int>("ArticleId");

                    b.Property<int>("StoreId");

                    b.HasKey("ArticleId", "StoreId");

                    b.HasIndex("StoreId");

                    b.ToTable("ArticleStore");
                });

            modelBuilder.Entity("BeepBackend.Models.ArticleUserSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AmountOnStock");

                    b.Property<int>("ArticleFk");

                    b.Property<bool>("IsOpened");

                    b.Property<int>("KeppStockMode");

                    b.Property<DateTime>("OpenedOn");

                    b.Property<int>("StockAmount");

                    b.HasKey("Id");

                    b.HasIndex("ArticleFk");

                    b.ToTable("ArticleUserSettings");
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

            modelBuilder.Entity("BeepBackend.Models.Article", b =>
                {
                    b.HasOne("BeepBackend.Models.ArticleGroup", "ArticleGroup")
                        .WithMany("Articles")
                        .HasForeignKey("ArticleGroupFk")
                        .OnDelete(DeleteBehavior.Cascade);
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
                });
#pragma warning restore 612, 618
        }
    }
}
