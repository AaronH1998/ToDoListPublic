using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using ToDoList.Models;

namespace ToDoList.Repositories
{
    public class TaskRepository : Repository<TaskModel>, ITasksRepository
    {
        public ApplicationDbContext TaskContext
        {
            get { return Context as ApplicationDbContext; }
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskRepository(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context) : base(context)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<Project> GetProjectsOfUser()
        {
            return TaskContext.Project.Include(p => p.ProjectUsers).Include(p => p.Tasks).Where(t => t.ProjectUsers.Any(pu => pu.User.Username == GetLoggedInUser()));
        }

        public Image GetImage(int imageId)
        {
            return TaskContext.Image.First(i => i.ImageID == imageId);
        }
        public Project GetProject(int projectId)
        {
            return TaskContext.Project.First(p => p.ProjectID == projectId);
        }

        public TaskModel GetTask(int taskId)
        {
            return TaskContext.Tasks.Include(t => t.Images).Include(p => p.Subtasks).First(t => t.TaskID == taskId);
        }

        public SubTask GetSubTask(int subTaskId)
        {
            return TaskContext.SubTask.First(s => s.SubTaskID == subTaskId);
        }

        public string GetLoggedInUser()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }

        public IEnumerable<TaskModel> GetTasksOfUser(int projectId)
        {
            return TaskContext.Tasks.Include(p => p.Subtasks).Where(t => t.Username == GetLoggedInUser()).Where(t => t.ProjectID == projectId);
        }

        public IEnumerable<TaskModel> GetTasksOfUserOfCorrespondingListType(TaskModel task)
        {
            var tasksOfUser = GetTasksOfUser(task.ProjectID);
            var tasksOfListType = tasksOfUser.Where(t => t.ListType == task.ListType);
            return tasksOfListType;
        }

        public IEnumerable<TaskModel> GetTasksOfUserByListType(string listType, int projectId)
        {
            var data = TaskContext.Tasks
                .Include(p => p.Subtasks)
                .Where(t => t.Project.ProjectUsers.Any(pu => pu.User.Username == GetLoggedInUser()))
                .Where(t => t.ListType == listType)
                .Where(t => t.ProjectID == projectId);
            return data;
        }

        public void AddUser(string username)
        {
            User user = new User
            {
                Username = username
            };
            TaskContext.Users.Add(user);
        }

        public void AddProject(string projectName)
        {
            var project = new Project
            {
                ProjectName = projectName,
                Owner = GetLoggedInUser(),
                IsClosed = false
            };

            AddProjectUser(project);
            TaskContext.Project.Add(project);

        }

        public void AddProjectUser(Project project)
        {
            if (project.ProjectID == 0)
            {
                ProjectUser projectUser = new ProjectUser()
                {
                    Project = project,
                    User = TaskContext.Users.First(u => u.Username == GetLoggedInUser())

                };

                project.ProjectUsers.Add(projectUser);
            }
            else
            {
                Project exitsingProject = GetProject(project.ProjectID);

                ProjectUser projectUser = new ProjectUser()
                {
                    Project = exitsingProject,
                    User = TaskContext.Users.First(u => u.Username == GetLoggedInUser())
                };
                exitsingProject.ProjectUsers.Add(projectUser);

            }
        }

        public IEnumerable<User> GetProjectUsers(int projectId)
        {
            return TaskContext.Users.Where(p => p.ProjectUsers.Any(pu => pu.ProjectID == projectId));
        }

        public void OpenProject(int projectId)
        {
            Project project = GetProject(projectId);
            project.IsClosed = false;
        }

        public void CloseProject(int projectId)
        {
            Project project = GetProject(projectId);
            project.IsClosed = true;
        }

