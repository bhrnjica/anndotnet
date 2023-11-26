using Anndotnet.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.Shared.Entities;
using Avalonia;
using Avalonia.Platform.Storage;
using Daany;

namespace Anndotnet.App.Service
{
    public interface IDialogService
    {

        Task<IStorageFile?> FileOpen(string title, string extension);
        Task<IStorageFile>  FileSave(string title, string extension, string suggestedFileName);

        public Task<IStorageFolder> OpenFolder(string title);
    }
}
