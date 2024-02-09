using BootstrapBlazor.Components;
using DailyPoetryHybrid.Library.Models;
using DailyPoetryHybrid.Library.Services;
using System.Linq.Expressions;
using System.Reflection;

namespace DailyPoetryHybrid.Library.Pages;

public partial class Query
{
    private IList<QueryFilter> _filters = new List<QueryFilter>
    {
        new()
    };
    //List,看起来像数组，可增可减。
    //当类型需要依赖外部类型，尽量依赖抽象接口，而非具体实现。

    private IEnumerable<SelectedItem> _filterTypeItems = 
        QueryFilterType.FilterTypes.Select(p=>new SelectedItem { Text = p.Name, Value = p.PropertyName}).ToList();
    //conver each item in list FilterTypes into new object in IEnumerable<SelectedItem> 

    private void AddFilter(QueryFilter after)
    {
        _filters.Insert(_filters.IndexOf(after) + 1, new QueryFilter());
        StateHasChanged();
    }

    private void RemoveFilter(QueryFilter filter)
    {
        _filters.Remove(filter);
        if (!_filters.Any())
        {
            _filters.Add(new QueryFilter());
        }
        StateHasChanged();
    }

    private void Search()
    {
        // Connection.Table<Poetry>().Where(p => p.Name.Contains("something")
        //                                          && p.Dynasty.Contains("something")
        //                                          && p.AuthorName.Contains("something")
        //                                          && p.Content.Contains("something")
        //                                 ).ToList();

        // p => p.Name.Contains("something")
        //      && p.Dynasty.Contains("something")
        //      && p.AuthorName.Contains("something")
        //      && p.Content.Contains("something")

        // p
        var parameter = Expression.Parameter(typeof(Poetry), "p");

        var aggregatedExpression = _filters
            // Those ViewModels who do have a content.
            .Where(p => !string.IsNullOrWhiteSpace(p.Content))
            // Translate a FilterViewModel to a condition
            // e.g. FilterViewModel {
            //     FileType = {
            //         Name = "作者",
            //         PropertyName = "AuthorName"
            //     },
            //     Content = "苏轼"
            // } => p.AuthorName.Contains("苏轼")
            .Select(p => GetExpression(parameter, p))
            // Put all the conditions together
            // e.g. true && p.AuthorName.Contains("苏轼") && p.Content.Contains("老夫")
            .Aggregate(Expression.Constant(true) as Expression,
                Expression.AndAlso);

        // Turning the expression into a lambda expression
        var where = Expression.Lambda<Func<Poetry, bool>>(aggregatedExpression, parameter);

        _navigationService.NavigateTo(NavigationServiceConstants.ResultPage,
            where);
    }

    private static Expression GetExpression(ParameterExpression parameter,QueryFilter queryFilter)
    {
        // parameter => p
        // p.Name or p.Dynasty or p.Author or p.Content
        var property = Expression.Property(parameter,
            queryFilter.QueryFilterTypePropertyName);

        // .Contains()
        var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        // "something"
        var condition = Expression.Constant(queryFilter.Content, typeof(string));

        // p.Name.Contains("something")
        // or p.Dynasty.Contains("something")
        // or p.Author.Contains("something")
        // or p.Content.Contains("something")
        return Expression.Call(property, method, condition);
    }
    
}