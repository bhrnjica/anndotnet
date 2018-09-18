//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                       //
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
using System;
using System.Collections.ObjectModel;

namespace anndotnet.wnd.Models
{
    public class StartModel:BaseModel
    {
       public StartModel(Action<BaseModel, bool> fun)
           :base(fun)
        {
            Models=null;
        }
        string m_Name;
        public string  Name 
        { 
            get
            {
                return "Start Page";
            }
            set
            {
                if(value != m_Name)
                {
                    m_Name = value;
                    RaisePropertyChangedEvent("Name");
                }
            }
        }

        public new string IconUri { get => "Images/start.png"; }

        public ObservableCollection<MLConfigController> Models{get;set;}
                
    }
}
