namespace MiljodataSearchClient;

internal class SearchResult
{
    public required string DataLine { get; set; }
    public required string FileName { get; set; }
    public required int RowNumber { get; set; }
    public required int HitIndex { get; set; }
}
