using TodoApp.Core.Contracts;
using TodoApp.Core.Domain;

namespace TodoApp.Core.Services;

public sealed class TodoService(ITodoReader todoReader, ITodoWriter todoWriter)
{
    public IReadOnlyList<TodoItem> GetTodos(ITodoFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return todoReader.GetAll()
            .Where(filter.Matches)
            .ToArray();
    }

    public TodoItem CreateTodo(string title, Priority priority, DateOnly? dueDate)
    {
        var todoTitle = new TodoTitle(title);
        var todo = TodoItem.Create(todoTitle, priority, dueDate);

        return todoWriter.Add(todo);
    }

    public bool CompleteTodo(int id)
    {
        var todo = todoReader.FindById(id);
        if (todo is null)
        {
            return false;
        }

        todo.Complete();
        todoWriter.Update(todo);
        return true;
    }

    public bool RenameTodo(int id, string title)
    {
        var todo = todoReader.FindById(id);
        if (todo is null)
        {
            return false;
        }

        todo.Rename(new TodoTitle(title));
        todoWriter.Update(todo);
        return true;
    }
}
