using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }

        public string ProjectName { get; set; }

        public List<TaskModel> Tasks { get; set; }

        public bool IsClosed { get; set; }

        public string Owner { get; set; }

        public List<ProjectUser> ProjectUsers { get; set; }

        public Project()
        {
            Tasks = new List<TaskModel>();
            ProjectUsers = new List<ProjectUser>();
        }
    }

    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public List<ProjectUser> ProjectUsers { get; set; }

        public User()
        {
            ProjectUsers = new List<ProjectUser>();
        }
    }

    public class ProjectUser
    {
        public int ProjectID { get; set; }
        public Project Project { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }
    }

    public class TaskModel
    {
        [Key]
        public int TaskID { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please enter a task name")]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Image> Images { get; set; }

        [Required(ErrorMessage = "Please enter a status")]
        public string Status { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime PostDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? Deadline { get; set; }

        public int Priority { get; set; }

        public string ListType { get; set; }

        public List<SubTask> Subtasks { get; set; }

        public string TaskComments { get; set; }

        public string Username { get; set; }

        public int ProjectID { get; set; }
        public Project Project { get; set; }

        public TaskModel()
        {
            Subtasks = new List<SubTask>();
            Images = new List<Image>();
        }

    }

    public class Image
    {
        [Key]
        public int ImageID { get; set; }

        public byte[] ImageData { get; set; }

    }

    public class SubTask
    {
        [Key]
        public int SubTaskID { get; set; }

        [Required(ErrorMessage = "Please enter a task name")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter a status")]
        public string Status { get; set; }

        public string TaskComments { get; set; }

        public string ParentTask { get; set; }

        public string Username { get; set; }

        public bool IsCompleted { get; set; }

        public int TaskModelTaskID { get; set; }
    }


    public enum Statuses
    {
        Active,
        Review,
        Closed
    }

    public enum TaskLists
    {
        Next,
        Backlog
    }
}