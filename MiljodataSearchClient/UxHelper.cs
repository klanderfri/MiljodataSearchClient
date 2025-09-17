namespace MiljodataSearchClient;

internal static class UxHelper
{
    public static void PrintVersion(string version)
    {
        Console.WriteLine($"MiljodataSearchClient version {version}");
        Console.WriteLine();
    }

    public static string GetSearchTerm()
    {
        string? searchTerm;
        do
        {
            Console.WriteLine("Vad vill du söka efter?");
            searchTerm = Console.ReadLine();
            Console.WriteLine();

        } while (string.IsNullOrWhiteSpace(searchTerm));

        return searchTerm!;
    }

    public static string GetValidDirectoryPath()
    {
        string? path;
        bool isValidPath;
        do
        {
            Console.WriteLine("Var på datorn finns mappen \"miljodata\"?");
            path = Console.ReadLine();
            Console.WriteLine();

            isValidPath = ValidatePath(path);
            if (!isValidPath)
            {
                Console.WriteLine("Felaktig sökväg. Den borde se ut ungefär såhär:");
                Console.WriteLine("C:\\Downloads\\miljodata");
            }

        } while (!isValidPath);

        return path!;
    }

    public static void PrintResult(IEnumerable<SearchResult> searchHits)
    {
        if (searchHits.Any())
        {
            Console.WriteLine($"Antal berörda rader: {searchHits.Count()}");
            Console.WriteLine("Tryck valfri tangent för att skriva ut...");
            Console.ReadKey();
            Console.WriteLine();

            Console.WriteLine("File,Row,Column,Data");
            foreach (var hit in searchHits)
            {
                var hitText = $"{hit.FileName},{hit.RowNumber},{hit.HitIndex},{hit.DataLine}";
                Console.WriteLine(hitText);
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Inga berörda rader hittades.");
            Console.WriteLine();
        }
    }

    private static bool ValidatePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) { return false; }
        if (!Directory.Exists(path)) { return false; }
        path = path.TrimEnd('\\');
        if (!path.EndsWith("\\miljodata")) { return false; }
        return true;
    }
}
