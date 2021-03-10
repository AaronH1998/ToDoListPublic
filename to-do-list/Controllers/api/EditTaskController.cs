using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Controllers.api
{
    public class TaskEditResults
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public int Priority { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<KeyValuePair<int, string>> Images { get; set; }
        public string Status { get; set; }
        public Statuses StatusList { get; }
        public string ListType { get; set; }
        public TaskLists ListTypes { get; }
        public string Deadline { get; set; }
        public List<SubtaskLookup> Subtasks { get; set; }
        public string Comments { get; set; }
    }

    public class SubtaskLookup
    {
        public int SubtaskId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }


    }

    public class EditTaskModel
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public string ListType { get; set; }
        public DateTime? Deadline { get; set; }
        public string Comments { get; set; }

    }

    [ApiController]
    [Route("api/[controller]")]
    public class EditTaskController : ControllerBase
    {
        private ApplicationDbContext context;
        private ITasksRepository _tasksRepository;
        private IUnitOfWork _unitOfWork;

        public EditTaskController(ApplicationDbContext ctx, ITasksRepository tasksRepository, IUnitOfWork unitOfWork)
        {
            context = ctx;
            _tasksRepository = tasksRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{taskId}")]
        public async Task<TaskEditResults> Get(int taskId, int projectId, string fromAction)
        {
            var taskEditResults = new TaskEditResults();
            if (taskId != 0)
            {
                var task = await context.Tasks.SingleAsync(p => p.TaskID == taskId);

                taskEditResults.TaskId = task.TaskID;
                taskEditResults.ProjectId = task.ProjectID;
                taskEditResults.Priority = task.Priority;
                taskEditResults.Name = task.Name;
                taskEditResults.Description = task.Description;
                taskEditResults.Status = task.Status;
                taskEditResults.ListType = task.ListType;
                taskEditResults.Comments = task.TaskComments;
                taskEditResults.Deadline = task.Deadline?.ToString("yyyy-MM-dd");
                taskEditResults.Images = new List<KeyValuePair<int, string>>();

                foreach (Image image in task.Images)
                {
                    string base64 = Convert.ToBase64String(image.ImageData);
                    string imgSrc = string.Format("data:image/gif;base64,{0}", base64);
                    taskEditResults.Images.Add(new KeyValuePair<int, string>(image.ImageID, imgSrc));
                }

                taskEditResults.Subtasks = context.SubTask.Where(p => p.TaskModelTaskID == taskId)
                    .Select(s => new SubtaskLookup { SubtaskId = s.SubTaskID, Name = s.Name, Status = s.Status })
                    .ToList();

                return taskEditResults;
            }
            else
            {
                return new TaskEditResults()
                {
                    ProjectId = projectId,
                    ListType = fromAction,
                    Status = "Active"
                };
            }
        }

        [HttpPost]
        public IActionResult Post([FromForm]TaskModel task, [FromForm] IFormFileCollection Images)
        {
            try
            {
                _unitOfWork.Tasks.SaveTask(task, Images);
                _unitOfWork.SaveChangesTaskContext();
                return Ok(new { success = true, data = task.TaskID });
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = e });
            }
        }
    }
}
