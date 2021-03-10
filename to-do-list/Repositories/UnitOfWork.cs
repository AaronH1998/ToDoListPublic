using Microsoft.AspNetCore.Http;
using ToDoList.Models;

namespace ToDoList.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _taskContext;
        private readonly SuggestionDbContext _suggestionContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ITasksRepository Tasks { get; private set; }
        public ISuggestionRepository Suggestions { get; private set; }

        public UnitOfWork(IHttpContextAccessor httpContextAccessor, ApplicationDbContext taskContext, SuggestionDbContext suggestionContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _taskContext = taskContext;
            _suggestionContext = suggestionContext;
            Tasks = new TaskRepository(_httpContextAccessor, _taskContext);
            Suggestions = new EFSuggestionRepository(_httpContextAccessor, _suggestionContext);
        }

        public int SaveChangesTaskContext()
        {
            return _taskContext.SaveChanges();
        }

        public int CompleteSuggestions()
        {
            return _suggestionContext.SaveChanges();
        }

        public void Dispose()
        {
            _taskContext.Dispose();
        }
    }
}
