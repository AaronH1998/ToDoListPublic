using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ToDoList.Models;

namespace ToDoList.Repositories
{
    public interface ITasksRepository : IRepository<TaskModel>
    {
        Image GetImage(int imageId);

        Project GetProject(int projectId);

        TaskModel GetTask(int taskId);

        SubTask GetSubTask(int subTaskId);

        string GetLoggedInUser();

        IEnumerable<Project> GetProjectsOfUser();

        IEnumerable<TaskModel> GetTasksOfUser(int projectId);

        IEnumerable<TaskModel> GetTasksOfUserOfCorrespondingListType(TaskModel task);

        IEnumerable<TaskModel> GetTasksOfUserByListType(string listType, int projectId);

        IEnumerable<User> GetProjectUsers(int projectId);

        void AddUser(string username);

        void AddProject(string projectName);

        void AddProjectUser(Project project);

        void OpenProject(int projectId);

        void CloseProject(int projectId);

        void SaveTask(TaskModel task, IFormFileCollection images);

        void CompleteTask(int taskId);

        void SaveSubTask(TaskModel task, SubTask subTask);

        void CompleteSubTask(int SubTaskId);

        void RemoveImage(int imageId);
    }
}
