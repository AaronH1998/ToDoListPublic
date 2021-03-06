// <auto-generated />
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
    [Migration("20200302154838_Initials")]
    partial class Initials
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.Property<byte[]>("Image");

                    b.Property<string>("ListType")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("PostDate");

                    b.Property<int>("Priority");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<string>("TaskComments");

                    b.Property<string>("Username");

                    b.HasKey("TaskID");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("ToDoList.Models.SubTask", b =>
                {
                    b.HasOne("ToDoList.Models.TaskModel")
                        .WithMany("Subtasks")
                        .HasForeignKey("TaskModelTaskID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
