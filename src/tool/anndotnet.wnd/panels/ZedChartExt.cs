using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
namespace anndotnet.wnd.panels
{
    public partial class ZedChartExt : ZedGraphControl
    {
        public ZedChartExt()
        {
            InitializeComponent();
            Load += ZedChartExt_Load;
        }

        private void ZedChartExt_Load(object sender, EventArgs e)
        {
            
        }

        internal void SetFont()
        {
            this.GraphPane.IsFontsScaled = false;

            this.GraphPane.XAxis.Title.FontSpec.Size = 25f;
            this.GraphPane.XAxis.Title.FontSpec.Family = "Segoe UI";
            this.GraphPane.YAxis.Title.FontSpec.Size = 25f;
            this.GraphPane.XAxis.Title.FontSpec.Family = "Segoe UI";
            this.GraphPane.Y2Axis.Title.FontSpec.Size = 25f;
            this.GraphPane.Y2Axis.Title.FontSpec.Family = "Segoe UI";
            this.GraphPane.Legend.FontSpec.Size = 25f;
            this.GraphPane.Legend.FontSpec.Family = "Segoe UI";
        }
    }
}
