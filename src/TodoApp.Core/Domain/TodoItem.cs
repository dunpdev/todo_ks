namespace TodoApp.Core.Domain;

public sealed class TodoItem
{
    private TodoItem(
        int id,
        TodoTitle title,
        Priority priority,
        DateOnly? dueDate,
        bool isCompleted)
    {
        Id = id;
        Title = title;
        Priority = priority;
        DueDate = dueDate;
        IsCompleted = isCompleted;
    }

    public int Id { get; }

    public TodoTitle Title { get; private set; }

    public Priority Priority { get; }

    public DateOnly? DueDate { get; }

    public bool IsCompleted { get; private set; }

    public static TodoItem Create(
        TodoTitle title,
        Priority priority,
        DateOnly? dueDate) =>
        new(0, title, priority, dueDate, isCompleted: false);

    public static TodoItem Rehydrate(
        int id,
        TodoTitle title,
        Priority priority,
        DateOnly? dueDate,
        bool isCompleted)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "A stored todo must have a positive id.");
        }

        return new TodoItem(id, title, priority, dueDate, isCompleted);
    }

    public void Complete() => IsCompleted = true;

    public void Rename(TodoTitle newTitle)
    {
        ArgumentNullException.ThrowIfNull(newTitle);
        Title = newTitle;
    }
}
