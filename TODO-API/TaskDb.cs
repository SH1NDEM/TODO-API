using Microsoft.EntityFrameworkCore;
namespace TODO_API;

public class TaskDb : DbContext
{
    public TaskDb(DbContextOptions<TaskDb> options)
        : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}