namespace DailyPoetryHybrid.Library.Services;

public interface INavigationService
{
    void NavigateTo(string uri);
    void NavigateTo(string uri, object parameter);
}

public static class NavigationServiceConstants
{
    public const string DetailPage = "/detail";
    public const string ResultPage = "/result";
    public const string TodayPage = "/today";
    public const string InitializationPage = "/initialization";
}