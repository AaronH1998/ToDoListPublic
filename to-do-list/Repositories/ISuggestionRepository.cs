using ToDoList.Models;

namespace ToDoList.Repositories
{
    public interface ISuggestionRepository : IRepository<Suggestion>
    {
        void SaveSuggestion(Suggestion suggestion);
    }
}
