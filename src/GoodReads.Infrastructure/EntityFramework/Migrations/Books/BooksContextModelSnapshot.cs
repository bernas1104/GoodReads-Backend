﻿// <auto-generated />
using System;
using GoodReads.Infrastructure.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GoodReads.Infrastructure.EntityFramework.Migrations.Books
{
    [DbContext(typeof(BooksContext))]
    partial class BooksContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("books")
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GoodReads.Domain.BookAggregate.Entities.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Author");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("DeletedAt");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Description");

                    b.Property<int>("Gender")
                        .HasColumnType("int")
                        .HasColumnName("Gender");

                    b.Property<string>("Isbn")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("ISBN");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Title");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("Isbn")
                        .IsUnique();

                    b.ToTable("Books", "books");
                });

            modelBuilder.Entity("GoodReads.Domain.BookAggregate.Entities.Book", b =>
                {
                    b.OwnsOne("GoodReads.Domain.BookAggregate.ValueObjects.BookData", "BookData", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("BookId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Pages")
                                .HasColumnType("int")
                                .HasColumnName("Pages");

                            b1.Property<string>("Publisher")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Publisher");

                            b1.Property<int>("YearOfPublication")
                                .HasColumnType("int")
                                .HasColumnName("YearOfPublication");

                            b1.HasKey("Id");

                            b1.HasIndex("BookId")
                                .IsUnique();

                            b1.ToTable("BookDatas", "books");

                            b1.WithOwner()
                                .HasForeignKey("BookId");
                        });

                    b.OwnsOne("GoodReads.Domain.BookAggregate.ValueObjects.MeanScore", "MeanScore", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("BookId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Scores")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Scores");

                            b1.HasKey("Id");

                            b1.HasIndex("BookId")
                                .IsUnique();

                            b1.ToTable("MeanScores", "books");

                            b1.WithOwner()
                                .HasForeignKey("BookId");
                        });

                    b.OwnsMany("GoodReads.Domain.BookAggregate.ValueObjects.RatingId", "RatingIds", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<Guid>("BookId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Value")
                                .HasColumnType("uniqueidentifier")
                                .HasColumnName("BookRatingId");

                            b1.HasKey("Id");

                            b1.HasIndex("BookId");

                            b1.ToTable("BookRatingIds", "books");

                            b1.WithOwner()
                                .HasForeignKey("BookId");
                        });

                    b.Navigation("BookData")
                        .IsRequired();

                    b.Navigation("MeanScore")
                        .IsRequired();

                    b.Navigation("RatingIds");
                });
#pragma warning restore 612, 618
        }
    }
}
