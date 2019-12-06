//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace anndotnet.wnd.Models
{
    public class AppModel
    {
        public AppModel()
        {
            Project = new ObservableCollection<BaseModel>();
        }

        public ObservableCollection<BaseModel> Project { get; set; }

        public string AppFullName
        {
            get
            {
                return $"ANNdotNET {AppVersion}";
            }
        }


        public string AppName
        {
            get
            {
                return $"ANNdotNET ";
            }
        }

        public string AppVersion
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                return $"v.{v.Major}.{v.Minor}-rc20191206";//{DateTime.Now.Year}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Day.ToString("D2")}";
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get
            {
                string desc = @"ANNdotNET - deep learning tool on .NET platform." + Environment.NewLine+
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
        public string Product
        {
            get
            {
                return "ANNdotNET";
            }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright
        {
            get
            {
                return $"Bahrudin Hrnjica, 2017-2019";
            }
        }

        /// <summary>
        /// Gets the product's company name.
        /// </summary>
        public string Company
        {
            get
            {
                return "Bahrudin Hrnjica";
            }
        }

        /// <summary>
        /// Gets the link text to display in the About dialog.
        /// </summary>
        public string LinkText
        {
            get
            {
                return "http://bhrnjica.net/anndotnet";
            }
        }

        /// <summary>
        /// Gets the link uri that is the navigation target of the link.
        /// </summary>
        public string LinkUri
        {
            get
            {
                return "http://bhrnjica.net/anndotnet";
            }
        }

    }
}
