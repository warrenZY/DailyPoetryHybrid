using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DailyPoetryHybrid.Library.Services
{
    public class AlertService : IAlertService
    {
        private readonly SwalService _swalService;
        private readonly ToastService _toastService;

        public AlertService(SwalService swalService, ToastService toastService)
        {
             _swalService = swalService;
            _toastService = toastService;
        }

        public async Task AlertSwalAsync(string title, string message, string button)
        {
            await _swalService.Show(new SwalOption
            {
                Category = SwalCategory.Error,
                Title = title,
                Content = message,
                CloseButtonText = button
            });
                
        }

        public async Task AlertToastAsync(
            ToastCategory category, string title, string content,int delay)
        {//	Category: Success / Information / Error / Warning
            await _toastService.Show(new ToastOption
            {
                Category = category,
                Title = title,
                Content = content,
                Delay = delay
            });
        }

    }
}
