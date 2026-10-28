using System.Globalization;
using TodoApp.Core.Contracts;
using TodoApp.Core.Domain;
using TodoApp.Core.Filters;
using TodoApp.Core.Services;

namespace TodoApp.Cli;

internal sealed class TodoApplication(TodoService todoService)
{
    public int Run()
    {
        while (true)
        {
            DisplayMenu();
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    CreateTodo();
                    break;
                case "2":
                    DisplayTodos(new AllTodosFilter(), "All tasks");
                    break;
                case "3":
                    DisplayTodos(new OpenTodosFilter(), "Open tasks");
                    break;
                case "4":
                    DisplayTodos(new CompletedTodosFilter(), "Completed tasks");
                    break;
                case "5":
                    CompleteTodo();
                    break;
                case "6":
                    RenameTodo();
                    break;
                case "0":
                    return 0;
                default:
                    Console.WriteLine("Choose one of the listed options.");
                    break;
            }
        }
    }

    private static void DisplayMenu()
    {
        Console.WriteLine();
        Console.WriteLine("1 Add  2 List all  3 List open  4 List completed");
        Console.WriteLine("5 Complete  6 Rename  0 Exit");
        Console.Write("Option: ");
    }

    private void CreateTodo()
    {
        var title = ReadTitle();
        var priority = ReadPriority();
        var dueDate = ReadDueDate();
        var todo = todoService.CreateTodo(title, priority, dueDate);

        Console.WriteLine($"Added task {todo.Id}: {todo.Title}");
    }

    private string ReadTitle()
    {
        while (true)
        {
            Console.Write("Title: ");
            var title = Console.ReadLine() ?? string.Empty;

            try
            {
                return new TodoTitle(title).Value;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }

    private static Priority ReadPriority()
    {
        while (true)
        {
            Console.Write("Priority (Low/Normal/High): ");
            var input = Console.ReadLine();

            if (Enum.TryParse<Priority>(input, ignoreCase: true, out var priority)
                && Enum.IsDefined(priority))
            {
                return priority;
            }

            Console.WriteLine("Enter Low, Normal, or High.");
        }
    }

    private static DateOnly? ReadDueDate()
    {
        while (true)
        {
            Console.Write("Due date (yyyy-MM-dd, optional): ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (DateOnly.TryParseExact(
                    input,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dueDate))
            {
                return dueDate;
            }

            Console.WriteLine("Enter a date in yyyy-MM-dd format, or leave it empty.");
        }
    }

    private void DisplayTodos(ITodoFilter filter, string heading)
    {
        Console.WriteLine();
        Console.WriteLine(heading);

        var todos = todoService.GetTodos(filter);
        if (todos.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }

        foreach (var todo in todos)
        {
            DisplayTodo(todo);
        }
    }

    private static void DisplayTodo(TodoItem todo)
    {
        var dueDate = todo.DueDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "-";
        var status = todo.IsCompleted ? "Done" : "Open";

        Console.WriteLine(
            $"{todo.Id,3} | {todo.Title.Value,-30} | {todo.Priority,-6} | {dueDate} | {status}");
    }

    private void CompleteTodo()
    {
        var id = ReadTodoId();
        var wasCompleted = todoService.CompleteTodo(id);

        Console.WriteLine(wasCompleted ? "Task completed." : $"Task {id} was not found.");
    }

    private void RenameTodo()
    {
        var id = ReadTodoId();
        Console.Write("New title: ");
        var title = Console.ReadLine() ?? string.Empty;

        try
        {
            var wasRenamed = todoService.RenameTodo(id, title);
            Console.WriteLine(wasRenamed ? "Task renamed." : $"Task {id} was not found.");
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    private static int ReadTodoId()
    {
        while (true)
        {
            Console.Write("Task id: ");

            if (int.TryParse(Console.ReadLine(), out var id) && id > 0)
            {
                return id;
            }

            Console.WriteLine("Enter a positive whole number.");
        }
    }
}
