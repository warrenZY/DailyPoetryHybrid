using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyPoetryHybrid.Library.Services;
using Moq;
namespace DailyPoetryHybrid.UnitTest.Services;

public class PoetryStorageTest : IDisposable
{
    public PoetryStorageTest() => PoetryStorageHelper.RemoveDataBaseFile();

    public Dispose() => PoetryStorageHelper.RemoveDataBaseFile();
    //[Fact]
    public void IsInitialized_Initialized()
    {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock.Setup(p => p.Get(PoetryStorageConstant.DbVersionKey, 0))
            .Returns(PoetryStorageConstant.Version);

        var mockPreferenceStorage = preferenceStorageMock.Object;
        var poetryStorage = new PoetryStorage(mockPreferenceStorage);
        //通过构造函数获得实例
        Assert.True(poetryStorage.IsInitialized);
    }

    [Fact]
    public void IsInitialized_NotInitialized()
    {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock.Setup(p => p.Get(PoetryStorageConstant.DbVersionKey, 0))
            .Returns(0);

        var mockPreferenceStorage = preferenceStorageMock.Object;
        var poetryStorage = new PoetryStorage(mockPreferenceStorage);
        //通过构造函数获得实例
        Assert.False(poetryStorage.IsInitialized);
    }

    [Fact]
    public async Task InitializedAsync_Default()
    {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;

        var poetryStorage = new PoetryStorage(mockPreferenceStorage);

        Assert.False(File.Exists(PoetryStorage.PoetryDbPath));
        await poetryStorage.InitializeAsync();
        Assert.True(File.Exists(PoetryStorage.PoetryDbPath));
        preferenceStorageMock.Verify(
            p=>p.Set(PoetryStorageConstant.DbVersionKey, PoetryStorageConstant.Version),Times.Once);
    }

    
    public static void Clean()
    {
        PoetryStorageHelper.RemoveDataBaseFile();
    }
}
