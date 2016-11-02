using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Spreadsheet_cwytko
{
    // I'm going to programmatically create my form

    public partial class Form1 : Form
    {
        //private Button Demo = new Button();
        private BindingSource CellBindingSource = new BindingSource();
        // this is the UI Layer of the Spreadsheet, where the user enters the
        // formulas and can edit them
        private DataGridView CellDataGridView = new DataGridView();
        // this is the Data Layer of the Spreadsheet, where the evaluations are
        // stored
        CptS322.Spreadsheet test = new CptS322.Spreadsheet(26, 50);

        public Form1()
        {
            
            CellDataGridView.Dock = DockStyle.Fill;
            this.Controls.Add(CellDataGridView);
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            InitializeComponent();
            CellDataGridView.Columns.Clear();

            foreach (char c in alpha)
            {
                CellDataGridView.Columns.Add(c.ToString(), c.ToString());
            }

            CellBindingSource.DataSource = test;
            // http://stackoverflow.com/questions/29633018/show-2d-array-in-datagridview
            int col = 0;
            for (int r = 0; r < 50; r++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.CellDataGridView);

                for (col = 0; col < 26; col++)
                {

                    //CellDataGridView[col, r].DataGridView.CellContentClick += DataGridView_CellContentClick;
                    CellDataGridView[col, r].DataGridView.CellEndEdit += DataGridView_CellEndEdit;
                    CellDataGridView[col, r].DataGridView.CellBeginEdit += DataGridView_CellBeginEdit;
                    CellDataGridView[col, r].Value = ((CptS322.Spreadsheet)CellBindingSource.DataSource).cell[col, r];
                    // THIS part took for freaking ever yo, I'm a potato
                    ((CptS322.Spreadsheet)CellBindingSource.DataSource).cell[col, r].PropertyChanged += new PropertyChangedEventHandler(Form1_PropertyChanged);

                }

                this.CellDataGridView.Rows.Add(row);
                CellDataGridView.Rows[r].HeaderCell.Value = (r + 1).ToString();
            }

            InitializeCellDataGridView();
        }

        // If there was previous text within this sender's text present it so 
        // the user on CellContent Click...
        private void DataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            string msg = String.Format("Editing Cell at ({0}, {1}) {2}",
                e.ColumnIndex, e.RowIndex, test.cell[e.ColumnIndex, e.RowIndex].ReturnText());
            this.Text = msg;
        }

        private void Form1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int r = (sender as CptS322.SpreadsheetCell).ColumnIndex;
            int c = (sender as CptS322.SpreadsheetCell).RowIndex;
            CellDataGridView[c, r].Value = (sender as CptS322.SpreadsheetCell).ReturnText();
        }

        private void InitializeCellDataGridView()
        {
            CellDataGridView.Dock = DockStyle.Fill;
            CellDataGridView.AllowUserToAddRows = false;
            CellDataGridView.AllowUserToDeleteRows = false;
        }

        private void DataGridView_CellEndEdit(object sender,
            DataGridViewCellEventArgs e)
        {
            if(CellDataGridView[e.ColumnIndex, e.RowIndex].Value != null)
                test.cell[e.ColumnIndex, e.RowIndex].SetText(CellDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString());
            string msg = String.Format("Finished Editing Cell at ({0}, {1} {2}) {3}",
                e.ColumnIndex, e.RowIndex, CellDataGridView[e.ColumnIndex, e.RowIndex].Value, test.cell[e.ColumnIndex, e.RowIndex].ReturnText());
            this.Text = msg;
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            
        }

        private void Demo_Click(object sender, EventArgs e)
        {
            // This doesnt seem to be writing to the data layer, only the ui
            test.testPropChanged();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
