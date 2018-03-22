//////////////////////////////////////////////////////////////////////////////////////////
// GPdotNET Tree based genetic programming tool                                         //
// Copyright 2006-2012 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the GNU Library General Public License (LGPL)       //
// See licence section of  http://gpdotnet.codeplex.com/license                         //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac,Bosnia and Herzegovina                                                         //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;


namespace ANNdotNet.Wnd.Dll.Panels
{
    /// <summary>
    /// Panel for selecting which primitive programs will be included in to GP
    /// </summary>
    public partial class InfoPanel : UserControl
    {
        #region Ctor and Fields
        public string InfoText
        {
            get 
            {
                return richTextBox1.Rtf;
            }
            set
            {
                try
                {
                    richTextBox1.Rtf = value;
                }
                catch (Exception)
                {

                    richTextBox1.Text = value;
                }
               
            }
        }
        public InfoPanel()
        {
            InitializeComponent();
           
            if (this.DesignMode)
                return;

            richTextBox1.ShortcutsEnabled = true;
            richTextBox1.KeyDown += RichTextBox1_KeyDown;
        }

        private void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                // get clipboard.
                var obj = Clipboard.GetDataObject();

                // Get the format for the object type.
                DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Rtf);

                if (richTextBox1.CanPaste(myFormat))
                {
                    richTextBox1.Paste(myFormat);
                }

                
            }
        }
        #endregion


        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Rich text files (*.rtf)|*.rtf|All files (*.*)|*.*";


            //
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                richTextBox1.LoadFile(dlg.FileName, RichTextBoxStreamType.RichText);
            }

        }

      
    }
}
