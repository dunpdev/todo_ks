using System.Globalization;
using Microsoft.Data.Sqlite;
using TodoApp.Core.Contracts;
using TodoApp.Core.Domain;

namespace TodoApp.Infrastructure.Persistence;

public sealed class SqliteTodoRepository : ITodoRepository
{
    private readonly string _connectionString;

    public SqliteTodoRepository(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath
        }.ToString();

        CreateSchemaIfNeeded();
    }

    public IReadOnlyList<TodoItem> GetAll()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Title, Priority, DueDate, IsCompleted
            FROM Todos
            ORDER BY Id;
            """;

        using var reader = command.ExecuteReader();
        var todos = new List<TodoItem>();

        while (reader.Read())
        {
            todos.Add(MapTodo(reader));
        }

        return todos;
    }

    public TodoItem? FindById(int id)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Title, Priority, DueDate, IsCompleted
            FROM Todos
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapTodo(reader) : null;
    }

    public TodoItem Add(TodoItem todo)
    {
        ArgumentNullException.ThrowIfNull(todo);

        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Todos (Title, Priority, DueDate, IsCompleted)
            VALUES ($title, $priority, $dueDate, $isCompleted);
            """;
        AddTodoParameters(command, todo);
        command.ExecuteNonQuery();

        using var idCommand = connection.CreateCommand();
        idCommand.CommandText = "SELECT last_insert_rowid();";
        var id = Convert.ToInt32(idCommand.ExecuteScalar(), CultureInfo.InvariantCulture);

        return TodoItem.Rehydrate(
            id,
            todo.Title,
            todo.Priority,
            todo.DueDate,
            todo.IsCompleted);
    }

    public void Update(TodoItem todo)
    {
        ArgumentNullException.ThrowIfNull(todo);

        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Todos
            SET Title = $title,
                Priority = $priority,
                DueDate = $dueDate,
                IsCompleted = $isCompleted
            WHERE Id = $id;
            """;
        AddTodoParameters(command, todo);
        command.Parameters.AddWithValue("$id", todo.Id);

        if (command.ExecuteNonQuery() != 1)
        {
            throw new InvalidOperationException($"Todo with id {todo.Id} could not be updated.");
        }
    }

    private void CreateSchemaIfNeeded()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Todos (
                Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Priority TEXT NOT NULL CHECK (Priority IN ('Low', 'Normal', 'High')),
                DueDate TEXT NULL,
                IsCompleted INTEGER NOT NULL DEFAULT 0 CHECK (IsCompleted IN (0, 1))
            );
            """;
        command.ExecuteNonQuery();
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private static void AddTodoParameters(SqliteCommand command, TodoItem todo)
    {
        command.Parameters.AddWithValue("$title", todo.Title.Value);
        command.Parameters.AddWithValue("$priority", todo.Priority.ToString());
        object dueDateValue = todo.DueDate is { } dueDate
            ? dueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : DBNull.Value;
        command.Parameters.AddWithValue(
            "$dueDate",
            dueDateValue);
        command.Parameters.AddWithValue("$isCompleted", todo.IsCompleted ? 1 : 0);
    }

    private static TodoItem MapTodo(SqliteDataReader reader)
    {
        DateOnly? dueDate = reader.IsDBNull(3)
            ? null
            : DateOnly.ParseExact(
                reader.GetString(3),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);

        return TodoItem.Rehydrate(
            reader.GetInt32(0),
            new TodoTitle(reader.GetString(1)),
            Enum.Parse<Priority>(reader.GetString(2)),
            dueDate,
            reader.GetInt32(4) == 1);
    }
}
