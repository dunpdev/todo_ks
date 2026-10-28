using TodoApp.Cli;
using TodoApp.Core.Services;
using TodoApp.Infrastructure.Persistence;

var solutionDirectory = SolutionDirectory.Find();
var databasePath = Path.Combine(solutionDirectory, "todo.db");
var repository = new SqliteTodoRepository(databasePath);
var todoService = new TodoService(repository, repository);
var application = new TodoApplication(todoService);

return application.Run();
