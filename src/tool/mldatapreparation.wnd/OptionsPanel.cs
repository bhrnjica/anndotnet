//////////////////////////////////////////////////////////////////////////////////////////
// GPdotNET - Genetic Programming Tool on .NET Platform                                 //
// Copyright 2006-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the GNU Library General Public License (LGPL)       //
// See license section of  https://github.com/bhrnjica/gpdotnet/blob/master/license.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DataProcessing.Core;

namespace MLDataPreparation.Dll
{

    /// <summary>
    /// Main panel for loading and defining data for validation dataset
    /// </summary>
    public partial class OptionsPanel : UserControl
    {

        #region Ctor and Fields

        public OptionsPanel()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;

            numCtrlNumForTest.Maximum = int.MaxValue;
            numCtrlNumForTest.Minimum = 0;
        }

        public (bool cntk, bool randomize, char delimiter, int testRows, bool prec) GetOptions()
        {
            char del = getDelimiter();
            return (checkBox1.Checked, randomoizeDataSet.Checked, del, (int)numCtrlNumForTest.Value, presentigeRadio.Checked);           
        }

        private char getDelimiter()
        {
            if (comboBox1.SelectedItem == null)
                return '\t';
            else if (comboBox1.SelectedItem.ToString() == "TAB")
                return '\t';
            else if (comboBox1.SelectedItem.ToString() == "COMMA")
                return ',';
            else if (comboBox1.SelectedItem.ToString() == "SAPCE")
                return ' ';
            else if (comboBox1.SelectedItem.ToString() == "SEMICOLON")
                return ';';
            else if (comboBox1.SelectedItem.ToString() == "COLON")
                return ':';
            else
                return '\t';
        }

        #endregion

        #region Private Methods

        #endregion

        #region Public Members
        /// <summary>
        /// When the Projects model is empty we should be able to reset previous state
        /// </summary>
        public void ResetExperimentalPanel()
        {
            
            numCtrlNumForTest.Value = 0;
            numberRadio.Checked = true;
        }


        #endregion

       
    }



}
