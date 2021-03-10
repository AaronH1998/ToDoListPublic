using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ToDoList.Infrastructure;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IUnitOfWork unitOfWork;
        private ITasksRepository tasksRepository;
        private ISuggestionRepository suggestionRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ISendEmail _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(UserManager<AppUser> userManager, ITasksRepository tasksRepo, ISuggestionRepository suggestionRepo, IUnitOfWork _unitOfWork, ISendEmail emailSender, IHttpContextAccessor httpContextAccessor)
        {
            unitOfWork = _unitOfWork;
            tasksRepository = tasksRepo;
            suggestionRepository = suggestionRepo;
            _userManager = userManager;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Projects");
        }
        [DefaultBreadcrumb("Projects")]
        public ViewResult Projects()
        {
            ViewBag.SelectedCategory = "Active Projects";
            ViewBag.Title = "Projects";
            ViewBag.Type = "Project";
            ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();
            return View(unitOfWork.Tasks.GetProjectsOfUser().Where(p => p.IsClosed == false));

        }
        public ViewResult ClosedProjects()
        {
            ViewBag.SelectedCategory = "Closed Projects";
            ViewBag.Type = "Project";
            ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();
            ViewBag.Title = "Closed Projects";
            return View(unitOfWork.Tasks.GetProjectsOfUser().Where(p => p.IsClosed == true));
        }

        [Breadcrumb("ViewData.ProjectName", FromAction = "Projects")]
        public ViewResult Tasks(int projectId)
        {
            ViewData["ProjectName"] = tasksRepository.GetProject(projectId).ProjectName;
            ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();
            ViewBag.ProjectID = projectId;
            ViewBag.Title = "Tasks";

            return View();
        }

        public ActionResult CompleteSubTask(int subTaskId)
        {
            unitOfWork.Tasks.CompleteSubTask(subTaskId);
            unitOfWork.SaveChangesTaskContext();

            var projectId = tasksRepository.GetTask(tasksRepository.GetSubTask(subTaskId).TaskModelTaskID).ProjectID;

            return RedirectToAction("Tasks", new { projectId });
        }

        public ActionResult OpenProject(int projectId)
        {
            unitOfWork.Tasks.OpenProject(projectId);
            unitOfWork.SaveChangesTaskContext();

            return RedirectToAction("Projects");
        }
        public ActionResult CloseProject(int projectId)
        {
            unitOfWork.Tasks.CloseProject(projectId);
            unitOfWork.SaveChangesTaskContext();

            return RedirectToAction("ClosedProjects");
        }

        public ViewResult CreateProject()
        {
            ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();
            return View(new Project());
        }

        public async Task SendInvite([FromQuery] string userEmail)
        {
            var projectState = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<ProjectState>("ProjectState");
            int projectID = projectState.ProjectID;
            string currentUser = tasksRepository.GetLoggedInUser();
            Project project = tasksRepository.GetProject(projectID);

            var acceptInviteLink = Url.Action("AcceptInvite", "Home", new { projectID }, Request.Scheme);
            var loginLink = Url.Action("Login", "Account", new { returnUrl = acceptInviteLink }, Request.Scheme);
            string recipientBody = $"You have been invited to join  {currentUser}'s Project: {project.ProjectName}. <br> Click <a href=\"{loginLink}\">here</a> to login and accept the invite.";

            await _emailSender.SendEmailAsync(userEmail, "Project Invite", recipientBody);

            string senderBody = $"Your invite has been sent to  + {userEmail}";

            string currentUserEmail = (await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier))).Email;
            await _emailSender.SendEmailAsync(currentUserEmail, "Project Invite Sent", senderBody);
        }

        [Authorize]
        public IActionResult AcceptInvite(int projectId)
        {
            Project project = tasksRepository.GetProject(projectId);
            unitOfWork.Tasks.AddProjectUser(project);
            unitOfWork.SaveChangesTaskContext();
            return RedirectToAction("AcceptedInvite", new { projectId });
        }

        public ViewResult AcceptedInvite(int projectId)
        {
            ViewBag.ProjectID = projectId;
            return View();
        }

        public ActionResult Edit(int taskId)
        {
            ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();
            TaskModel taskToEdit = unitOfWork.Tasks.GetTask(taskId);

            if (taskToEdit == null)
            {
                return RedirectToAction("Projects");
            }
            else
            {
                int projectId = taskToEdit.ProjectID;

                string projectName = tasksRepository.GetProject(projectId).ProjectName;

                var projectNode = new MvcBreadcrumbNode("Tasks", "Home", projectName)
                {
                    RouteValues = new { projectId, listType = taskToEdit.ListType }
                };

                var taskNode = new MvcBreadcrumbNode("Edit", "Home", taskToEdit.Name)
                {
                    RouteValues = new { taskId },
                    Parent = projectNode
                };
                ViewData["BreadcrumbNode"] = taskNode;

                return View();
            }
        }
        public ViewResult Create(int projectId, string fromAction)
        {
            string projectName = tasksRepository.GetProject(projectId).ProjectName;

            var projectNode = new MvcBreadcrumbNode("Tasks", "Home", projectName)
            {
                RouteValues = new { projectId, listType = fromAction }
            };

            var taskNode = new MvcBreadcrumbNode("Create", "Home", "New Task")
            {
                Parent = projectNode
            };
            ViewData["BreadcrumbNode"] = taskNode;

            return View("Edit");
        }
        [HttpPost]
        public IActionResult Edit(TaskModel task, [FromForm] IFormFileCollection Images)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Tasks.SaveTask(task, Images);
                unitOfWork.SaveChangesTaskContext();
                return RedirectToAction("Tasks", new { projectId = task.ProjectID, listType = task.ListType });

            }
            else
            {

                if (task.TaskID == 0)
                {
                    ViewBag.PostDate = DateTime.Now.ToString("dd-MM-yyyy");
                    ViewBag.Title = "Create New Task";
                }
                else
                {
                    TaskModel currentTask = tasksRepository.Get(task.TaskID);
                    ViewBag.Comments = currentTask.TaskComments;
                    ViewBag.PostDate = currentTask.PostDate;
                    ViewBag.Deadline = currentTask.Deadline;
                    ViewBag.Title = "Edit Task";
                }

                return View(task);

            }
        }

        public ActionResult RemoveImage()
        {
            int imageId = Convert.ToInt32(HttpContext.Request.Query["imageid"]);
            int taskId = Convert.ToInt32(HttpContext.Request.Query["taskid"]);
            unitOfWork.Tasks.RemoveImage(imageId);
            unitOfWork.SaveChangesTaskContext();

            return RedirectToAction("Edit", new { taskId });
        }

        public ActionResult EditSubTask(int subTaskId)
        {
            ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();
            SubTask subTaskToEdit = unitOfWork.Tasks.GetSubTask(subTaskId);
            if (subTaskToEdit == null)
            {
                return RedirectToAction("Projects");
            }
            else
            {
                var parentTask = tasksRepository.GetTask(subTaskToEdit.TaskModelTaskID);
                int taskId = parentTask.TaskID;

                int projectId = parentTask.ProjectID;

                string projectName = tasksRepository.GetProject(projectId).ProjectName;

                var projectNode = new MvcBreadcrumbNode("Tasks", "Home", projectName)
                {
                    RouteValues = new { projectId, listType = parentTask.ListType }
                };
                var taskNode = new MvcBreadcrumbNode("Edit", "Home", parentTask.Name)
                {
                    RouteValues = new { taskId },
                    Parent = projectNode
                };
                var subTaskNode = new MvcBreadcrumbNode("EditSubTask", "Home", subTaskToEdit.Name)
                {
                    RouteValues = new { subTaskId },
                    Parent = taskNode
                };
                ViewData["BreadcrumbNode"] = subTaskNode;

                return View(subTaskToEdit);
            }
        }
        public ViewResult CreateSubTask(int taskId)
        {
            ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();
            ViewBag.Action = "Create New Sub Task";

            var parentTask = tasksRepository.GetTask(taskId);

            string projectName = tasksRepository.GetProject(parentTask.ProjectID).ProjectName;

            var projectNode = new MvcBreadcrumbNode("Tasks", "Home", projectName)
            {
                RouteValues = new { projectId = parentTask.ProjectID, listType = parentTask.ListType }
            };
            var taskNode = new MvcBreadcrumbNode("Edit", "Home", parentTask.Name)
            {
                RouteValues = new { taskId },
                Parent = projectNode
            };
            var subTaskNode = new MvcBreadcrumbNode("EditSubTask", "Home", "New Task")
            {
                Parent = taskNode
            };

            ViewData["BreadcrumbNode"] = subTaskNode;


            return View("EditSubTask", new SubTask() { TaskModelTaskID = parentTask.TaskID });
        }
        [HttpPost]
        public IActionResult EditSubTask(SubTask subTask)
        {
            var parentTask = tasksRepository.GetTask(subTask.TaskModelTaskID);

            if (ModelState.IsValid)
            {

                unitOfWork.Tasks.SaveSubTask(parentTask, subTask);
                unitOfWork.SaveChangesTaskContext();

                return RedirectToAction("Edit", new { taskId = parentTask.TaskID });

            }
            else
            {

                if (subTask.SubTaskID == 0)
                {
                    ViewBag.Title = "Create New Sub Task";
                }
                else
                {
                    SubTask currentSubTask = unitOfWork.Tasks.GetSubTask(subTask.SubTaskID);

                    ViewBag.Comments = currentSubTask.TaskComments;
                    ViewBag.Title = "Edit Sub Task";
                }

                return View(subTask);

            }
        }

        public ViewResult Feedback()
        {
            ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();
            ViewBag.Title = "User Feedback";
            ViewBag.Response = "Thank you for your feedback!";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Feedback(Suggestion suggestion)
        {
            ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();

            if (ModelState.IsValid)
            {
                unitOfWork.Suggestions.SaveSuggestion(suggestion);
                unitOfWork.CompleteSuggestions();

                string currentUser = tasksRepository.GetLoggedInUser();

                var feedbackLink = Url.Action("Index", "Developer", Request.Scheme);
                var loginLink = Url.Action("Login", "Account", new { returnUrl = feedbackLink }, Request.Scheme);
                string emailBody = currentUser + " has sent you the following feedback: <br />" + suggestion.Details + "<br /> click <a href=\"" + loginLink + "\">here</a> to login and view all feedback";

                await _emailSender.SendEmailAsync("aaronhodgson1998@gmail.com", "Task Lists Feedback", emailBody);

                ViewBag.Title = "User Feedback";
                ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();

                return RedirectToAction("Projects");
            }
            else
            {
                ViewBag.Title = "User Feedback";
                ViewBag.Response = "Invalid Response";
                ViewBag.User = unitOfWork.Tasks.GetLoggedInUser();
                return View();
            }
        }
    }
}


