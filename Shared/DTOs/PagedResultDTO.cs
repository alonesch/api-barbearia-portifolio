namespace BarbeariaPortifolio.API.Shared.DTOs;

public class PagedResultDTO<T>
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<T> Items { get; set; } = new List<T>();
}
