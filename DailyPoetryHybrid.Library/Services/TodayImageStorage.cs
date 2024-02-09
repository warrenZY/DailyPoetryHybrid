using DailyPoetryHybrid.Library.Models;
namespace DailyPoetryHybrid.Library.Services;

public class TodayImageStorage : ITodayImageStorage
{
    private readonly IPreferenceStorage _preferenceStorage;

    public TodayImageStorage(IPreferenceStorage preferenceStorage)
    {
        _preferenceStorage = preferenceStorage;
    }

    public static readonly string FullStartDateKey = nameof(TodayImageStorage) +
        "." + nameof(TodayImage.FullStartDate);

    public static readonly string ExpiresAtKey =
        nameof(TodayImageStorage) + "." + nameof(TodayImage.ExpiresAt);

    public static readonly string CopyrightKey =
        nameof(TodayImageStorage) + "." + nameof(TodayImage.Copyright);

    public static readonly string CopyrightLinkKey = nameof(TodayImageStorage) +
        "." + nameof(TodayImage.CopyrightLink);

    public const string FullStartDateDefault = "202312150700";

    public static readonly DateTime ExpiresAtDefault = new(2023, 12, 16, 7, 0, 0);

    public const string CopyrightDefault = "Mountains snow slope cold forest (© Andy_Bay/Pixabay)";

    public const string CopyrightLinkDefault =
        "https://pixabay.com/photos/mountains-snow-slope-cold-forest-8446221/";

    public const string FileName = "TodayImageDefault.jpg.bin";

    public static readonly string TodayImagePath =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder
                .LocalApplicationData), FileName);

    public async Task<TodayImage> GetTodayImageAsync(bool includingImageStream)
    {
        var todayImage = new TodayImage
        {
            FullStartDate = _preferenceStorage.Get(FullStartDateKey, FullStartDateDefault),
            ExpiresAt = _preferenceStorage.Get(ExpiresAtKey, ExpiresAtDefault),
            Copyright = _preferenceStorage.Get(CopyrightKey, CopyrightDefault),
            CopyrightLink = _preferenceStorage.Get(CopyrightLinkKey, CopyrightLinkDefault)
        };

        if (!File.Exists(TodayImagePath))
        {
            await using var imageAssetFileStream =
                new FileStream(TodayImagePath, FileMode.Create) ?? throw new NullReferenceException("Null file stream.");
            await using var imageAssetStream =
                typeof(TodayImageStorage).Assembly.GetManifestResourceStream(
                    FileName) ?? throw new NullReferenceException("Null manifest resource stream");
            await imageAssetStream.CopyToAsync(imageAssetFileStream);
        }

        if (!includingImageStream)
        {
            return todayImage;
        }

        var imageMemoryStream = new MemoryStream();
        await using var imageFileStream = new FileStream(TodayImagePath, FileMode.Open);
        await imageFileStream.CopyToAsync(imageMemoryStream);
        todayImage.ImageBytes = imageMemoryStream.ToArray();

        return todayImage;
    }

    public async Task SaveTodayImageAsync(TodayImage todayImage,
        bool savingExpiresAtOnly)
    {
        _preferenceStorage.Set(ExpiresAtKey, todayImage.ExpiresAt);
        if (savingExpiresAtOnly)
        {
            return;
        }

        if (todayImage.ImageBytes == null)
        {
            throw new ArgumentException($"Null image bytes.",nameof(todayImage));
        }

        _preferenceStorage.Set(FullStartDateKey, todayImage.FullStartDate);
        _preferenceStorage.Set(CopyrightKey, todayImage.Copyright);
        _preferenceStorage.Set(CopyrightLinkKey, todayImage.CopyrightLink);

        await using var imageFileStream = new FileStream(TodayImagePath, FileMode.Create);
        await imageFileStream.WriteAsync(todayImage.ImageBytes, 0, todayImage.ImageBytes.Length);
    }
}
