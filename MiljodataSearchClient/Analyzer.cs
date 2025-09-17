using System.Collections.Concurrent;
using System.Text;

namespace MiljodataSearchClient;

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
