using DailyPoetryHybrid.Library.Services;
using DailyPoetryHybrid.Services;
using Microsoft.Extensions.Logging;

namespace DailyPoetryHybrid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            // CodeComment
            builder.Services.AddBootstrapBlazor();
            builder.Services.AddScoped<IPoetryStorage, PoetryStorage>();
            //storage poetry data and database function
            builder.Services.AddScoped<IPreferenceStorage, PreferenceStorage>();
            //bridge to convert action to MAUI blazor Hybrid action Preferences.
            builder.Services.AddScoped<IAlertService, AlertService>();
            //send alert message in GUI, use Bootstrap Blazor.
            builder.Services.AddScoped<ITodayPoetryService, JinrishiciService>();
            //get daily poetry from server.
            builder.Services.AddScoped<INavigationService, NavigationService>();
            //display individual poetry pages.
            builder.Services.AddScoped<ITodayImageService, BingImageService>();
            //get daily image from Bing server.
            builder.Services.AddScoped<ITodayImageStorage, TodayImageStorage>();
            //storage and refresh Bing image.
            builder.Services.AddScoped<IParcelBoxService, ParcelBoxService>();
            //parcel data between different Blazor pages.
            builder.Services.AddScoped<IFavoriteStorage, FavoriteStorage>();
            return builder.Build();
        }
    }
}
