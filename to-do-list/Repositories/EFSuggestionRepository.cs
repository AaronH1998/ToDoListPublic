using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using ToDoList.Models;

namespace ToDoList.Repositories
{
    public class EFSuggestionRepository : Repository<Suggestion>, ISuggestionRepository
    {
        public SuggestionDbContext SuggestionContext
        {
            get { return Context as SuggestionDbContext; }
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public EFSuggestionRepository(IHttpContextAccessor httpContextAccessor, SuggestionDbContext context) : base(context)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public void SaveSuggestion(Suggestion suggestion)
        {
            var username = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            suggestion.User = username;
            suggestion.PostDate = DateTime.Now;

            SuggestionContext.Suggestions.Add(suggestion);
        }
    }
}
