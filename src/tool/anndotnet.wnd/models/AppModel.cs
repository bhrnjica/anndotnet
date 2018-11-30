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
using System.Collections.ObjectModel;

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
                return "ANNdotNET v1.1";
            }
        }

    }
}
