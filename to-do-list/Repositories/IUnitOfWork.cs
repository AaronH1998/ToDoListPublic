using System;

namespace ToDoList.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ITasksRepository Tasks { get; }
        ISuggestionRepository Suggestions { get; }
        int SaveChangesTaskContext();
        int CompleteSuggestions();
    }
}
