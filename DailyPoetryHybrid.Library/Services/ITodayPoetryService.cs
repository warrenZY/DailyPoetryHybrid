using DailyPoetryHybrid.Library.Models;

namespace DailyPoetryHybrid.Library.Services;

public interface ITodayPoetryService
{
    Task<TodayPoetry> GetTodayPoetryAsync();
}

public static class TodayPoetrySources
{
    public const string Jinrishici = nameof(Jinrishici);
    //from service API
    public const string Local = nameof(Local);
    //from local db
}
