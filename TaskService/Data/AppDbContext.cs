using Microsoft.EntityFrameworkCore;
using Task = TaskService.Models.Task;

namespace TaskService.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Task> Task => Set<Task>();
    }
}
