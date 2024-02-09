using SQLite;

namespace DailyPoetryHybrid.Library.Models;

public class Favorite
{
    [PrimaryKey]
    public int PoetryId { get; set; }

    public bool IsFavorite { get; set; }

    public long Timestamp {  get; set; }
}
