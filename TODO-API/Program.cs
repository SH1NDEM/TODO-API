using Microsoft.EntityFrameworkCore;
using TODO_API;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TaskDb>(opt => opt.UseInMemoryDatabase("TaskItemList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Получить все задачи
app.MapGet("/taskitems", async (TaskDb db) => 
    await db.Tasks.ToListAsync());

// Получить все выполненные задачи
app.MapGet("/taskitems/iscomplite", async (TaskDb db) =>
    await db.Tasks.Where(t => t.IsClose).ToListAsync());

// Получить все не выполненные задачи
app.MapGet("/taskitems/isnotcomplite", async (TaskDb db) =>
    await db.Tasks.Where(t => t.IsClose == false).ToListAsync());

// Получить задачу по id
app.MapGet("/taskitems/{id}", async (int id, TaskDb db) => 
    await db.Tasks.FindAsync(id));

// Создать задачу
app.MapPost("/taskitems", async (TaskItem task, TaskDb db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();

    return Results.Created($"/taskitems/{task.Id}", task);
});

// Изменение задачи по id
app.MapPut("/taskitems/{id}", async (TaskItem inputTask, TaskDb db, int id) =>
{
    var taskItem = await  db.Tasks.FindAsync(id);
    if (taskItem is null) return Results.NotFound();

    taskItem.Text = inputTask.Text;
    taskItem.IsClose = inputTask.IsClose;

    await db.SaveChangesAsync();
    return Results.Created($"/taskitems/{inputTask.Id}", inputTask);
});

// Удаление задачи по id
app.MapDelete("/taskitems/{id}", async (TaskDb db, int id) =>
{
    if (await db.Tasks.FindAsync(id) is TaskItem taskItem)
    {
        db.Tasks.Remove(taskItem);
        await db.SaveChangesAsync();
    }
    return Results.NoContent();
});

app.Run();