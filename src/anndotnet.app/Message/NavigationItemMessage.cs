using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.App.Model;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Anndotnet.App.Message;

public sealed class InsertNavigationItemMessage : ValueChangedMessage<NavigationItem>
{
    public InsertNavigationItemMessage(NavigationItem item) : base(item)
    {
    }
}

