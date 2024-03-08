﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Upsertable.SqlServer.Tests;

#nullable disable

namespace Upsertable.SqlServer.Tests.Migrations
{
    [DbContext(typeof(TestDbContext))]
    [Migration("20240308191013_Create")]
    partial class Create
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Upsertable.Tests.Entities.Ack", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FooId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FooId");

                    b.ToTable("Ack");
                });

            modelBuilder.Entity("Upsertable.Tests.Entities.Baz", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Baz");
                });

            modelBuilder.Entity("Upsertable.Tests.Entities.Foo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Zot")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Foos");
                });

            modelBuilder.Entity("Upsertable.Tests.Entities.Fub", b =>
                {
                    b.Property<int>("FooId")
                        .HasColumnType("int");

                    b.Property<int>("BazId")
                        .HasColumnType("int");

                    b.HasKey("FooId", "BazId");

                    b.HasIndex("BazId");

                    b.ToTable("Fub");
                });

            modelBuilder.Entity("Upsertable.Tests.Entities.Qux", b =>
                {
                    b.Property<int>("BazId")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BazId");

                    b.ToTable("Qux");
                });

            modelBuilder.Entity("Upsertable.Tests.Entities.Ack", b =>
                {
                    b.HasOne("Upsertable.Tests.Entities.Foo", "Foo")
                        .WithMany("Acks")
                        .HasForeignKey("FooId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Upsertable.Tests.Entities.Bar", "Bar", b1 =>
                        {
                            b1.Property<int>("AckId")
                                .HasColumnType("int");

                            b1.Property<string>("Code")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("AckId");

                            b1.ToTable("Ack");

                            b1.WithOwner()
                                .HasForeignKey("AckId");
                        });

                    b.Navigation("Bar");

                    b.Navigation("Foo");
                });

            modelBuilder.Entity("Upsertable.Tests.Entities.Fub", b =>
                {
                    b.HasOne("Upsertable.Tests.Entities.Baz", "Baz")
                        .WithMany()
                        .HasForeignKey("BazId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Upsertable.Tests.Entities.Foo", "Foo")
                        .WithMany("Fubs")
                        .HasForeignKey("FooId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Baz");

                    b.Navigation("Foo");
                });

            modelBuilder.Entity("Upsertable.Tests.Entities.Qux", b =>
                {
                    b.HasOne("Upsertable.Tests.Entities.Baz", "Baz")
                        .WithOne("Qux")
                        .HasForeignKey("Upsertable.Tests.Entities.Qux", "BazId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("Upsertable.Tests.Entities.Fum", "Fums", b1 =>
                        {
                            b1.Property<int>("OwnerId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Code")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("OwnerId", "Id");

                            b1.ToTable("Fum");

                            b1.WithOwner()
                                .HasForeignKey("OwnerId");
                        });

                    b.Navigation("Baz");

                    b.Navigation("Fums");
                });

            modelBuilder.Entity("Upsertable.Tests.Entities.Baz", b =>
                {
                    b.Navigation("Qux");
                });

            modelBuilder.Entity("Upsertable.Tests.Entities.Foo", b =>
                {
                    b.Navigation("Acks");

                    b.Navigation("Fubs");
                });
#pragma warning restore 612, 618
        }
    }
}
