using DailyPoetryHybrid.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DailyPoetryHybrid.Library.Services;

public interface IPoetryStorage
{
    bool IsInitialized { get; }

    Task InitializeAsync();

    Task<Poetry> GetPoetryAsync(int id);

    Task<IEnumerable<Poetry>> GetPoetriesAsync(Expression<Func<Poetry, bool>> where, int skip, int take);

    Task SavePoetryAsync(Poetry poetry);

    Task RemovePoetryByIdAsync(int id);

}

