using SQLite;

namespace DailyPoetryHybrid.Library.Models;

[SQLite.Table("works")]
public class Poetry
{
    [PrimaryKey]
    [AutoIncrement]
    [SQLite.Column("id")]
    public int Id { get; set; }

    [SQLite.Column("name")]
    public string Name { get; set; } = string.Empty;

    [SQLite.Column("author_name")]
    public string Author { get; set; } = string.Empty;

    [SQLite.Column("dynasty")]
    public string Dynasty { get; set; } = string.Empty;

    [SQLite.Column("content")]
    public string Content { get; set; } = string.Empty;

    //quick view of first line.
    [SQLite.Ignore] public string Snippet => Content.Split('。')[0].Replace("\r\n", "");

}
