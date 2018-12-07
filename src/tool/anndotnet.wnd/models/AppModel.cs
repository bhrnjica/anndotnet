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
                return $"v.{v.Major}.{v.Minor}-rc{DateTime.Now.Year}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Day.ToString("D2")}";
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get
            {
                string desc = @"ANNdotNET - deep learning tool on .NET platform.
                                ANNdotNET uses the folowing third party open source libraries:

                                - CNTK - Microsoft Congnitive Toolkit, with MIT License,
                                - Fluent.ibbon - WPF Ribbon control like in Office , with MIT License
                                - ZedGraph - charting control, with lgpl2.1 license,
                                - ClosedXML - reading and writing Excel files, with MIT license,
                                - Excel-DNA - Excel add-ins using .NET, with MIT license.
                                ";
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
                return "Bahrudin Hrnjica, 2017-2018";
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
