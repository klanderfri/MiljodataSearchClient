using System.Collections.Concurrent;
using System.Text;

namespace MiljodataSearchClient;

internal class Program
{
    static void Main(string[] args)
    {
        UxHelper.PrintVersion("0.1.0");
        var path = UxHelper.GetValidDirectoryPath();

        while (true)
        {
            var searchTerm = UxHelper.GetSearchTerm();
            var searchHits = Analyzer.AnalyseDirectory(path, searchTerm);
            UxHelper.PrintResult(searchHits);
        }
    }
}

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

internal static class Analyzer
{
    private static readonly Encoding FileEncoding
        = Encoding.Latin1;

    private static readonly HashSet<string> SearchedExtensions
        = [".csv", ".txt"];

    public static ConcurrentBag<SearchResult> AnalyseDirectory(
        string directoryPath,
        string searchTerm)
    {
        var result = new ConcurrentBag<SearchResult>();
        var filePaths = Directory.GetFiles(directoryPath!);

        Parallel.ForEach(filePaths, filePath =>
        {
            var file = new FileInfo(filePath);
            if (SearchedExtensions.Contains(file.Extension))
            {
                AnalyseFile(file, searchTerm)
                    .ForEach(result.Add);
            }
        });

        return result;
    }

    private static List<SearchResult> AnalyseFile(
        FileInfo file,
        string searchTerm)
    {
        var result = new List<SearchResult>();
        var rowNumber = 0;
        foreach (var row in File.ReadAllLines(file.FullName, FileEncoding))
        {
            rowNumber++; //Assume non-programmer user.
            if (string.IsNullOrWhiteSpace(row)) { continue; }
            var hitIndex = row.IndexOf(searchTerm);
            if (hitIndex < 0) { continue; }

            var hit = new SearchResult
            {
                DataLine = row!,
                FileName = file.Name,
                HitIndex = hitIndex,
                RowNumber = rowNumber
            };
            result.Add(hit);
        }
        return result;
    }
}

internal class SearchResult
{
    public required string DataLine { get; set; }
    public required string FileName { get; set; }
    public required int RowNumber { get; set; }
    public required int HitIndex { get; set; }
}
