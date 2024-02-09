using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Use to link .NET MAUI function, 
namespace DailyPoetryHybrid.Library.Services;

public interface IPreferenceStorage
{
    void Set(string key, int value);
    int Get(string key, int defaultValue);
    void Set(string key, string value);
    string Get(string key, string defaultValue);
    void Set(string key, DateTime value);
    DateTime Get(string key, DateTime defaultValue);
}
