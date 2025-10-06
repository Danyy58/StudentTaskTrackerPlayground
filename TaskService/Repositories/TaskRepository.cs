
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TaskService.Data;

namespace TaskService.Repositories
{
    public class TaskRepository(AppDbContext context, IDistributedCache? cache) : ITaskRepository
    {
        public async Task<int?> AddTask(Models.Task task)
        {
            try
            {
                context.Add(task);
                await context.SaveChangesAsync();
                return task.ID;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Models.Task>> GetAllTasks(int authorId)
        {
            var cachedTasks = cache?.GetString($"user-{authorId}");

            if (cachedTasks is not null)
                return (JsonSerializer.Deserialize<List<Models.Task>>(cachedTasks))!;

            var tasks = await context.Task.Where(t => t.AuthorID == authorId).ToListAsync();

            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            cache?.SetString($"user-{authorId}", JsonSerializer.Serialize(tasks), options);

            return tasks;
        }

        public async Task<Models.Task?> GetTask(int taskId, int authorId)
        {
            var task = await context.Task.FirstOrDefaultAsync(t => t.ID == taskId && t.AuthorID == authorId);
            return task;
        }

        public async Task Save(Models.Task result)
        {
            Models.Task task = (await context.Task.FindAsync(result.ID))!;
            task = result;
            await context.SaveChangesAsync();
        }

        public async Task Delete(Models.Task result)
        {
            context.Task.Remove(result);
            await context.SaveChangesAsync();
        }
    }
}
