using DailyPoetryHybrid.Library.Services;

namespace DailyPoetryHybrid.Services;

public class PreferenceStorage : IPreferenceStorage
{
    public void Set(string key, int value)=>Preferences.Set(key, value);
    public int Get(string key, int defaultValue)=>Preferences.Get(key,defaultValue);
    public void Set(string key, string value) => Preferences.Set(key, value);
    public string Get(string key, string defaultValue) => Preferences.Get(key, defaultValue);
    public void Set(string key, DateTime value) => Preferences.Set(key, value);

    public DateTime Get(string key, DateTime defaultValue)
    {
        var value = Preferences.Get(key, defaultValue);
        return value == default ? defaultValue : value;
    }
    //bridge
}
