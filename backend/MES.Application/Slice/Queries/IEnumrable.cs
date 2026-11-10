namespace MesMicroservice.Application.Queries;

public class IEnumrable<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalItems { get; set; } = 0;

    public IEnumrable(IEnumerable<T> items, int totalItems)
    {
        Items = items;
        TotalItems = totalItems;
    }
}