using BootstrapBlazor.Components;

namespace DailyPoetryHybrid.Library.Services;

public interface IAlertService
{
    Task AlertSwalAsync(string title, string message, string button);
    Task AlertToastAsync(ToastCategory category, string title, string content, int delay);

}
