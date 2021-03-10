using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Component
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private ITasksRepository tasksRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NavigationMenuViewComponent(ITasksRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            tasksRepository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke()
        {
            var projectState = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<ProjectState>("ProjectState");
            if (projectState != null)
            {
                ViewData["ProjectName"] = tasksRepository.GetProject(projectState.ProjectID).ProjectName;
            }
            ViewBag.Username = tasksRepository.GetLoggedInUser();
            List<string> categories = new List<string>();

            if (ViewBag.Type == "Project")
            {
                categories.AddRange(new List<string> { "Active Projects", "Closed Projects" });
            }
            else
            {
                categories.AddRange(new List<string> { "Next", "Backlog", "Completed" });

            }

            return View(categories.Select(x => x).Distinct());
        }
    }
}
