using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;

namespace ToDoList.Component
{
    public class TopNavbarViewComponent : ViewComponent
    {
        private IHttpContextAccessor _httpContextAccessor;

        public TopNavbarViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IViewComponentResult Invoke()
        {
            var projectState = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<ProjectState>("ProjectState");
            if (projectState != null)
            {
                ViewBag.ProjectID = projectState.ProjectID;
            }
            return View();
        }
    }
}
