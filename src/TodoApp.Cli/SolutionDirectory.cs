namespace TodoApp.Cli;

internal static class SolutionDirectory
{
    public static string Find()
    {
        for (DirectoryInfo? directory = new(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "TodoApp.sln")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException(
            "Could not locate TodoApp.sln to determine where todo.db should be created.");
    }
}
