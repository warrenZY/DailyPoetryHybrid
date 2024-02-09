using BootstrapBlazor.Components;
using DailyPoetryHybrid.Library.Models;
using Microsoft.JSInterop;

namespace DailyPoetryHybrid.Library.Pages;

public partial class Today
{
    private TodayPoetry _todayPoetry = new();
    private TodayImage _todayImage = new();

    private bool _isLoading = true;
    private bool _isShowDetail = false;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        Task.Run(async () => {
            _isLoading = true;
            await InvokeAsync(StateHasChanged);

            await Task.Delay(1000);
            _todayPoetry = await _todayPoetryService.GetTodayPoetryAsync();
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        });

        Task.Run(async () => {
            _todayImage = await _todayImageService.GetTodayImageAsync();
            await _jsRuntime.InvokeVoidAsync("setImage",
                new DotNetStreamReference(
                    new MemoryStream(_todayImage.ImageBytes)), "image");

            var updateResult = await _todayImageService.CheckUpdateAsync();
            if (updateResult.HasUpdate)
            {
                _todayImage = updateResult.TodayImage;
                await _jsRuntime.InvokeVoidAsync("setImage",
                    new DotNetStreamReference(
                        new MemoryStream(_todayImage.ImageBytes)), "image");
            }
        });
    }

    private async void SaveTodayPoetryAsync()
    {
        var poetry = new Models.Poetry()
        {
            Name = _todayPoetry.Name,
            Author = _todayPoetry.Author,
            Dynasty = _todayPoetry.Dynasty,
            Content = _todayPoetry.Content
        };

        try {
            await _poetryStorage.SavePoetryAsync(poetry);
        }catch (Exception ex)
        {
            await ToastService.Error("Add Failed", ex.Message, autoHide: true);
        }
    }
}