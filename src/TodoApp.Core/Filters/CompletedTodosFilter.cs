using TodoApp.Core.Contracts;
using TodoApp.Core.Domain;

namespace TodoApp.Core.Filters;

public sealed class CompletedTodosFilter : ITodoFilter
{
    public bool Matches(TodoItem todo) => todo.IsCompleted;
}
