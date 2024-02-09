using Microsoft.VisualBasic;
using System.Linq.Expressions;
using DailyPoetryHybrid.Library.Models;
using DailyPoetryHybrid.Library.Services;
using Microsoft.AspNetCore.Components;
using System.Security;
using BootstrapBlazor.Components;

namespace DailyPoetryHybrid.Library.Pages;

public partial class Result
{
    [Parameter]
    public string Ticket { get; set; } = string.Empty;

    private Expression<Func<Poetry, bool>> _where =p => true;

    public const string Loading = "正在载入";
    public const string NoResult = "没有满足条件的结果";
    public const string NoMoreResult = "已经到底了";

    private string _status = string.Empty;

    public const int PageSize = 20;

    private List<Poetry> _poetries = new();

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(Ticket))
        {
            return;
        }

        var parameter = _parcelBoxService.Get(Ticket) as Expression<Func<Poetry, bool>>;
        if (parameter is null)
        {
            return;
        }

        _poetries.Clear();//clear page data, if exist.
        _where = parameter;
        await LoadMoreAsync();
    }


    //infinite scroll
    private async Task LoadMoreAsync()
    {
        _status = Loading;
        var poetries =
            await _poetryStorage.GetPoetriesAsync(_where, _poetries.Count, PageSize);
        //load from curren shown to current+PageSize
        _poetries.AddRange(poetries);

        if(poetries.Count() < PageSize)
        {
            _status = NoMoreResult;
        }
        if(poetries.Count() == 0 && _poetries.Count == 0)
        {
            _status = NoResult;
        }
    }

    private void OnClick(Poetry poetry) =>
        _navigationService.NavigateTo($"{NavigationServiceConstants.DetailPage}/{poetry.Id}");

    private async void RemovePoetryAsync(int id,string name)
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
        }) ;
    }
}
