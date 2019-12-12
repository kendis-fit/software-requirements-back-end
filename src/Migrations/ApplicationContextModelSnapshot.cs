﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SoftwareRequirements.Db;

namespace SoftwareRequirements.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SoftwareRequirements.Models.Db.Requirement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<string>("Profile")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Requirements");
                });

            modelBuilder.Entity("SoftwareRequirements.Models.Db.Requirement", b =>
                {
                    b.HasOne("SoftwareRequirements.Models.Db.Requirement", "Parent")
                        .WithMany("Requirements")
                        .HasForeignKey("ParentId");
                });
#pragma warning restore 612, 618
        }
    }
}
