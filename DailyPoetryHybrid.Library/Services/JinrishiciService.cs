using BootstrapBlazor.Components;
using DailyPoetryHybrid.Library.Models;
using System.Linq.Expressions;
using System.Text.Json;
using static DailyPoetryHybrid.Library.Services.JinrishiciService;

namespace DailyPoetryHybrid.Library.Services;
//use API from www.jinrishici.com
public class JinrishiciService : ITodayPoetryService
{
    private readonly IAlertService _alertService;
    private readonly IPreferenceStorage _preferenceStorage;
    private readonly IPoetryStorage _poetryStorage;
    private const string Server = "今日诗词服务器";

    public static readonly string JinrishiciTokenKey = $"{nameof(JinrishiciTokenKey)}.Token";

    public JinrishiciService(IAlertService alertService,
        IPreferenceStorage preferenceStorage, IPoetryStorage poetryStorage)
    {
        _alertService = alertService;
        _preferenceStorage = preferenceStorage;
        _poetryStorage = poetryStorage;
    }


    public async Task<TodayPoetry> GetTodayPoetryAsync()
    {
        var token = await GetTokenAsync();
        //得不到Token
        if(string.IsNullOrWhiteSpace(token))
        {
            await _alertService.AlertToastAsync(
                ToastCategory.Error,
                "Connection Failed",$"Failed connecting to {Server}, now using local data.",4000);
            return await GetRandomPoetryAsync();
        }//随机从数据库中取一个诗词

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-User-Token", token);
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync("https://v2.jinrishici.com/sentence");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            await _alertService.AlertToastAsync(BootstrapBlazor.Components.ToastCategory.Error,
                ErrorMessage.HttpClientErrorTitle,
                ErrorMessage.GetHttpClientError(Server, ex.Message), default);
            //得不到诗词的情况
            return await GetRandomPoetryAsync();
            //随机从数据库中取一个诗词
        }

        var json = await response.Content.ReadAsStringAsync();
        JinrishiciSentence jinrishiciSentence;
        try
        {
            jinrishiciSentence = JsonSerializer.Deserialize<JinrishiciSentence>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }catch (Exception ex)
        {
            await _alertService.AlertToastAsync(BootstrapBlazor.Components.ToastCategory.Error,
                ErrorMessage.JsonDeserializationErrorTitle,
                ErrorMessage.GetJsonDeserializationError(Server,ex.Message), default);
            return new TodayPoetry();
            //反序列化失败
        }

        return new TodayPoetry
        {
            Snippet = jinrishiciSentence.Data.Content,
            Name = jinrishiciSentence.Data.Origin.Title,
            Dynasty = jinrishiciSentence.Data.Origin.Dynasty,
            Author = jinrishiciSentence.Data.Origin.Author,
            Content = string.Join(Environment.NewLine,
            jinrishiciSentence.Data.Origin.Content),
            //use newline to combine string
            Source = TodayPoetrySources.Jinrishici
        };

    }

    public async Task<TodayPoetry> GetRandomPoetryAsync()
    {
        var poetries = await _poetryStorage.GetPoetriesAsync(
            Expression.Lambda<Func<Poetry, bool>>(Expression.Constant(true),
            Expression.Parameter(typeof(Poetry),"p")),
            new Random().Next(30),1);
        // p=> true,
        var poetry = poetries.First();
        return new TodayPoetry
        {
            Snippet = poetry.Snippet,
            Name = poetry.Name,
            Dynasty=poetry.Dynasty,
            Author = poetry.Author,
            Content = poetry.Content,
            Source = TodayPoetrySources.Local
        };
    }


    private string _token = string.Empty;

    public async Task<string> GetTokenAsync()
    {
        //1, read token from local(first RAM, second storage)
        //2, if exist, return local token
        //3, if not exist, request token from server

        //is token in RAM?
        if(!string.IsNullOrWhiteSpace(_token))
        {
            return _token;
        }

        //token not in RAM, is in storage?
        _token = _preferenceStorage.Get(JinrishiciTokenKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(_token))
        {
            return _token;
        }

        //token not in RAM and storage, get from server
        using var HttpClient = new HttpClient();
        HttpResponseMessage response;
        try
        {
            response = await HttpClient.GetAsync("https://v2.jinrishici.com/token");
            //should be like {"status":"success","data":"1Zph0fVJmkutd0T1g7flBlYNP4r563C0"}
            response.EnsureSuccessStatusCode();
            //http 200
        }catch (Exception ex)
        {
            await _alertService.AlertToastAsync(
                ToastCategory.Error,
                ErrorMessage.HttpClientErrorTitle,
                ErrorMessage.GetHttpClientError(Server, ex.Message),
                default
                );
            return _token;
        }

        var json = await response.Content.ReadAsStringAsync();
        JinrishiciToken jinrishiciToken;
        try
        {
            jinrishiciToken = JsonSerializer.Deserialize<JinrishiciToken>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
            //convert to instance
            
        }catch (Exception ex)
        {
            await _alertService.AlertToastAsync(
                BootstrapBlazor.Components.ToastCategory.Error,
                ErrorMessage.JsonDeserializationErrorTitle,
                ErrorMessage.GetJsonDeserializationError(Server, ex.Message),
                default
                );
            return _token;
        }

        _token = jinrishiciToken.Data;
        _preferenceStorage.Set(JinrishiciTokenKey, _token);
        return _token;

    }

    public class JinrishiciToken
    {
        public string Status { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }

    public class JinrishiciSentence
    {
        public JinrishiciData Data { get; set; }
    }


    public class JinrishiciData
    {
        public string Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public JinrishiciOrigin Origin { get; set; }
    }

    public class JinrishiciOrigin
    {
        public string Title { get; set; } = string.Empty;
        public string Dynasty { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string[] Content { get; set; }
    }
}
