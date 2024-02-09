using BootstrapBlazor.Components;
using DailyPoetryHybrid.Library.Models;
using Microsoft.AspNetCore.Components;

namespace DailyPoetryHybrid.Library.Pages;

public partial class Detail
{
    [Parameter]//get Id from {id}
    public string Id { get; set; }

    private Poetry _poetry = new();
    private bool _isLoadingPoetry = true;
    private bool _isLoadingFavorite = true;
    private bool _isDisabled = false;

    private Models.Favorite _favorite = new();
 
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(Id))
        {
            return;
        }
        if(!int.TryParse(Id, out var poetryId))
        {
            return;
        }
        _isLoadingPoetry = true;
        _poetry = await _poetryStorage.GetPoetryAsync(poetryId) ?? new Poetry { Id = poetryId};
        _isLoadingPoetry = false;
        //await Task.Delay(200); test delay function
        StateHasChanged();

        _favorite = await _favoriteStorage.GetFavoriteAsync(_poetry.Id) ??
            new Models.Favorite { PoetryId = _poetry.Id };
        _isLoadingFavorite = false;
        StateHasChanged();
        if (string.IsNullOrWhiteSpace(_poetry.Content))
        {
            _isDisabled = true;
            StateHasChanged();
        }
        

    }

    private async Task FavoriteValueChanged()
    {
        _isLoadingFavorite = true;
        StateHasChanged();

        await _favoriteStorage.SaveFavoriteAsync(_favorite);

        _isLoadingFavorite = false;
        StateHasChanged();
    }
}

