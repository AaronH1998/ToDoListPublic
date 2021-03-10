using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Controllers.api
{
    public class StringContentWrapper
    {
        public string Content { get; set; }
    }
    public class TasksResults
    {
        public string ListType { get; set; }
        public List<TaskLookup> Tasks { get; set; } = new List<TaskLookup>();
        public List<User> ProjectUsers { get; set; }
        public string ProjectOwner { get; set; }
    }
    public class TaskLookup
    {
        public int TaskId { get; set; }
        public int Priority { get; set; }
        public string TaskName { get; set; }
        public string Status { get; set; }
        public DateTime? Deadline { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private ApplicationDbContext context;
        private ITasksRepository _tasksRepository;
        private IUnitOfWork _unitOfWork;

        public TasksController(ApplicationDbContext ctx, ITasksRepository tasksRepository, IUnitOfWork unitOfWork)
        {
            context = ctx;
            _tasksRepository = tasksRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{projectId}")]
        public TasksResults Get(int projectId, string listType)
        {
            var tasks = _tasksRepository.GetTasksOfUserByListType(listType ?? "Next", projectId)
                .Select(p => new TaskLookup { TaskId = p.TaskID, Priority = p.Priority, TaskName = p.Name, Status = p.Status, Deadline = p.Deadline }).OrderBy(p => p.Priority)
                .ToList();

            if (listType == "Completed")
            {
                tasks.Reverse();
            }

            return new TasksResults
            {
                ListType = listType ?? "Next",
                Tasks = tasks,
                ProjectUsers = _tasksRepository.GetProjectUsers(projectId).ToList(),
                ProjectOwner = _tasksRepository.GetProject(projectId).Owner
            };
        }

        [HttpPut("{taskId}")]
        public ActionResult Put(int taskId, StringContentWrapper targetList)
        {
            var task = context.Tasks.Single(t => t.TaskID == taskId);


            if (targetList.Content == "Completed")
            {
                task.Status = "Closed";
                task.Priority = 0;
            }
            else
            {
                task.Status = "Active";

                if (!_tasksRepository.GetTasksOfUserByListType(targetList.Content, task.ProjectID).Any())
                {
                    task.Priority = 1;
                }
                else
                {
                    task.Priority = _tasksRepository.GetTasksOfUserByListType(targetList.Content, task.ProjectID)
                        .OrderBy(t => t.Priority)
                        .Last()
                        .Priority + 1;
                }
            }
            task.ListType = targetList.Content;
            context.SaveChanges();
            return NoContent();
        }

        [HttpPut("sortable")]
        public ActionResult Put([FromForm]List<TaskLookup> tasks)
        {
            foreach (var task in tasks)
            {
                context.Tasks.Single(p => p.TaskID == task.TaskId).Priority = task.Priority;
            }
            context.SaveChanges();
            return NoContent();
        }

        [HttpPut("Complete/{taskId}")]
        public ActionResult Complete(int taskId)
        {
            _unitOfWork.Tasks.CompleteTask(taskId);
            _unitOfWork.SaveChangesTaskContext();

            return NoContent();
        }
    }
}