        public void SaveImages(TaskModel task, IFormFileCollection images)
        {
            foreach (IFormFile image in images)
            {
                Image imageToAdd = new Image();
                string fileName = image.FileName;

                if (ExtensionIsPermitted(fileName))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", Path.GetTempFileName());

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }

                    if (SignatureIsValid(fileName, filePath))
                    {
                        imageToAdd.ImageData = File.ReadAllBytes(filePath);
                        task.Images.Add(imageToAdd);
                    }
                }
            }
        }

        public bool ExtensionIsPermitted(string fileName)
        {
            string[] permittedExtensions = { ".jpg", ".png", ".jpeg", ".gif", "tiff" };
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool SignatureIsValid(string fileName, string filePath)
        {

            Dictionary<string, List<byte[]>> fileSignature = new Dictionary<string, List<byte[]>>
            {

                { ".jpeg", new List<byte[]>
                    {
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                    }
                },
                { ".jpg", new List<byte[]>
                    {
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                    }
                },
                { ".png", new List<byte[]>
                    {
                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A,0x0A }
                    }
                },
                {".gif", new List<byte[]>
                    {
                        new byte[]{0x47, 0x49, 0x46, 0x38}
                    }
                },
                {".tiff", new List<byte[]>
                    {
                        new byte[]{0x49, 0x20, 0x49 },
                        new byte[]{ 0x49, 0x49, 0x2A, 0x00 },
                        new byte[] {0x4D,0x4D,0x00,0x2A },
                        new byte[] {0x4D,0x4D,0x00,0x2B }
                    }
                }
            };

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                List<byte[]> signatures = fileSignature[extension];
                byte[] headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                return signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }

        public void SaveTask(TaskModel task, IFormFileCollection images)
        {
            if (task.TaskID == 0)
            {
                if (!GetTasksOfUserOfCorrespondingListType(task).Any())
                {
                    task.Priority = 1;
                }
                else
                {
                    task.Priority = GetTasksOfUserOfCorrespondingListType(task)
                        .OrderBy(t => t.Priority)
                        .Last()
                        .Priority + 1;
                }

                task.PostDate = DateTime.Now;
                task.Username = GetLoggedInUser();
                SaveImages(task, images);
                TaskContext.Tasks.Add(task);
            }
            else
            {
                TaskModel existingTask = Get(task.TaskID);

                if (existingTask != null)
                {
                    existingTask.Name = task.Name;
                    existingTask.Status = task.Status;
                    existingTask.ListType = task.ListType;
                    existingTask.Description = task.Description;
                    SaveImages(existingTask, images);
                    existingTask.Deadline = task.Deadline;
                    existingTask.TaskComments = task.TaskComments;
                    existingTask.Username = GetLoggedInUser();
                }
                if (existingTask.Status == "Closed")
                {
                    CompleteTask(existingTask.TaskID);
                }

                int i = 1;
                int j = 1;

                foreach (TaskModel t in GetTasksOfUser(task.ProjectID).Where(t => t.Priority != 0).OrderBy(t => t.Priority))
                {
                    if (t.ListType == "Next")
                    {
                        t.Priority = i;
                        i++;
                    }
                    else
                    {
                        t.Priority = j;
                        j++;
                    }
                }
            }
        }

        public void SaveSubTask(TaskModel task, SubTask subTask)
        {
            var parentTask = GetTask(task.TaskID);
            if (subTask.SubTaskID == 0)
            {
                subTask.Username = GetLoggedInUser();
                subTask.ParentTask = parentTask.Name;

                parentTask.Subtasks.Add(subTask);

            }
            else
            {
                SubTask subTaskToSave = parentTask.Subtasks.First(s => s.SubTaskID == subTask.SubTaskID);
                if (subTaskToSave != null)
                {
                    subTaskToSave.Name = subTask.Name;
                    subTaskToSave.Description = subTask.Description;
                    subTaskToSave.Status = subTask.Status;
                    subTaskToSave.TaskComments = subTask.TaskComments;
                    subTaskToSave.Username = subTask.Username;

                    if (subTaskToSave.Status == "Closed")
                    {
                        subTaskToSave.IsCompleted = true;
                    }

                }
            }
        }

        public void CompleteTask(int taskID)
        {
            TaskModel completedTask = Get(taskID);
            completedTask.Priority = 0;
            completedTask.Status = "Closed";

            int i = 1;

            foreach (var t in GetTasksOfUserOfCorrespondingListType(completedTask))
            {
                if (t.Status != "Closed")
                {
                    t.Priority = i;
                    i++;
                }
            }

            completedTask.ListType = "Completed";
        }

        public void CompleteSubTask(int subTaskID)
        {
            SubTask completedSubTask = GetSubTask(subTaskID);
            completedSubTask.Status = "Closed";
            completedSubTask.IsCompleted = true;
        }

        public void RemoveImage(int imageId)
        {
            Image image = GetImage(imageId);

            TaskContext.Image.Remove(image);
        }
    }
}
