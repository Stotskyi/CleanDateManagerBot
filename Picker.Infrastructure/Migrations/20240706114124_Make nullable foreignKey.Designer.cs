﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Picker.Infrastructure.Data;

#nullable disable

namespace Picker.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240706114124_Make nullable foreignKey")]
    partial class MakenullableforeignKey
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Picker.Infrastructure.Entities.CleaningTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.ToTable("CleaningTimes");
                });

            modelBuilder.Entity("Picker.Infrastructure.Entities.Coliver", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("CleaningTimeId")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CleaningTimeId");

                    b.ToTable("Colivers");
                });

            modelBuilder.Entity("Picker.Infrastructure.Entities.UserState", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("UserId"));

                    b.Property<string>("InputDate")
                        .HasColumnType("text");

                    b.Property<string>("LastCommand")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastInteraction")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("State")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("UserStates");
                });

            modelBuilder.Entity("Picker.Infrastructure.Entities.Coliver", b =>
                {
                    b.HasOne("Picker.Infrastructure.Entities.CleaningTime", "CleaningTime")
                        .WithMany("Colivers")
                        .HasForeignKey("CleaningTimeId");

                    b.Navigation("CleaningTime");
                });

            modelBuilder.Entity("Picker.Infrastructure.Entities.CleaningTime", b =>
                {
                    b.Navigation("Colivers");
                });
#pragma warning restore 612, 618
        }
    }
}
