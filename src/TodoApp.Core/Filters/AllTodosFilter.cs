using TodoApp.Core.Contracts;
using TodoApp.Core.Domain;

namespace TodoApp.Core.Filters;

public sealed class AllTodosFilter : ITodoFilter
{
    public bool Matches(TodoItem todo) => true;
}
