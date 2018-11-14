//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using System.Windows.Input;

namespace anndotnet.wnd.commands
{
    public static class AppCommands
    {

        private static readonly RoutedUICommand m_NewCommand =
            new RoutedUICommand("Creates New ANN Model.", "NewCommand", typeof(AppCommands));

        public static RoutedCommand NewCommand
        {
            get
            {
                return m_NewCommand;
            }
        }
        private static readonly RoutedUICommand m_OpenCommand =
            new RoutedUICommand("Open existing ANN Model.", "OpenCommand", typeof(AppCommands));

        public static RoutedCommand OpenCommand
        {
            get
            {
                return m_OpenCommand;
            }
        }

        private static readonly RoutedUICommand m_SaveCommand =
            new RoutedUICommand("Save or update ANN Model on disk.", "SaveCommand", typeof(AppCommands));

        public static RoutedCommand SaveCommand
        {
            get
            {
                return m_SaveCommand;
            }
        }

        private static readonly RoutedUICommand m_SaveAsCommand =
            new RoutedUICommand("Save ANN Model on disk with prompt for file name.", "SaveAsCommand", typeof(AppCommands));

        public static RoutedCommand SaveAsCommand
        {
            get
            {
                return m_SaveAsCommand;
            }
        }

        private static readonly RoutedUICommand m_ExportCommand =
            new RoutedUICommand("Export ANN Model to Excel.", "ExportCommand", typeof(AppCommands));

        public static RoutedCommand ExportCommand
        {
            get
            {
                return m_ExportCommand;
            }
        }

        private static readonly RoutedUICommand m_RunCommand =
            new RoutedUICommand("Run ANN training.", "RunCommand", typeof(AppCommands));

        public static RoutedCommand RunCommand
        {
            get
            {
                return m_RunCommand;
            }
        }

        private static readonly RoutedUICommand m_StopCommand =
            new RoutedUICommand("Stop ANN training.", "StopCommand", typeof(AppCommands));

        public static RoutedCommand StopCommand
        {
            get
            {
                return m_StopCommand;
            }
        }

        private static readonly RoutedUICommand m_AboutCommand =
            new RoutedUICommand("Shows ANNdotNET basic information.", "AboutCommand", typeof(AppCommands));

        public static RoutedCommand AboutCommand
        {
            get
            {
                return m_AboutCommand;
            }
        }

        private static readonly RoutedUICommand m_CloseCommand =
            new RoutedUICommand("Closes current selected project.", "CloseCommand", typeof(AppCommands));

        public static RoutedCommand CloseCommand
        {
            get
            {
                return m_CloseCommand;
            }
        }

        private static readonly RoutedUICommand m_ExitCommand =
           new RoutedUICommand("Exit ANNdotNET.", "ExitCommand", typeof(AppCommands));

        public static RoutedCommand ExitCommand
        {
            get
            {
                return m_ExitCommand;
            }
        }

        private static readonly RoutedUICommand m_treeItemClickCommand =
            new RoutedUICommand("Click on Tree item.", "TreeItemClickCommand", typeof(AppCommands));

        public static RoutedCommand TreeItemClickCommand
        {
            get
            {
                return m_treeItemClickCommand;
            }
        }

        private static readonly RoutedUICommand m_LoadDataCommand =
            new RoutedUICommand("Loads Raw data in curent project.", "LoadDataCommand", typeof(AppCommands));

        public static RoutedCommand LoadDataCommand
        {
            get
            {
                return m_LoadDataCommand;
            }
        }

        private static readonly RoutedUICommand m_CreateModelCommand =
            new RoutedUICommand("Create new ANN model within the project.", "CreateModelCommand", typeof(AppCommands));

        public static RoutedCommand CreateModelCommand
        {
            get
            {
                return m_CreateModelCommand;
            }
        }

        private static readonly RoutedUICommand m_AddLayerCommand =
            new RoutedUICommand("Add Layer into network.", "AddLayerCommand", typeof(AppCommands));

        public static RoutedCommand AddLayerCommand
        {
            get
            {
                return m_AddLayerCommand;
            }
        }

        private static readonly RoutedUICommand m_RemoveLayerCommand =
            new RoutedUICommand("Delete selected layer from network.", "RemoveLayerCommand", typeof(AppCommands));

        public static RoutedCommand RemoveLayerCommand
        {
            get
            {
                return m_RemoveLayerCommand;
            }
        }
        private static readonly RoutedUICommand m_InsertLayerCommand =
            new RoutedUICommand("Insert layer above selected layer from network.", "InsertLayerCommand", typeof(AppCommands));

        public static RoutedCommand InsertLayerCommand
        {
            get
            {
                return m_InsertLayerCommand;
            }
        }
        
        private static readonly RoutedUICommand m_RenameConfigCommand =
           new RoutedUICommand("Rename MLConfig item.", "RenameConfigCommand", typeof(AppCommands));
        public static RoutedCommand RenameConfigCommand
        {
            get
            {
                return m_RenameConfigCommand;
            }
        }

        private static readonly RoutedUICommand m_DeleteConfigCommand =
            new RoutedUICommand("Delete MLConfig item.", "DeleteConfigCommand", typeof(AppCommands));

        public static RoutedCommand DeleteConfigCommand
        {
            get
            {
                return m_DeleteConfigCommand;
            }
        }

        private static readonly RoutedUICommand m_DuplicateConfigCommand =
            new RoutedUICommand("Duplicate MLConfig item.", "DuplicateConfigCommand", typeof(AppCommands));
        public static RoutedCommand DuplicateConfigCommand
        {
            get
            {
                return m_DuplicateConfigCommand;
            }
        }

        private static readonly RoutedUICommand m_EvaluateModelCommand =
            new RoutedUICommand("Evaluate MLConfig item.", "EvaluateModelCommand", typeof(AppCommands));
        public static RoutedCommand EvaluateModelCommand
        {
            get
            {
                return m_EvaluateModelCommand;
            }
        }
    }
}
