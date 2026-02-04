namespace TODO_API.Mobile.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsClose { get; set; }
}
