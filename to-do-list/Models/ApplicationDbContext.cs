using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ToDoList.Models
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Project> Project { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<SubTask> SubTask { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var username = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            modelBuilder.Entity<TaskModel>().HasQueryFilter(t => t.Username == username);
            //modelBuilder.Entity<Project>().HasQueryFilter(t => t.ProjectUsers.Any(pu => pu.User.Username == username));

            modelBuilder.Entity<ProjectUser>().HasKey(pu => new { pu.ProjectID, pu.UserID });
            modelBuilder.Entity<ProjectUser>().HasOne(pu => pu.Project).WithMany(p => p.ProjectUsers).HasForeignKey(pu => pu.ProjectID);
            modelBuilder.Entity<ProjectUser>().HasOne(pu => pu.User).WithMany(p => p.ProjectUsers).HasForeignKey(pu => pu.UserID);
            modelBuilder.HasDefaultSchema("dbo");
        }
    }
}