namespace PetCareAI.Application.Common;

public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int Total);

public class QueryParams
{
    private int _page = 1;
    private int _pageSize = 10;

    public int Page { get => _page; set => _page = value < 1 ? 1 : value; }
    public int PageSize { get => _pageSize; set => _pageSize = value < 1 ? 10 : Math.Min(value, 100); }
    public string? Sort { get; set; }
}