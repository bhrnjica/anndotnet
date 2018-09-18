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
using anndotnet.wnd.Mvvm;
using System;

namespace anndotnet.wnd.Models
{
    public abstract class BaseModel : ObservableObject
    {
        public BaseModel(Action<BaseModel, bool> fun)
        {
            activeModelChanged = fun;
            
        }

        protected Action<BaseModel, bool> activeModelChanged;

        private bool m_IsEditing;
        /// <summary>
        /// indicate if certain item is expanded
        /// </summary>
        private bool m_IsExpanded;
        public bool IsExpanded 
        {
            get
            {
                return m_IsExpanded;
            }
            set
            {
                m_IsExpanded = value;
                RaisePropertyChangedEvent("IsExpanded");
            }
        }

        /// <summary>
        /// Indicate if tree item selected
        /// </summary>
        private bool m_IsSelected;
        public bool IsSelected 
        {
            get
            {
                return m_IsSelected;
            }
            set
            {
                m_IsSelected = value;
                //we need to monitor when the model is deselected in order 
                //to save current state and selected in order to load active state
                activeModelChanged(this, m_IsSelected);

                //raise notification to change selection
                RaisePropertyChangedEvent("IsSelected");
               
            }
        }

        private bool m_IsSelectionActive;
        public bool IsSelectionActive
        {
            get
            {
                return m_IsSelectionActive;
            }
            set
            {
                if (m_IsSelectionActive != value)
                {
                    m_IsSelectionActive = value;
                    //raise notification to change selection
                    RaisePropertyChangedEvent("IsSelectionActive");
                }
            }
        }

        public bool IsEditing
        {
            get
            {
                return m_IsEditing;
            }
            set
            {
                if(m_IsEditing != value)
                {
                    m_IsEditing = value;
                    //raise notification to change selection
                    RaisePropertyChangedEvent("IsEditing");
                }                
            }
        }

        /// <summary>
        /// Indicate Index of Currently selected Table page 
        /// </summary>
        int m_SelectedPage;
        public int SelectedPage
        {
            get
            {
                return m_SelectedPage;
            }
            set
            {
                m_SelectedPage = value;
                RaisePropertyChangedEvent("SelectedPage");
            }
        }

        /// <summary>
        /// Icon representation in TreeVIew control
        /// </summary>
        string m_IconUri;
        public string IconUri
        {
            get
            {
                return m_IconUri;
            }
            set
            {
                m_IconUri = value;
                RaisePropertyChangedEvent("IconUri");
            }
        }

        /// <summary>
        /// Title of the opened file
        /// </summary>
        public string TitleWindow
            {
            get
            {
                if (this is MLConfigController)
                {
                    var exp = ((MLConfigController)this);
                    return exp.Name;
                }
                else if (this is StartModel)
                    return "Start Page";
                else
                {
                    var exp = (ANNProjectController)this;
                    return exp.Name;
                }

            }

        }
    }
}
