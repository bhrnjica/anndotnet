using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Anndotnet.App.ViewModel;

public partial class DialogBaseViewModel : BaseViewModel
{
    public string? Title      { get; internal set; }
    public Window  DialogWnd { get; set; }

}
