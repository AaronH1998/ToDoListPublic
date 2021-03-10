using Microsoft.AspNetCore.Mvc;
using ToDoList.Repositories;

namespace ToDoList.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private ITasksRepository taskRepository;

        public ImageController(IUnitOfWork _unitOfWork, ITasksRepository _taskRepository)
        {
            unitOfWork = _unitOfWork;
            taskRepository = _taskRepository;
        }

        [HttpPut]
        public ActionResult Put([FromForm] int imageId)
        {
            unitOfWork.Tasks.RemoveImage(imageId);
            unitOfWork.SaveChangesTaskContext();
            return Ok();
        }
    }
}