﻿// <auto-generated />
using System;
using CatScraper.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CatScraper.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CatScraper.Domain.Entities.Cat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CatId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTimeOffset>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<int>("Height")
                        .HasColumnType("int");

                    b.Property<int>("Width")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CatId")
                        .IsUnique();

                    b.ToTable("Cats", (string)null);
                });

            modelBuilder.Entity("CatScraper.Domain.Entities.CatTag", b =>
                {
                    b.Property<int>("CatId")
                        .HasColumnType("int");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("CatId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("CatTag", (string)null);
                });

            modelBuilder.Entity("CatScraper.Domain.Entities.GlobalCounter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("GlobalCounters", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Type = 0,
                            Value = 0
                        });
                });

            modelBuilder.Entity("CatScraper.Domain.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tags", (string)null);
                });

            modelBuilder.Entity("CatScraper.Domain.Entities.Cat", b =>
                {
                    b.OwnsOne("CatScraper.Domain.ValueObjects.Image", "Image", b1 =>
                        {
                            b1.Property<int>("CatId")
                                .HasColumnType("int");

                            b1.Property<byte[]>("ImageData")
                                .IsRequired()
                                .HasColumnType("varbinary(max)");

                            b1.Property<string>("ImageExtension")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("CatId");

                            b1.ToTable("Cats");

                            b1.ToJson("Image");

                            b1.WithOwner()
                                .HasForeignKey("CatId");
                        });

                    b.Navigation("Image")
                        .IsRequired();
                });

            modelBuilder.Entity("CatScraper.Domain.Entities.CatTag", b =>
                {
                    b.HasOne("CatScraper.Domain.Entities.Cat", "Cat")
                        .WithMany("CatTags")
                        .HasForeignKey("CatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CatScraper.Domain.Entities.Tag", "Tag")
                        .WithMany("CatTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cat");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("CatScraper.Domain.Entities.Cat", b =>
                {
                    b.Navigation("CatTags");
                });

            modelBuilder.Entity("CatScraper.Domain.Entities.Tag", b =>
                {
                    b.Navigation("CatTags");
                });
#pragma warning restore 612, 618
        }
    }
}
