using System.Runtime.InteropServices;

namespace DailyPoetryHybrid.Library.Models;

public class QueryFilterType
{
    private QueryFilterType(string name, string propertyName)
    {
        Name = name;
        PropertyName = propertyName;
    }//私有构造函数,用于创建和限制filter type,确保filter type实例变量唯一

    public static readonly QueryFilterType NameFilter = new("标题", nameof(Poetry.Name));
    public static readonly QueryFilterType DynastyFilter = new("朝代", nameof(Poetry.Dynasty));
    public static readonly QueryFilterType AuthorFilter = new("作者", nameof(Poetry.Author));
    public static readonly QueryFilterType ContentFilter = new("内容", nameof(Poetry.Content));

    public static IEnumerable<QueryFilterType> FilterTypes { get; } =
        new List<QueryFilterType> { NameFilter, DynastyFilter, AuthorFilter, ContentFilter};

    public string Name {  get; }
    public string PropertyName { get; } //字段对应，Name, Dynasty, Author, Content
}
