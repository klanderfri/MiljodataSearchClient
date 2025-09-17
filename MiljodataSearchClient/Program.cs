namespace MiljodataSearchClient;

internal class Program
{
    static void Main(string[] args)
    {
        UxHelper.PrintVersion("0.2.0");
        var path = UxHelper.GetValidDirectoryPath();

        while (true)
        {
            var searchTerm = UxHelper.GetSearchTerm();
            var searchHits = Analyzer.AnalyseDirectory(path, searchTerm);
            UxHelper.PrintResult(searchHits);
        }
    }
}
