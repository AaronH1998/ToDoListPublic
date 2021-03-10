using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using ToDoList.Repositories;

namespace ToDoList.Controllers
{

    public class DeveloperController : Controller
    {
        private IUnitOfWork unitOfWork;

        public DeveloperController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        [Breadcrumb("Feedback")]
        [Authorize(Roles = "Developer")]
        public ViewResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Developer")]
        public ViewResult View(int suggestionId)
        {
            var suggestion = unitOfWork.Suggestions.Get(suggestionId);

            var feedbackHomeNode = new MvcBreadcrumbNode("Index", "Developer", "Feedback");
            var feedbackViewNode = new MvcBreadcrumbNode("View", "Developer", suggestion.User + " - " + suggestion.PostDate)
            {
                Parent = feedbackHomeNode
            };
            ViewData["BreadcrumbNode"] = feedbackViewNode;
            return View(suggestion);
        }
    }
}
