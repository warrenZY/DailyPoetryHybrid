using DailyPoetryHybrid.Library.Models;
using Microsoft.VisualBasic;
using SQLite;
using System.Data.Common;
using System.Linq.Expressions;
using System.Xml.Linq;
using BootstrapBlazor.Components;
namespace DailyPoetryHybrid.Library.Services;

public class PoetryStorage : IPoetryStorage
{
    public const string DbName = "poetrydb.sqlite3";

    public static readonly string PoetryDbPath = Path.Combine(Environment
        .GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);

    private SQLiteAsyncConnection? _connection;
    private SQLiteAsyncConnection Connection => _connection ??= new SQLiteAsyncConnection(PoetryDbPath);

    private readonly IPreferenceStorage _preferenceStorage;
    private readonly IAlertService _alertService;

    public PoetryStorage(IPreferenceStorage preferenceStorage, IAlertService alertService)
    {
        _preferenceStorage = preferenceStorage;
        //make sure class will get instance

        _alertService = alertService;
    }
    
    public bool IsInitialized => _preferenceStorage.Get("Key", 0) == PoetryStorageConstant.Version;
    //see if DbVersion == 1, defalut 0.

    public async Task InitializeAsync()
    {
        await using var dbFileStream =
            new FileStream(PoetryDbPath, FileMode.OpenOrCreate);
        //auto dbFileStream.Close(); when after using. read file.
        await using var dbAssetStream =
            typeof(PoetryStorage).Assembly.GetManifestResourceStream(DbName);
        await dbAssetStream.CopyToAsync(dbFileStream);

        _preferenceStorage.Set(PoetryStorageConstant.DbVersionKey, PoetryStorageConstant.Version);

    }

    public async Task<Poetry> GetPoetryAsync(int id) =>
        await Connection.Table<Poetry>().FirstOrDefaultAsync(p => p.Id == id);


    public async Task<IEnumerable<Poetry>> GetPoetriesAsync(
        Expression<Func<Poetry, bool>> where, int skip, int take) => 
        await Connection.Table<Poetry>().Where(where).Skip(skip).Take(take).
        ToListAsync();
    /*public async Task<IEnumerable<Poetry>> GetPoetriesAsync(string name) =>
        await Connection.Table<Poetry>().Where(p => p.Name.Contains(name)).ToListAsync();
    */



    public async Task SavePoetryAsync(Poetry poetry)
    {
        int isExist = Connection.Table<Poetry>().Where(p => p.Content.Contains(poetry.Content)).CountAsync().Result;
        
        if (isExist != 0)
        {
            await _alertService.AlertToastAsync(ToastCategory.Error, "PoetryStorage Error",
                $"《{poetry.Name}》already exist, failed inserting to Database", 4000);
        }
        else
        {
            await Connection.InsertAsync(poetry);
            await _alertService.AlertToastAsync(ToastCategory.Success,"Add Succeed",
                $"Successfully added: <br/>《{poetry.Name}》 {poetry.Dynasty} · {poetry.Author}",2100);
        }

        /*
         another simple way : just try delete data with same content
        try
        {
            await Connection.Table<Poetry>().Where(p => p.Content.Contains(poetry.Content)).DeleteAsync();
        }
        catch ()
         
         */

    }


    public async Task RemovePoetryByIdAsync(int id)
    {
        try {
            await Connection.Table<Poetry>().Where(p => p.Id == id).DeleteAsync();
        } catch(Exception ex)
        {
            await _alertService.AlertToastAsync(ToastCategory.Error, "PoetryStorage Error",
                ex.Message, 4000);
        }
    }

    public async Task CloseAsync() => await Connection.CloseAsync();
}



//generate storage Constant name to avoid mistake
public static class PoetryStorageConstant
{
    public const String DbVersionKey = nameof(PoetryStorageConstant) + "." + nameof(DbVersionKey);
    //nameof(PoetryStorageConstant) -> "PoetryStorageConstant"
    //"PoetryStorageConstant.DbVersionKey"

    public const int Version = 1;
}
