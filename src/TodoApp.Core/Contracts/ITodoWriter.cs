using TodoApp.Core.Domain;

namespace TodoApp.Core.Contracts;

public interface ITodoWriter
{
    TodoItem Add(TodoItem todo);

    void Update(TodoItem todo);
}
