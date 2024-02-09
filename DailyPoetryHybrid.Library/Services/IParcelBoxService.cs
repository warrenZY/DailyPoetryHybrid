namespace DailyPoetryHybrid.Library.Services;

public interface IParcelBoxService
{
    string Put(object o);

    object Get(string ticket);
}