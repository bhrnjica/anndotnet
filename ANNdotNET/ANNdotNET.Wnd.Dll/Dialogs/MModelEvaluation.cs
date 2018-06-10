using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ANNdotNet.Wnd;
using GPdotNet.MathStuff;

namespace ANNdotNet.Wnd.Dialogs
{
    public partial class MModelEvaluation : Form
    {
        string[] m_Classes;
        double[] m_yobs = null;
        double[] m_ypre = null;

        double[] m_yobst = null;
        double[] m_ypret = null;

        public MModelEvaluation()
        {
            InitializeComponent();
            this.Icon = Extensions.LoadIconFromName("GPdotNet.Wnd.Dll.Images.gpdotnet.ico");
            Load += BModelEvaluation_Load;
        }
        private int[] convertToIntAray(double[] y)
        {
            int[] retVal = new int[y.Length];
            for (int i = 0; i < y.Length; i++)
            {
                retVal[i] = (int)y[i];
            }
            return retVal;
        }
        private void BModelEvaluation_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;

           // constructConfusionMatric(comboBox1.SelectedIndex < 1);
        }
        public void loadClasses(string[] classes)
        {
            m_Classes = classes;
        }
        public void loadData(double[] y1, double[] ytr, double[] y2, double[] yts)
        {
            m_yobs = y1;
            m_ypre = ytr;

            m_yobst = y2;
            m_ypret = yts;

        }

        private void constructConfusionMatric(bool isTrainingData)
        {
            var y = m_yobs;
            var yp = m_ypre;

            if (!isTrainingData)
            {
                y = m_yobst;
                yp = m_ypret;
            }
               
            //add extra point 
            var o = convertToIntAray(y);
            var p = convertToIntAray(yp);

            //in case of empty values return cleaned table
            resetValues();
            listView1.Clear();
            if (o.Length == 0 || p.Length == 0)
                return;

            var cm = new ConfusionMatrix(o, p, m_Classes.Length);

            
            var colHeader = new ColumnHeader();
            colHeader.Text = "  ";
            colHeader.Width = 250;
            listView1.Columns.Add(colHeader);

            for (int i = 0; i < m_Classes.Length; i++)
            {
                // listView1
                colHeader = new ColumnHeader();
                colHeader.Text = m_Classes[i];
                colHeader.Width = 150;
                listView1.Columns.Add(colHeader);
            }

            //add total column
            colHeader = new ColumnHeader();
            colHeader.Text = "Totals";
            colHeader.Width = 150;
            listView1.Columns.Add(colHeader);


            //  for (int i = 0; i < m_Classes.Length; i++)
            {
                var LVI1 = listView1.Items.Add($"Actual\\Predicted");
                LVI1.BackColor = SystemColors.ControlDark;
                LVI1.UseItemStyleForSubItems = false;
                for (int j = 0; j < m_Classes.Length; j++)
                {
                    System.Windows.Forms.ListViewItem.ListViewSubItem itm = new ListViewItem.ListViewSubItem();
                    itm.BackColor = SystemColors.ControlDark;
                    //itm.ForeColor = Color.Red;
                    itm.Text = $"{m_Classes[j]}";
                    LVI1.SubItems.Add(itm);
                }

                //add total
                System.Windows.Forms.ListViewItem.ListViewSubItem itm1 = new ListViewItem.ListViewSubItem();
                
                itm1.BackColor = SystemColors.ControlDark;
                itm1.Text = "Totals";
                LVI1.SubItems.Add(itm1);
            }



            //insert data
            for (int i = 0; i < m_Classes.Length; i++)
            {
                var LVI2 = listView1.Items.Add(m_Classes[i]);
                LVI2.UseItemStyleForSubItems = false;
                LVI2.BackColor = SystemColors.ControlDark;
                //LVI2.ForeColor = Color.Red;

                int total = 0;
                for (int j = 0; j < m_Classes.Length; j++)
                {
                    var itm = new ListViewItem.ListViewSubItem();
                    //itm.BackColor = SystemColors.ControlLight;
                    itm.Text = $"{cm.Matrix[i][j]}";
                    total += cm.Matrix[i][j];
                    LVI2.SubItems.Add(itm);
                }

                var itm1 = new ListViewItem.ListViewSubItem();
                //itm1.BackColor = SystemColors.ControlDark;
                itm1.Text = $"{total}";
                LVI2.SubItems.Add(itm1);

            }

            //insert total row
            var LVI = listView1.Items.Add("Total");
            LVI.UseItemStyleForSubItems = false;
            LVI.BackColor = SystemColors.ControlDark;

            for (int i = 0; i < m_Classes.Length; i++)
            {
                int total = 0;
                for (int j = 0; j < m_Classes.Length; j++)
                    total += cm.Matrix[j][i];

                var itm = new ListViewItem.ListViewSubItem();
                itm.Text = $"{total}";
                LVI.SubItems.Add(itm);
            }
            //last cell
            var itm11 = new ListViewItem.ListViewSubItem();
            itm11.Text = $"n={yp.Length}";
            LVI.SubItems.Add(itm11);
           
            setConfusionMatrix(cm, isTrainingData);
        }

        private void setConfusionMatrix(ConfusionMatrix cm, bool isTrainingData)
        {
            int rowCount = isTrainingData ? m_yobs.Length : m_yobst.Length;
            ////confusion matrix for MCC
            txOAccuracy.Text = ConfusionMatrix.OAC(cm.Matrix).ToString("F3");
            txAAccuracy.Text = (ConfusionMatrix.AAC(cm.Matrix)).ToString("F3");

            txMiPrecision.Text = ConfusionMatrix.MicroPrecision(cm.Matrix).ToString("F3");
            txMaPrecision.Text = ConfusionMatrix.MacroPrecision(cm.Matrix).ToString("F3");


            txMiRecall.Text = ConfusionMatrix.MicroRecall(cm.Matrix).ToString("F3");
            txMaRecall.Text = ConfusionMatrix.MacroRecall(cm.Matrix).ToString("F3");

            txHSS.Text = ConfusionMatrix.HSS(cm.Matrix, rowCount).ToString("F3");
            txPSS.Text = ConfusionMatrix.PSS(cm.Matrix, rowCount).ToString("F3");

        }

        void resetValues()
        {
            ////confusion matrix for MCC
            txOAccuracy.Text = "";
            txAAccuracy.Text = "";

            txMiPrecision.Text = "";
            txMaPrecision.Text = "";


            txMiRecall.Text = "";
            txMaRecall.Text = "";

            txHSS.Text = "";
            txPSS.Text = "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0 && m_yobst != null)
            {

                constructConfusionMatric(false);
            }

            else
            {
                constructConfusionMatric(true);
            }

        }
    }
}
