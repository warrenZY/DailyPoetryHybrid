using BootstrapBlazor.Components;
using DailyPoetryHybrid.Library.Models;
using DailyPoetryHybrid.Library.Services;
using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace DailyPoetryHybrid.Library.Pages;

public partial class Favorite
{
    private List<PoetryFavorite> _poetryFavorites = new();
    private string _status = string.Empty;
    private Models.Favorite _favorite = new();

    public const string Loading = "正在载入";
    public const string NoMoreResult = "已经到底了";

    protected override async Task OnInitializedAsync()
    {
        _status = Loading;
        _poetryFavorites.Clear();
        var favorites = await _favoriteStorage.GetFavoritesAsync();
        foreach (var _favorite in favorites)
        {
            var poetry = await _poetryStorage.GetPoetryAsync(_favorite.PoetryId);
            try
            {
                if (!string.IsNullOrWhiteSpace(poetry.Content))
                {
                    _poetryFavorites.Add(new PoetryFavorite { Favorite = _favorite, Poetry = poetry });
                    //avoid Null cause of FavoriteStorage and PoetryStorage out of sync.
                }
                else
                {
                    await _favoriteStorage.RemoveFavoriteByPoetryIdAsync(_favorite.PoetryId);
                }
            }
            catch (Exception ex)
            {
                await _favoriteStorage.RemoveFavoriteByPoetryIdAsync(_favorite.PoetryId);
                var op = new SwalOption()
                {
                    Category = SwalCategory.Error,
                    Title = "Render Error",
                    Content = $"{ex.Message}",
                    ShowClose = true,
                    IsAutoHide = false,
                };
                await SwalService.Show(op);
            }
        }
        _status = NoMoreResult;

    }

    public class PoetryFavorite
    {
        public Poetry Poetry { get; set; }
        public Models.Favorite Favorite { get; set; }
    }

    private void OnClick(Poetry poetry) =>
        _navigationService.NavigateTo($"{NavigationServiceConstants.DetailPage}/{poetry.Id}");

    private async void RemovePoetryAsync(int id, string name)
    {
        await _poetryStorage.RemovePoetryByIdAsync(id);
        await _favoriteStorage.RemoveFavoriteByPoetryIdAsync(id);
        StateHasChanged();
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Success,
            Title = "Delete Completed",
            Content = $"Successfully deleted poetry named: <br/>《{name}》. <br/>Refresh to see changes",
            Delay = 2100,
            IsAutoHide = true,
        });
    }

    private async void RemoveFavoriteAsync(int PoetryId, string name)
    {

        _favorite = await _favoriteStorage.GetFavoriteAsync(PoetryId);
        _favorite.IsFavorite = false;
        await _favoriteStorage.SaveFavoriteAsync(_favorite);
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Success,
            Title = "UnFavorite Completed",
            Content = $"Successfully UnFavorite poetry named: <br/>《{name}》. <br/>Refresh to see changes",
            Delay = 2100,
            IsAutoHide = true,
        });
    }
}