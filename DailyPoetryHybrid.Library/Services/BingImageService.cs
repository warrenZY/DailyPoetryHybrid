using DailyPoetryHybrid.Library.Models;
using System.Globalization;
using System.Text.Json;

namespace DailyPoetryHybrid.Library.Services;

public class BingImageService : ITodayImageService
{
    //1.get default image
    //2.check for image update
    private ITodayImageStorage _todayImageStorage;
    private IAlertService _alertService;

    private const string Server = "必应每日图片服务器";

    public BingImageService(ITodayImageStorage todayImageStorage,
        IAlertService alertService)
    {
        _todayImageStorage = todayImageStorage;
        _alertService = alertService;
    }

    public async Task<TodayImage> GetTodayImageAsync() =>
        await _todayImageStorage.GetTodayImageAsync(true);

    public async Task<TodayImageServiceCheckUpdateResult> CheckUpdateAsync()
    {
        var todayImage = await _todayImageStorage.GetTodayImageAsync(false);
        if (todayImage.ExpiresAt > DateTime.Now)
        {
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }

        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync("https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=zh-CN");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            await _alertService.AlertToastAsync(BootstrapBlazor.Components.ToastCategory.Error,
                ErrorMessage.HttpClientErrorTitle,
                ErrorMessage.GetHttpClientError(Server, ex.Message),
                default);
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }

        var json = await response.Content.ReadAsStringAsync();
        string bingImageUrl;
        try
        {
            var bingImage = JsonSerializer.Deserialize<BingImageOfTheDay>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })
                ?.Images?.FirstOrDefault() ?? throw new JsonException();

            var bingImageFullStartDate = DateTime.ParseExact(
                bingImage.FullStartDate ?? throw new JsonException(),
                "yyyyMMddHHmm", CultureInfo.InvariantCulture);
            //get StartDate from server json file content.

            var todayImageFullStartDate = DateTime.ParseExact(
                todayImage.FullStartDate, "yyyyMMddHHmm",
                CultureInfo.InvariantCulture);
            //get StartDate from local json file content.

            if (bingImageFullStartDate <= todayImageFullStartDate)
            {
                //local has same image with server, delay 2h to update image.
                todayImage.ExpiresAt = DateTime.Now.AddHours(2);
                await _todayImageStorage.SaveTodayImageAsync(todayImage, true);
                return new TodayImageServiceCheckUpdateResult
                {
                    HasUpdate = false
                };
            }

            todayImage = new TodayImage
            {
                FullStartDate = bingImage.FullStartDate,
                ExpiresAt = bingImageFullStartDate.AddDays(1),
                Copyright = bingImage.Copyright ?? throw new JsonException(),
                CopyrightLink = bingImage.CopyrightLink ??
                    throw new JsonException()
            };

            bingImageUrl = bingImage.Url ?? throw new JsonException();
        }
        catch (Exception ex)
        {
            await _alertService.AlertToastAsync(BootstrapBlazor.Components.ToastCategory.Error,
                ErrorMessage.JsonDeserializationErrorTitle,
                ErrorMessage.GetJsonDeserializationError(Server, ex.Message),default);
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }

        try
        {
            //get new image
            response =await httpClient.GetAsync("https://www.bing.com" + bingImageUrl);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            await _alertService.AlertToastAsync(BootstrapBlazor.Components.ToastCategory.Error,
                ErrorMessage.HttpClientErrorTitle,
                ErrorMessage.GetHttpClientError(Server, ex.Message),
                default);
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }

        todayImage.ImageBytes = await response.Content.ReadAsByteArrayAsync();
        //read content as bin arry and save.
        await _todayImageStorage.SaveTodayImageAsync(todayImage, false);
        return new TodayImageServiceCheckUpdateResult
        {
            HasUpdate = true,
            TodayImage = todayImage
        };
    }
}

public class BingImageOfTheDay
{
    public List<BingImageOfTheDayImage>? Images { get; set; }
}

public class BingImageOfTheDayImage
{
    //from json file
    public string StartDate { get; set; } = string.Empty;
    public string FullStartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Copyright { get; set; } = string.Empty;
    public string CopyrightLink { get; set; } = string.Empty;
}