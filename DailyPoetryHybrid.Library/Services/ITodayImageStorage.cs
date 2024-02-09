using DailyPoetryHybrid.Library.Models;
namespace DailyPoetryHybrid.Library.Services;

public interface ITodayImageStorage
{
    Task<TodayImage> GetTodayImageAsync(bool includingImageStream);
    Task SaveTodayImageAsync(TodayImage todayImage, bool savingExpiresAtOnly);
}
