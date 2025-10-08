using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskService.DTO;
using TaskService.Services;

namespace TaskService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(ITaskServices service) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Models.Task?>> CreateTask(TaskRequest request)
        {
            if (request is null)
                return BadRequest("Некорректно заполнены поля");

            var authorId = GetUser();
            var result = await service.AddTask(request, authorId);

            return result is null ? BadRequest("Ошибка при создании задания") : Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskResponse>>> GetAllTasks()
        {
            var authorId = GetUser();
            var tasks = await service.GetAllTasks(authorId);

            return tasks.Count == 0 ? NotFound("Задания не найдены") : Ok(tasks);
        }

        [HttpPut("taskId")]
        public async Task<ActionResult<Models.Task?>> EditTask(int taskId, TaskRequest request)
        {
            if (request is null)
                return BadRequest();

            var authorId = GetUser();

            var result = await service.EditTask(taskId, authorId, request);

            return result is null ? NotFound("Редактируемое задание не найдено") : Ok(result);
        }

        [HttpDelete("taskId")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var authorId = GetUser();
            var result = await service.DeleteTask(taskId, authorId);

            return result is null ? NotFound("Удаляемое задание не найдено") : Ok("Задание успешно удалено");
        }

        private int GetUser()
        {
            return Convert.ToInt32(HttpContext.Request.Headers["X-User-Id"].FirstOrDefault());
        }
    }
}
