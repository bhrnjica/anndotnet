using Anndotnet.App.Model;
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
namespace Anndotnet.App.Service;

public class DialogService : IDialogService
{
    private readonly List<FilePickerFileType> _fileTypes = new List<FilePickerFileType>
                                                          {
                                                              new("Project file (.ann)")
                                                              {
                                                                  Patterns = new[] { "*.ann" },
                                                                  MimeTypes = new[] { "json/*" },
                                                                  AppleUniformTypeIdentifiers = new[] { "utf8PlainText" }
                                                              },
                                                              new("MLConfig file (.mlconfig)")
                                                              {
                                                                  Patterns = new[] { "*.mlconfig" },
                                                                  MimeTypes = new[] { "json/*" },
                                                                  AppleUniformTypeIdentifiers = new[] { "utf8PlainText" }
                                                              },
                                                              new("ANN data files (txt)")
                                                              {
                                                                  Patterns = new[] {"*.txt","*.csv","*.dat" },
                                                                  MimeTypes = new[] { "text/*" },
                                                                  AppleUniformTypeIdentifiers = new[] { "utf8PlainText" }
                                                              },
                                                              FilePickerFileTypes.ImageAll,
                                                              FilePickerFileTypes.ImagePng,
                                                              FilePickerFileTypes.Pdf,
                                                              FilePickerFileTypes.All
                                                          };

    private readonly MainWindow _mainWindow;

    public DialogService(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public async Task<IStorageFile?> FileOpen(string title, string extension)
    {
        var fileType = ResolveFileType(extension);
        var topLevel = TopLevel.GetTopLevel(_mainWindow);

        if (topLevel == null)
        {
            throw new InvalidOperationException("Could not find top level.");   
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                                                                       {
                                                                           Title = title,
                                                                           AllowMultiple = false,
                                                                           
                                                                           FileTypeFilter = fileType,
                                                                        });

        if (files.Count >= 1)
        {
            return files[0];
        }
        else
        {
            
            return null;        
        }
    }


    public async Task<IStorageFile> FileSave(string title, string extension, string suggestedFileName)
    {
        var fileType = ResolveFileType(extension);
        var topLevel = TopLevel.GetTopLevel(_mainWindow);

        if (topLevel == null)
        {
            throw new InvalidOperationException("Could not find top level.");
        }

        // Start async operation to open the dialog.
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                                                                      { 
                                                                          Title = title,
                                                                          DefaultExtension = extension,
                                                                          SuggestedFileName = suggestedFileName,
                                                                          ShowOverwritePrompt = true,
                                                                          
                                                                          FileTypeChoices = fileType,
                                                                      });
        return file!;
        
    }

    private IReadOnlyList<FilePickerFileType> ResolveFileType(string extension)
    {
        return _fileTypes.Where(x=>x.Name.Contains(extension) || x.Name.Equals("All")).ToList();
    }
}

