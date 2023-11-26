﻿using Anndotnet.App.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.Shared.Entities;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Daany;
using Avalonia.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Anndotnet.App.ViewModel;
namespace Anndotnet.App.Service;

public class WindowService : IWindowService
{
    private readonly MainWindow _mainWindow;

    public WindowService(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public async Task<bool?> ShowDialog<TViewModel, TView>(TViewModel viewModel, TView view) where TViewModel : DialogBaseViewModel
                                                                                             where TView : UserControl
    {
        var dlg = new Window
                  {
                   // DataContext = viewModel,
                    Content = view,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    CanResize = true,
                    Title = viewModel.Title,
                    WindowState = WindowState.Normal,
                    Topmost = true,
                    ShowInTaskbar = false,
                  //  HasSystemDecorations = false,
                    ShowActivated = true,
                    MinWidth = 800,
                    MinHeight = 400,
                    MaxWidth = 1200,
                    MaxHeight = 900,
                    
                    
                };

        viewModel.DialogWnd=dlg;
        
        var retValue = await dlg.ShowDialog<bool?>(_mainWindow);
        dlg.Close();
        return retValue;
    }
}
