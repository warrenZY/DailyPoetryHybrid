namespace DailyPoetryHybrid.Library.Models;

public class QueryFilter
{
    public QueryFilterType Type { get; set; } = QueryFilterType.NameFilter;
    //Type -> *Filter
    public string Content { get; set; } = string.Empty;

    public string QueryFilterTypePropertyName
    {
        get => Type.PropertyName;
        set => Type = QueryFilterType.FilterTypes.FirstOrDefault(t => t.PropertyName == value)
                ?? QueryFilterType.NameFilter;
        //if user input filter type is invalid, then set filter type as NameFilter
    }
}
