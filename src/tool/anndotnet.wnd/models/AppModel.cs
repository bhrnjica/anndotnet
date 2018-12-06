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

        public string AppName
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                return $"ANNdotNET v.{v.Major}.{v.Minor}-rc{DateTime.Now.Year}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Day.ToString("D2")}";
            }
        }

    }
}
