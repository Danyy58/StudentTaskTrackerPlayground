namespace TaskService.Repositories
{
    public interface ITaskRepository
    {
        public Task<int?> AddTask(Models.Task task);
        public Task<List<Models.Task>> GetAllTasks(int authorId);
        public Task<Models.Task?> GetTask(int taskId, int authorId);
        public Task Save(Models.Task result);
        public Task Delete(Models.Task result);
    }
}
