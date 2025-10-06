using TaskService.DTO;

namespace TaskService.Services
{
    public interface ITaskServices
    {
        public Task<Models.Task?> AddTask(TaskRequest request, int authorId);
        public Task<List<TaskResponse>> GetAllTasks(int authorId);
        public Task<Models.Task?> EditTask(int taskId, int authorId, TaskRequest request);
        public Task<Models.Task?> DeleteTask(int taskId, int authorId);
    }
}
