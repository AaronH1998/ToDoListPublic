using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ToDoList.Models
{
    public class SuggestionDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SuggestionDbContext(DbContextOptions<SuggestionDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Suggestion> Suggestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var username = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            modelBuilder.Entity<Suggestion>().HasQueryFilter(s => username == "Developer");
            modelBuilder.HasDefaultSchema("Suggestions");

        }
    }
}
