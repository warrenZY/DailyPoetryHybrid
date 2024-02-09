using DailyPoetryHybrid.Library.Models;

namespace DailyPoetryHybrid.Library.Services;

public interface IFavoriteStorage
{
    bool IsInitialized { get; }

    Task InitializeAsync();

    Task<Favorite?> GetFavoriteAsync(int poetryId);

    Task<IEnumerable<Favorite>> GetFavoritesAsync();

    Task SaveFavoriteAsync(Favorite favorite);

    Task RemoveFavoriteByPoetryIdAsync(int poetryId);

}
