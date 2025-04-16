﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ResumeSystem.Models.Database;

#nullable disable

namespace ResumeSystem.Migrations
{
    [DbContext(typeof(ResumeContext))]
    partial class ResumeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ResumeSystem.Models.Database.Candidate", b =>
                {
                    b.Property<int>("CandidateID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CandidateID"));

                    b.Property<string>("CAN_EMAIL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CAN_NAME")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CAN_PHONE")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CandidateID");

                    b.ToTable("Candidates");
                });

            modelBuilder.Entity("ResumeSystem.Models.Database.CandidateSkill", b =>
                {
                    b.Property<int>("CandidateID")
                        .HasColumnType("int");

                    b.Property<int>("SkillID")
                        .HasColumnType("int");

                    b.HasKey("CandidateID", "SkillID");

                    b.HasIndex("SkillID");

                    b.ToTable("CandidateSkill");
                });

            modelBuilder.Entity("ResumeSystem.Models.Database.Resume", b =>
                {
                    b.Property<int>("ResumeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ResumeID"));

                    b.Property<int>("CandidateID")
                        .HasColumnType("int");

                    b.Property<string>("RESUME_STRING")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RESUME_URL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ResumeID");

                    b.HasIndex("CandidateID");

                    b.ToTable("Resumes");
                });

            modelBuilder.Entity("ResumeSystem.Models.Database.Skill", b =>
                {
                    b.Property<int>("SkillID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SkillID"));

                    b.Property<string>("SKILL_NAME")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SkillID");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("ResumeSystem.Models.Database.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ResumeSystem.Models.Database.CandidateSkill", b =>
                {
                    b.HasOne("ResumeSystem.Models.Database.Candidate", "Candidate")
                        .WithMany("CandidateSkills")
                        .HasForeignKey("CandidateID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ResumeSystem.Models.Database.Skill", "Skill")
                        .WithMany("CandidateSkills")
                        .HasForeignKey("SkillID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Candidate");

                    b.Navigation("Skill");
                });

            modelBuilder.Entity("ResumeSystem.Models.Database.Resume", b =>
                {
                    b.HasOne("ResumeSystem.Models.Database.Candidate", "Candidate")
                        .WithMany("Resumes")
                        .HasForeignKey("CandidateID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Candidate");
                });

            modelBuilder.Entity("ResumeSystem.Models.Database.Candidate", b =>
                {
                    b.Navigation("CandidateSkills");

                    b.Navigation("Resumes");
                });

            modelBuilder.Entity("ResumeSystem.Models.Database.Skill", b =>
                {
                    b.Navigation("CandidateSkills");
                });
#pragma warning restore 612, 618
        }
    }
}
