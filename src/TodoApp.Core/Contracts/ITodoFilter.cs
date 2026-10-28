using TodoApp.Core.Domain;

namespace TodoApp.Core.Contracts;

public interface ITodoFilter
{
    bool Matches(TodoItem todo);
}
