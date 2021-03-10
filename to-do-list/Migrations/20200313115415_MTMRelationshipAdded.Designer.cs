﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ToDoList.Models;

namespace ToDoList.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200313115415_MTMRelationshipAdded")]
    partial class MTMRelationshipAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ToDoList.Models.Image", b =>
                {
                    b.Property<int>("ImageID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("ImageData");

                    b.Property<int?>("TaskModelTaskID");

                    b.HasKey("ImageID");

                    b.HasIndex("TaskModelTaskID");

                    b.ToTable("Image");
                });

            modelBuilder.Entity("ToDoList.Models.Project", b =>
                {
                    b.Property<int>("ProjectID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsClosed");

                    b.Property<string>("Owner");

                    b.Property<string>("ProjectName");

                    b.HasKey("ProjectID");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("ToDoList.Models.ProjectUser", b =>
                {
                    b.Property<int>("ProjectID");

                    b.Property<int>("UserID");

                    b.HasKey("ProjectID", "UserID");

                    b.HasIndex("UserID");

                    b.ToTable("ProjectUsers");
                });

            modelBuilder.Entity("ToDoList.Models.SubTask", b =>
                {
                    b.Property<int>("SubTaskID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<bool>("IsCompleted");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ParentTask");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<string>("TaskComments");

                    b.Property<int>("TaskModelTaskID");

                    b.Property<string>("Username");

                    b.HasKey("SubTaskID");

                    b.HasIndex("TaskModelTaskID");

                    b.ToTable("SubTask");
                });

            modelBuilder.Entity("ToDoList.Models.TaskModel", b =>
                {
                    b.Property<int>("TaskID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("Deadline");

                    b.Property<string>("Description");

                    b.Property<string>("ListType")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("PostDate");

                    b.Property<int>("Priority");

                    b.Property<int>("ProjectID");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<string>("TaskComments");

                    b.Property<string>("Username");

                    b.HasKey("TaskID");

                    b.HasIndex("ProjectID");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("ToDoList.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Username");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ToDoList.Models.Image", b =>
                {
                    b.HasOne("ToDoList.Models.TaskModel")
                        .WithMany("Images")
                        .HasForeignKey("TaskModelTaskID");
                });

            modelBuilder.Entity("ToDoList.Models.ProjectUser", b =>
                {
                    b.HasOne("ToDoList.Models.Project", "Project")
                        .WithMany("ProjectUsers")
                        .HasForeignKey("ProjectID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ToDoList.Models.User", "User")
                        .WithMany("ProjectUsers")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ToDoList.Models.SubTask", b =>
                {
                    b.HasOne("ToDoList.Models.TaskModel")
                        .WithMany("Subtasks")
                        .HasForeignKey("TaskModelTaskID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ToDoList.Models.TaskModel", b =>
                {
                    b.HasOne("ToDoList.Models.Project", "Project")
                        .WithMany("Tasks")
                        .HasForeignKey("ProjectID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
