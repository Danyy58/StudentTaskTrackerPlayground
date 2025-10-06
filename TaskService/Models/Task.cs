namespace TaskService.Models
{
    public class Task
    {
        public int ID { get; set; }
        public int AuthorID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
