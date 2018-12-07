using GPdotNet.MathStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace anndotnet.wnd
{
    /// <summary>
    /// Interaction logic for MClassEvalWnd.xaml
    /// </summary>
    public partial class MClassEvalWnd : Window
    {
        string[] m_Classes;
        double[] m_yobs = null;
        double[] m_ypre = null;

        public MClassEvalWnd()
        {
            InitializeComponent();
            this.listView1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listView1.GridLines = true;
            this.Loaded += MClassEvalWnd_Loaded;
        }

        private void MClassEvalWnd_Loaded(object sender, RoutedEventArgs e)
        {
            constructConfusionMatrix();
        }

        public void loadClasses(string[] classes)
        {
            m_Classes = classes;
        }
        public void loadData(double[] y1, double[] ytr)
        {
            m_yobs = y1;
            m_ypre = ytr;
        }

        private void constructConfusionMatrix()
        {
            var y = m_yobs;
            var yp = m_ypre;

            //add extra point 
            var o = convertToIntAray(y);
            var p = convertToIntAray(yp);

            //in case of empty values return cleaned table
            resetValues();
            
            //listView1.View.Clear();
            if (o.Length == 0 || p.Length == 0)
                return;

            var cm = new ConfusionMatrix(o, p, m_Classes.Length);


            var colHeader = new System.Windows.Forms.ColumnHeader();
            colHeader.Text = "  ";
            colHeader.Width = 250;
            listView1.Columns.Add(colHeader);

            for (int i = 0; i < m_Classes.Length; i++)
            {
                // listView1
                colHeader = new System.Windows.Forms.ColumnHeader();
                colHeader.Text = m_Classes[i];
                colHeader.Width = 150;
                listView1.Columns.Add(colHeader);
            }

            //add total column
            colHeader = new System.Windows.Forms.ColumnHeader();
            colHeader.Text = "Totals";
            colHeader.Width = 150;
            listView1.Columns.Add(colHeader);


            //  for (int i = 0; i < m_Classes.Length; i++)
            {
                var LVI1 = listView1.Items.Add($"Actual\\Predicted");
                LVI1.BackColor = System.Drawing.Color.LightSteelBlue;
                LVI1.UseItemStyleForSubItems = false;
                for (int j = 0; j < m_Classes.Length; j++)
                {
                    System.Windows.Forms.ListViewItem.ListViewSubItem itm = new System.Windows.Forms.ListViewItem.ListViewSubItem();
                    itm.BackColor = System.Drawing.Color.LightSteelBlue;
                    //itm.ForeColor = Color.Red;
                    itm.Text = $"{m_Classes[j]}";
                    LVI1.SubItems.Add(itm);
                }

                //add total
                System.Windows.Forms.ListViewItem.ListViewSubItem itm1 = new System.Windows.Forms.ListViewItem.ListViewSubItem();

                itm1.BackColor = System.Drawing.Color.LightSteelBlue;
                itm1.Text = "Totals";
                LVI1.SubItems.Add(itm1);
            }



            //insert data
            for (int i = 0; i < m_Classes.Length; i++)
            {
                var LVI2 = listView1.Items.Add(m_Classes[i]);
                LVI2.UseItemStyleForSubItems = false;
                LVI2.BackColor = System.Drawing.Color.LightSteelBlue;
                //LVI2.ForeColor = Color.Red;

                int total = 0;
                for (int j = 0; j < m_Classes.Length; j++)
                {
                    var itm = new System.Windows.Forms.ListViewItem.ListViewSubItem();
                    //itm.BackColor = SystemColors.ControlLight;
                    itm.Text = $"{cm.Matrix[i][j]}";
                    total += cm.Matrix[i][j];
                    LVI2.SubItems.Add(itm);
                }

                var itm1 = new System.Windows.Forms.ListViewItem.ListViewSubItem();
                //itm1.BackColor = SystemColors.ControlDark;
                itm1.Text = $"{total}";
                LVI2.SubItems.Add(itm1);

            }

            //insert total row
            var LVI = listView1.Items.Add("Total");
            LVI.UseItemStyleForSubItems = false;
            LVI.BackColor = System.Drawing.Color.LightSteelBlue; 

            for (int i = 0; i < m_Classes.Length; i++)
            {
                int total = 0;
                for (int j = 0; j < m_Classes.Length; j++)
                    total += cm.Matrix[j][i];

                var itm = new System.Windows.Forms.ListViewItem.ListViewSubItem();
                itm.Text = $"{total}";
                LVI.SubItems.Add(itm);
            }
            //last cell
            var itm11 = new System.Windows.Forms.ListViewItem.ListViewSubItem();
            itm11.Text = $"n={yp.Length}";
            LVI.SubItems.Add(itm11);
            listView1.Update();
            setConfusionMatrix(cm);
        }

        private void setConfusionMatrix(ConfusionMatrix cm)
        {
            int rowCount = m_yobs.Length;
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

        private int[] convertToIntAray(double[] y)
        {
            int[] retVal = new int[y.Length];
            for (int i = 0; i < y.Length; i++)
            {
                if (m_Classes.Length > 2)
                    retVal[i] = (int)y[i];
                else
                    retVal[i] = y[i] < 0.5 ? 0 : 1;
            }
            return retVal;
        }
    }
}
