//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //
// Copyright 2017-2023 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 ////
// http://hrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Anndotnet.App.Model
{
    public class AppModel
    {
        public AppModel()
        {
            //Project = new ObservableCollection<BaseModel>();
        }

        //public ObservableCollection<BaseModel> Project { get; set; }

        public string AppFullName => $"ANNdotNET {AppVersion}";


        public string AppName => $"ANNdotNET ";

        public string AppVersion
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                return v == null ? "v.0.0" : 
                    $"v.{v.Major}.{v.Minor}-{DateTime.Now.Year}{DateTime.Now.Month:D2}{DateTime.Now.Day:D2}";
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get
            {
                var desc = @"ANNdotNET - deep learning tool on .NET platform." + Environment.NewLine+
                           "ANNdotNET uses the following third party open source libraries:" + Environment.NewLine +
                           " " + Environment.NewLine+
                           "- CNTK - Microsoft Cognitive Toolkit, with MIT License," + Environment.NewLine +
                           "- Fluent.ibbon - WPF Ribbon control like in Office , with MIT License" + Environment.NewLine +
                           "- ZedGraph - charting control, with lgpl2.1 license," + Environment.NewLine +
                           "- ClosedXML - reading and writing Excel files, with MIT license," + Environment.NewLine +
                           "- Excel-DNA - Excel add-ins using .NET, with MIT license." + Environment.NewLine
                                ;
                return desc;
            }
        }

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product => "ANNdotNET";

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright => $"Bahrudin Hrnjica, 2017-{DateTime.Now.Year}";

        /// <summary>
        /// Gets the product's company name.
        /// </summary>
        public string Company => "Bahrudin Hrnjica";

        /// <summary>
        /// Gets the link text to display in the About dialog.
        /// </summary>
        public string LinkText => "http://bhrnjica.net/anndotnet";

        /// <summary>
        /// Gets the link uri that is the navigation target of the link.
        /// </summary>
        public string LinkUri => "http://bhrnjica.net/anndotnet";
    }
}
