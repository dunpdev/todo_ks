using TodoApp.Core.Domain;

namespace TodoApp.Core.Contracts;

public interface ITodoReader
{
    IReadOnlyList<TodoItem> GetAll();

    TodoItem? FindById(int id);
}
