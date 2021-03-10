using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ToDoList.Repositories;

namespace ToDoList.Controllers.api
{
    public class ProjectLookup
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public ProjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public List<ProjectLookup> Get(bool isClosed)
        {
            if (!isClosed)
            {
                return _unitOfWork.Tasks.GetProjectsOfUser()
                    .Where(p => p.IsClosed == false)
                    .Select(s => new ProjectLookup { ProjectId = s.ProjectID, Name = s.ProjectName })
                    .ToList();
            }
            else
            {
                return _unitOfWork.Tasks.GetProjectsOfUser().Where(p => p.IsClosed)
                    .Select(s => new ProjectLookup { ProjectId = s.ProjectID, Name = s.ProjectName })
                    .ToList();
            }
        }

        [HttpPost]
        public ActionResult CreateProject(string projectName)
        {
            _unitOfWork.Tasks.AddProject(projectName);
            _unitOfWork.SaveChangesTaskContext();
            return NoContent();
        }

        [HttpPut("{projectId}")]
        public ActionResult EditProjectName(int projectId, string projectName)
        {
            var project = _unitOfWork.Tasks.GetProject(projectId);
            project.ProjectName = projectName;
            _unitOfWork.SaveChangesTaskContext();
            return NoContent();
        }

        [HttpPut("Close/{projectId}")]
        public ActionResult CloseProject(int projectId)
        {
            _unitOfWork.Tasks.CloseProject(projectId);
            _unitOfWork.SaveChangesTaskContext();

            return NoContent();
        }
        [HttpPut("Open/{projectId}")]
        public ActionResult OpenProject(int projectId)
        {
            _unitOfWork.Tasks.OpenProject(projectId);
            _unitOfWork.SaveChangesTaskContext();

            return NoContent();
        }
    }
}
