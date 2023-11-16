using Anndotnet.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.App.ViewModel;
using Anndotnet.Shared.Entities;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Daany;

namespace Anndotnet.App.Service
{
    public interface IWindowService
    {
        Task<bool?> ShowDialog<TViewModel, TView>(TView view, TViewModel viewModel)
            where TViewModel : DialogBaseViewModel
            where TView : UserControl;
    }
}
