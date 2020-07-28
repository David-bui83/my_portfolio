﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using resume_site.Models;

namespace resume_site.Migrations
{
    [DbContext(typeof(MyContext))]
    partial class MyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("resume_site.Models.Association", b =>
                {
                    b.Property<int>("AssociationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int?>("ProjectId")
                        .HasColumnName("project_id");

                    b.Property<int?>("TechnologyId")
                        .HasColumnName("technology_id");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("AssociationId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TechnologyId");

                    b.ToTable("Associations");
                });

            modelBuilder.Entity("resume_site.Models.Detail", b =>
                {
                    b.Property<int>("DetailId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("DetailDetail")
                        .IsRequired();

                    b.Property<string>("DetailName")
                        .IsRequired();

                    b.Property<int>("ProjectId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("DetailId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Details");
                });

            modelBuilder.Entity("resume_site.Models.EmailMessage", b =>
                {
                    b.Property<int>("EmailId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("FromEmail")
                        .IsRequired();

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("Subject")
                        .IsRequired();

                    b.Property<string>("ToEmail");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("EmailId");

                    b.ToTable("EmailMessages");
                });

            modelBuilder.Entity("resume_site.Models.HackerTracker", b =>
                {
                    b.Property<int>("HackerId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("HackerDevice");

                    b.Property<string>("HackerIp");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("HackerId");

                    b.ToTable("HackerTrackers");
                });

            modelBuilder.Entity("resume_site.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<bool>("IsDone");

                    b.Property<string>("ProjectDescription")
                        .IsRequired();

                    b.Property<string>("ProjectGitHub");

                    b.Property<string>("ProjectImage")
                        .IsRequired();

                    b.Property<string>("ProjectLink")
                        .IsRequired();

                    b.Property<string>("ProjectName")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int?>("UserId");

                    b.HasKey("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("resume_site.Models.Technology", b =>
                {
                    b.Property<int>("TechnologyId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("TechnologyImage")
                        .IsRequired();

                    b.Property<string>("TechnologyName")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("TechnologyId");

                    b.ToTable("Technologies");
                });

            modelBuilder.Entity("resume_site.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("Info");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("UserImage");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.Property<string>("UserResume");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("resume_site.Models.VisitorTracker", b =>
                {
                    b.Property<int>("VisitorId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("VisitorDevice");

                    b.Property<string>("VisitorIp");

                    b.HasKey("VisitorId");

                    b.ToTable("VisitorTrackers");
                });

            modelBuilder.Entity("resume_site.Models.Association", b =>
                {
                    b.HasOne("resume_site.Models.Project", "Project")
                        .WithMany("TechnologyAssociated")
                        .HasForeignKey("ProjectId");

                    b.HasOne("resume_site.Models.Technology", "Technology")
                        .WithMany("ProjectAssociated")
                        .HasForeignKey("TechnologyId");
                });

            modelBuilder.Entity("resume_site.Models.Detail", b =>
                {
                    b.HasOne("resume_site.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("resume_site.Models.Project", b =>
                {
                    b.HasOne("resume_site.Models.User")
                        .WithMany("Projects")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
