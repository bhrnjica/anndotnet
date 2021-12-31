﻿using DataProcessing.Core;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace DataProcessing.Wnd
{
  

    /// <summary>
    /// Interaction logic for DataPanelWPF.xaml
    /// </summary>
    public partial class DataPanelWPF : System.Windows.Controls.UserControl
    {
        //listview items
        private System.Windows.Forms.ListViewItem li;
        private int X = 0;
        private int Y = 0;
        private int subItemSelected = 0;
        private System.Windows.Forms.ComboBox cmbBox1 = new System.Windows.Forms.ComboBox();
        private System.Windows.Forms.ComboBox cmbBox2 = new System.Windows.Forms.ComboBox();
        private System.Windows.Forms.ComboBox cmbBox3 = new System.Windows.Forms.ComboBox();

        private string[][] m_strData; //loaded string of data
        private string[] m_strHeader; //loaded string of data
        public Action<bool> UpdateModel { get; set; }
        public Action<bool> CreateModel { get; set; }

        public DataPanelWPF()
        {
            InitializeComponent();

            this.listView1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));

            //first row combobox
            cmbBox1.Items.Add(ColumnType.Numeric.Description());
            cmbBox1.Items.Add(ColumnType.Category.Description());
            cmbBox1.Items.Add(ColumnType.None.Description());
            cmbBox1.DropDownClosed += CmbBox1_DropDownClosed;
            cmbBox1.Size = new System.Drawing.Size(0, 0);
            cmbBox1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Controls.AddRange(new System.Windows.Forms.Control[] { this.cmbBox1 });
            cmbBox1.SelectedIndexChanged += new System.EventHandler(this.CmbSelected);
            cmbBox1.LostFocus += new System.EventHandler(this.CmbFocusOver);
            cmbBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CmbKeyPress);
            cmbBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbBox1.Hide();

            //second row combobox
            cmbBox2.Items.Add(VariableType.None.Description());
            cmbBox2.Items.Add(VariableType.Feature.Description());
            cmbBox2.Items.Add(VariableType.Label.Description());
            cmbBox2.DropDownClosed += CmbBox1_DropDownClosed;
            cmbBox2.Size = new System.Drawing.Size(0, 0);
            cmbBox2.Location = new System.Drawing.Point(0, 0);
            this.listView1.Controls.AddRange(new System.Windows.Forms.Control[] { this.cmbBox2 });
            cmbBox2.SelectedIndexChanged += new System.EventHandler(this.CmbSelected);
            cmbBox2.LostFocus += new System.EventHandler(this.CmbFocusOver);
            cmbBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CmbKeyPress);
            cmbBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbBox2.Hide();


            //forth row combo box
            cmbBox3.Items.Add(MissingValue.None.Description());
            cmbBox3.Items.Add(MissingValue.Ignore.Description());
            cmbBox3.Items.Add(MissingValue.Average.Description());
            cmbBox3.Items.Add(MissingValue.Max.Description());
            cmbBox3.Items.Add(MissingValue.Min.Description());
            cmbBox3.Items.Add(MissingValue.Mode.Description());
            cmbBox3.Items.Add(MissingValue.Random.Description());
            cmbBox3.DropDownClosed += CmbBox1_DropDownClosed;

            cmbBox3.Size = new System.Drawing.Size(0, 0);
            cmbBox3.Location = new System.Drawing.Point(0, 0);
            this.listView1.Controls.AddRange(new System.Windows.Forms.Control[] { this.cmbBox3 });
            cmbBox3.SelectedIndexChanged += new System.EventHandler(this.CmbSelected);
            cmbBox3.LostFocus += new System.EventHandler(this.CmbFocusOver);
            cmbBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CmbKeyPress);
            cmbBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbBox3.Hide();
        }

        private void CmbBox1_DropDownClosed(object sender, EventArgs e)
        {
            var cd = sender as System.Windows.Forms.ComboBox;
            if (cd != null)
                cd.Hide();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]*(?:\.[0-9]*)?$");
        }


        //implementation Header control 
        #region Cell ComboBox Events
        private void CmbKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            var combo = sender as System.Windows.Forms.ComboBox;

            if (e.KeyChar == 13 || e.KeyChar == 27)
            {
                combo.Hide();
            }
        }

        private void CmbFocusOver(object sender, EventArgs e)
        {
            var combo = sender as System.Windows.Forms.ComboBox;
            combo.Hide();
        }

        private void CmbSelected(object sender, EventArgs e)
        {
            var combo = sender as System.Windows.Forms.ComboBox;

            int sel = combo.SelectedIndex;
            if (sel >= 0)
            {
                string itemSel = combo.Items[sel].ToString();
                li.SubItems[subItemSelected].Text = itemSel;
                var cols = ParseHeader();
                setSummary(m_strData, cols.ToList());
            }
        }
        #endregion



        #region Private Methods
        /// <summary>
        /// Fill Table with data 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="data"></param>
        public void FillDataGrid(string[] header, string[][] data)
        {
            m_strHeader = header;
            m_strData = data;

            //clear the list first
            listView1.Clear();
            listView1.GridLines = true;
            listView1.HideSelection = false;
            if (data == null)
                return;
            int numRow = data.Length;
            int numCol = data[0].Length;

            //inspect the data
            var cols = parseData(data, header);

            //header
            setColumn(cols);
            //setDefaultColumns(header, numCol);

            //insert data
            setData(data.Select(x => x.ToList()).ToList());

            //set summary
            setSummary(m_strData, cols);
        }

       
        /// <summary>
        /// Handling double mouse click for changing MetaData info of the loaded data columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.ListViewHitTestInfo info = listView1.HitTest(X, Y);
            var row = info.Item.Index;
            var col = info.Item.SubItems.IndexOf(info.SubItem);
            var colType = listView1.Items[3].SubItems[col];
            li = info.Item;
            subItemSelected = col;
            //only first and second Row process the mouse input 
            if (li == null || row > 3 || row < 1 || col < 1)
                return;

            System.Windows.Forms.ComboBox combo = null;
            if (row == 1)
                combo = cmbBox1;
            else if (row == 2)
            {
                combo = cmbBox2;
            }
            else if (row == 3)
                combo = cmbBox3;
            else
                combo = cmbBox1;

            var subItm = li.SubItems[col];
            if (combo != null)
            {
                combo.Bounds = subItm.Bounds;
                combo.Show();
                combo.Text = subItm.Text;
                combo.SelectAll();
                combo.Focus();
            }

        }

        private void ListView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
            X = e.X;
            Y = e.Y;

            // Console.WriteLine(string.Format("Row={0}:Col={1} val='{2}'", row, col, value));
        }

        /// <summary>
        /// Fill ListView with proper columns
        /// </summary>
        /// <param name="cols"></param>
        private void setColumn(List<MetaColumn> cols)
        {
            //clear the list first
            listView1.Clear();
            listView1.GridLines = true;
            listView1.HideSelection = false;

            int numCol = cols.Count;
            //int numRow = 5;

            System.Windows.Forms.ColumnHeader colHeader = null;
            colHeader = new System.Windows.Forms.ColumnHeader();
            colHeader.Text = " ";
            colHeader.Width = 200;
            listView1.Columns.Add(colHeader);
            //
            for (int i = 0; i < numCol; i++)
            {
                colHeader = new System.Windows.Forms.ColumnHeader();
                colHeader.Text = cols[i].Name;
                colHeader.Width = 200;
                colHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

                listView1.Columns.Add(colHeader);
            }
            //first row is going to represent column names
            System.Windows.Forms.ListViewItem LVI = listView1.Items.Add("Column name:");
            for (int i = 0; i < numCol; i++)
            {
                LVI.SubItems.Add(cols[i].Name);
                LVI.BackColor = System.Drawing.SystemColors.MenuHighlight;

            }

            //second row is going to represent the type of each column (input parameter, output variable)
            LVI = listView1.Items.Add(MetaData.CType.Description());
            for (int i = 0; i < numCol; i++)
            {
                //LVI.SubItems.Add("numeric");
                LVI.SubItems.Add(cols[i].Type);
                LVI.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            }

            //second row is going to represent is the column input, output or ignored column
            LVI = listView1.Items.Add(MetaData.Variable.Description());
            for (int i = 0; i < numCol; i++)
            {
                LVI.SubItems.Add(cols[i].Param);
                LVI.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            }

            //forth row is going to represent missing values action
            LVI = listView1.Items.Add(MetaData.MissingValue.Description());
            for (int i = 0; i < numCol; i++)
            {
                //LVI.SubItems.Add("Ignore");
                LVI.SubItems.Add(cols[i].MissingValue);
                LVI.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            }
        }

        private List<MetaColumn> parseData(string[][] data, string[] header)
        {

            var cols = new List<MetaColumn>();
            var columnData = data.toColumnVector<string>();
            //in case no header is provided
            if (header == null|| header.Length==0)
            {
                header = Enumerable.Range(1, data[0].Length).Select(x => $"Column{x}").ToArray();
                m_strHeader = header;
            }

            //
            for (int i = 0; i < header.Length; i++)
            {
                var mc = new MetaColumn();
                mc.Id = i;
                mc.Index = i;

                mc.Name = header[i];

                //determine how many different values column has
                var classes = columnData[i].Distinct().Count();
                var count = data.Length;

                //type
                if (classes < 5)
                    mc.Type = ColumnType.Category.ToString();
                else
                    mc.Type = ColumnType.Numeric.ToString();

                //encoding 
                if (mc.Type == ColumnType.Category.ToString())
                    mc.Encoding = CategoryEncoding.OneHot.ToString();
                else
                    mc.Encoding = CategoryEncoding.None.ToString();


                //default
                mc.MissingValue = MissingValue.Ignore.ToString();
                mc.Scale = Scaling.None.ToString();

                //The last column is label by default
                if (i + 1 == header.Length)
                    mc.Param = VariableType.Label.ToString();
                else
                    mc.Param = VariableType.Feature.ToString();

                cols.Add(mc);
            }

            //
            return cols;
        }
        /// <summary>
        /// Set default columns
        /// </summary>
        /// <param name="header"></param>
        /// <param name="numCol"></param>
        private void setDefaultColumns(string[] header, int numCol)
        {
            var cols = new List<MetaColumn>();
            for (int i = 0; i < numCol; i++)
            {
                var mc = new MetaColumn();
                mc.Encoding = CategoryEncoding.None.ToString();
                mc.Id = i;
                mc.Index = i;
                mc.MissingValue = MissingValue.Ignore.ToString();
                mc.Scale = Scaling.None.ToString();
                mc.Type = ColumnType.Numeric.ToString();


                if (header == null)
                {
                    if (i + 1 == numCol)
                    {
                        mc.Name = "y";
                        mc.Param = VariableType.Label.ToString();
                    }

                    else
                    {
                        mc.Name = "x" + (i + 1).ToString();
                        mc.Param = VariableType.Feature.ToString();
                    }

                }
                else
                {
                    mc.Name = header[i];
                    if (i + 1 == numCol)
                        mc.Param = VariableType.Label.ToString();
                    else
                        mc.Name = "x" + (i + 1).ToString();
                }
            }
            ///
            System.Windows.Forms.ColumnHeader colHeader = null;
            colHeader = new System.Windows.Forms.ColumnHeader();
            colHeader.Text = " ";
            colHeader.Width = 150;
            listView1.Columns.Add(colHeader);
            //
            for (int i = 0; i < numCol; i++)
            {
                colHeader = new System.Windows.Forms.ColumnHeader();

                if (header == null)
                {
                    if (i + 1 == numCol)
                        colHeader.Text = "y";
                    else
                        colHeader.Text = "x" + (i + 1).ToString();
                }
                else
                    colHeader.Text = header[i];


                colHeader.Width = 200;
                colHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

                listView1.Columns.Add(colHeader);
            }
            //first row is going to represent column names
            System.Windows.Forms.ListViewItem LVI = listView1.Items.Add(MetaData.Name.Description());
            for (int i = 0; i < numCol; i++)
            {
                if (header == null)
                {
                    if (i + 1 == numCol)
                        LVI.SubItems.Add("y");
                    else
                        LVI.SubItems.Add("x" + (i + 1).ToString());
                }
                else
                    LVI.SubItems.Add(header[i]);


                LVI.BackColor = System.Drawing.SystemColors.MenuHighlight;

            }

            //second row is going to represent the type of each column (input parameter, output variable)
            LVI = listView1.Items.Add(MetaData.CType.Description());
            for (int i = 0; i < numCol; i++)
            {
                LVI.SubItems.Add(ColumnType.Numeric.Description());
                LVI.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            }

            //fourth row is going to represent is the column input, output or ignored column
            LVI = listView1.Items.Add(MetaData.Variable.Description());
            for (int i = 0; i < numCol; i++)
            {
                if (i + 1 >= numCol)
                    LVI.SubItems.Add(VariableType.Label.Description());
                else
                    LVI.SubItems.Add(VariableType.Feature.Description());

                LVI.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            }

            //sixth row is going to represent missing values action
            LVI = listView1.Items.Add(MetaData.MissingValue.Description());
            for (int i = 0; i < numCol; i++)
            {
                LVI.SubItems.Add(MissingValue.Ignore.Description());

                LVI.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            }
        }


        private void setData(List<List<string>> data)
        {
            if (data == null)
                return;
            int numCol = data[0].Count;
            int numRow = data.Count > 10 ? 10 : data.Count;
            System.Windows.Forms.ListViewItem LVI = null;
            //insert data
            for (int j = 0; j < numRow; j++)
            {
                LVI = listView1.Items.Add((j + 1).ToString());
                LVI.UseItemStyleForSubItems = false;
                for (int i = 0; i < numCol; i++)
                {
                    if (ColumnData.m_missingSymbols.Contains(data[j][i]))
                    {
                        System.Windows.Forms.ListViewItem.ListViewSubItem itm = new System.Windows.Forms.ListViewItem.ListViewSubItem();
                        itm.ForeColor = System.Drawing.Color.Red;
                        itm.Text = data[j][i];
                        LVI.SubItems.Add(itm);
                    }

                    else
                        LVI.SubItems.Add(data[j][i].ToString());
                }

            }
            m_strData = data.Select(x => x.ToArray()).ToArray();
            return;
        }

        private void setSummary(string[][] data, List<MetaColumn> cols)
        {
            if (data == null)
                return;
            int numCol = data[0].Length;
            int numRow = data.Length;
            System.Windows.Forms.ListViewItem LVI = null;
            var toColumnData = data.toColumnVector<string>();

            if (listView1.Items.Count <= 14)
                LVI = listView1.Items.Add("...");
            else
                LVI = listView1.Items[14];
            LVI.UseItemStyleForSubItems = false;
            //LVI.BackColor = SystemColors.GradientActiveCaption;
            for (int i = 0; i < numCol; i++)
            {
                System.Windows.Forms.ListViewItem.ListViewSubItem itm = new System.Windows.Forms.ListViewItem.ListViewSubItem();
                //
                itm.Text = "...";
                //
                if (LVI.SubItems.Count > i + 1)
                    LVI.SubItems[i + 1] = itm;
                else
                    LVI.SubItems.Add(itm);

            }

            //insert summary data
            //class values if available
            if (listView1.Items.Count <= 15)
                LVI = listView1.Items.Add("Row Count");
            else
                LVI = listView1.Items[15];

            LVI.UseItemStyleForSubItems = false;
            LVI.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            for (int i = 0; i < numCol; i++)
            {
                System.Windows.Forms.ListViewItem.ListViewSubItem itm = new System.Windows.Forms.ListViewItem.ListViewSubItem();
                itm.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
                itm.Text = toColumnData[i].Count().ToString();
                //
                if (LVI.SubItems.Count > i + 1)
                    LVI.SubItems[i + 1] = itm;
                else
                    LVI.SubItems.Add(itm);
            }

            //insert summary data
            //class values if available
            if (listView1.Items.Count <= 16)
                LVI = listView1.Items.Add("Class Count");
            else
                LVI = listView1.Items[16];
            LVI.UseItemStyleForSubItems = false;
            LVI.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            for (int i = 0; i < numCol; i++)
            {
                System.Windows.Forms.ListViewItem.ListViewSubItem itm = new System.Windows.Forms.ListViewItem.ListViewSubItem();
                itm.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
                if (cols.Count > 0 && cols[i].Type.StartsWith("Numeric", StringComparison.InvariantCulture))
                    itm.Text = "n/a";
                else
                    itm.Text = toColumnData[i].Distinct().Count().ToString();
                //
                if (LVI.SubItems.Count > i + 1)
                    LVI.SubItems[i + 1] = itm;
                else
                    LVI.SubItems.Add(itm);
            }

            return;
        }
        private MetaColumn[] ParseHeader(bool omitIgnored = false)
        {

            var lst = new List<MetaColumn>();

            if (listView1.Items.Count == 0)
                return null;
            //f name of the columns
            var firstRow = listView1.Items[0];
            var secondRow = listView1.Items[1];
            var thirdRow = listView1.Items[2];
            var forthRow = listView1.Items[3];


            for (int i = 1; i < firstRow.SubItems.Count; i++)
            {
                int colIndex = i - 1;
                string colName = firstRow.SubItems[i].Text;
                string colType = secondRow.SubItems[i].Text;
                string variableType = thirdRow.SubItems[i].Text;
                string missingValue = forthRow.SubItems[i].Text;


                var col = new MetaColumn() { Id = colIndex, Index = colIndex, Name = colName, Type = colType, Param = variableType, MissingValue = missingValue };
                if (!col.IsIngored || !omitIgnored)
                    lst.Add(col);
            }


            return lst.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="omitIgnored"></param>
        /// <returns></returns>
        private List<List<string>> ParseData(MetaColumn[] metaCols)
        {
            if (metaCols == null)
                return null;
            var data = new List<List<string>>();
            //string[][] data = new string[this.m_strData.Length][];

            for (int k = 0; k < this.m_strData.Length; k++)
            {
                var i = k;
                var row = m_strData[i];

                //calculate number of columns
                int col = row.Length;
                var rowData = new List<string>();
                for (int j = 0; j < metaCols.Length; j++)
                {
                    rowData.Add(row[metaCols[j].Index]);
                }
                data.Add(rowData);
            }


            return data;
        }
        #endregion

        #region Public Members
        /// <summary>
        /// When the Projects model is empty we should be able to reset previous state
        /// </summary>
        public void ResetExperimentalPanel()
        {
            listView1.Clear();

            txtValidationCount.Text = "0";
            txtTestCount.Text = "0";
            txtTestCount.Text = "0";
            radionPercentige.IsChecked = true;
        }

        public void SetDataSet(ANNDataSet dataSet)
        {
            if (dataSet == null || dataSet.Data == null || dataSet.MetaData == null)
            {
                ResetExperimentalPanel();
                return;
            }
            //header
            m_strHeader = dataSet.MetaData.Select(x => x.Name).ToArray();
            setColumn(dataSet.MetaData.ToList());
            //Data
            setData(dataSet.Data);
            //summary
            setSummary(m_strData, dataSet.MetaData.ToList());

            //set 
            //txtValidationCount.Text = dataSet.TestRows.ToString();
            txtValidationCount.Text = dataSet.RowsToValidation.ToString();
            txtTestCount.Text = dataSet.RowsToTest.ToString();
            radionNumber.IsChecked = !dataSet.IsPrecentige;
            radionPercentige.IsChecked = dataSet.IsPrecentige;
        }

        public ANNDataSet GetDataSet(bool omitIgnored = false)
        {
            try
            {
                var data1 = new ANNDataSet();
                //
                data1.MetaData = ParseHeader(omitIgnored);

                //data1.TestRows = int.Parse(txtValidationCount.Text);
                data1.RowsToValidation = int.Parse(txtValidationCount.Text);
                data1.RowsToTest = int.Parse(txtTestCount.Text);

                data1.IsPrecentige = radionPercentige.IsChecked.Value;

                var strData = ParseData(data1.MetaData);
                if (strData==null)
                    return null;
                data1.RandomizeData = checkRandomizeDataset.IsChecked.Value;
                //
                data1.Data = strData;
                return data1;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void LoadData()
        {
            try
            {
                ImportData dlg = new ImportData();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (dlg.Data == null)
                        return;
                   // this.Cursor = Cursors.WaitCursor;
                    m_strData = dlg.Data;
                    m_strHeader = dlg.Header;
                    FillDataGrid(dlg.Header, dlg.Data.Where(x=>x!=null).ToArray());
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //this.Cursor = Cursors.Arrow;
            }

        }

        public void CreateNewModel()
        {
            try
            {
                if (m_strData == null || m_strData.Length < 1)
                {
                    MessageBox.Show("Cannot create Model from empty data set.");
                    return;

                }
                if (CreateModel != null)
                {
                    CreateModel(checkRandomizeDataset.IsChecked.Value);
                    checkRandomizeDataset.IsChecked = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ML Preparation Tool");
            }
        }

        #endregion
    }
}
