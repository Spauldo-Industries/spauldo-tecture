namespace spauldo_techture;
public class Page<T>()
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 1000;
    public Type PageType = typeof(T);
    public List<T> Values { get; set; }
    public int Count => Values?.Count ?? 0;
}