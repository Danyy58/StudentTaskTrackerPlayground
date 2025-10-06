using Confluent.Kafka;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using TaskService.DTO;
using TaskService.Repositories;

namespace TaskService.Services
{
    public class TaskServices(ITaskRepository repo) : ITaskServices
    {
        public async Task<Models.Task?> AddTask(TaskRequest request, int authorId)
        {
            var task = new Models.Task
            {
                AuthorID = authorId,
                Title = request.Title,
                Description = request.Description
            };

            var result = await repo.AddTask(task);

            if (result <= 0)
                return null;

            await ProduceMsg(authorId);

            return task;
        }

        public async Task<List<TaskResponse>> GetAllTasks(int authorId)
        {
            var tasks = await repo.GetAllTasks(authorId);
            var response = tasks.Select(t => new TaskResponse
            {
                ID = t.ID,
                Title = t.Title,
                Description = t.Description
            }).ToList();

            return response;
        }

        public async Task<Models.Task?> EditTask(int taskId, int authorId, TaskRequest request)
        {
            var result = await repo.GetTask(taskId, authorId);
            if (result is null)
                return null;

            result.Title = request.Title;
            result.Description = request.Description;
            await repo.Save(result);

            await ProduceMsg(authorId);


            return result;
        }

        public async Task<Models.Task?> DeleteTask(int taskId, int authorId)
        {
            var result = await repo.GetTask(taskId, authorId);
            if (result is null)
                return null;

            await repo.Delete(result);

            await ProduceMsg(authorId);

            return result;
        }

        private async Task ProduceMsg(int authorId)
        {
            var message = new Message<int, int>()
            {
                Key = authorId,
                Value = authorId
            };

            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092",
                Acks = Acks.All
            };

            using var producer = new ProducerBuilder<int, int>(producerConfig).Build();
            await producer.ProduceAsync("redis-remove", message);
        }
    }
}
