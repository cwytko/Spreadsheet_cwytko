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
        // this is the UI Layer of the Spreadsheet, where the user enters the
        // formulas and can edit them
        private DataGridView CellDataGridView = new DataGridView();
        // this is the Data Layer of the Spreadsheet, where the evaluations are
        // stored
        CptS322.Spreadsheet test = new CptS322.Spreadsheet(26, 50);

        // I need to have the variable that will store the variables' values
        // out here, the current setup each cell has its own m_vars which is 
        // not right
        Dictionary<string, double> cell_vals = new Dictionary<string, double>();
        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        // The hashset stores the dependent coordinates of a given 'key' cell
        // the 'values' would be the coordinates of othercells that rely on the 
        // contents of the 'key' cell
        Dictionary<Tuple<int, int>, List<Tuple<int, int>>> Dependencies = new Dictionary<Tuple<int, int>, List<Tuple<int, int>>>();

        public Form1()
        {
            CellDataGridView.Dock = DockStyle.Fill;
            this.Controls.Add(CellDataGridView);
            InitializeComponent();
            CellDataGridView.Columns.Clear();

            foreach (char c in alpha)
            {
                CellDataGridView.Columns.Add(c.ToString(), c.ToString());
            }

            int col = 0;
            for (int r = 0; r < 50; r++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.CellDataGridView);

                for (col = 0; col < 26; col++)
                {
                    CellDataGridView[col, r].DataGridView.CellEndEdit += DataGridView_CellEndEdit;
                    CellDataGridView[col, r].DataGridView.CellBeginEdit += DataGridView_CellBeginEdit;
                    CellDataGridView[col, r].DataGridView.CellLeave += DataGridView_CellLeave;
                    // THIS part took for freaking ever yo, I'm a potato
                    // It seems awfully sloppy but it will be pasta for now
                    test.cell[col,r].PropertyChanged += new PropertyChangedEventHandler(Form1_PropertyChanged);
                }

                this.CellDataGridView.Rows.Add(row);
                CellDataGridView.Rows[r].HeaderCell.Value = (r + 1).ToString();
            }

            InitializeCellDataGridView();
        }


        // This is for if the text entered in the cell hasn't changed once
        // focus is left
        private void DataGridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            CellDataGridView[e.ColumnIndex, e.RowIndex].Value = test.cell[e.ColumnIndex, e.RowIndex].ReturnValue();
        }

        private void DataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            string msg = String.Format("Editing Cell at ({0}, {1}): {2}",
                e.ColumnIndex, e.RowIndex, test.cell[e.ColumnIndex, e.RowIndex].ReturnText());
            this.Text = msg;

            if(test.cell[e.ColumnIndex, e.RowIndex].ReturnText() != null)
                CellDataGridView[e.ColumnIndex, e.RowIndex].Value = test.cell[e.ColumnIndex, e.RowIndex].ReturnText();
        }

        // this is should handle all displaying event of the data through the
        // ui
        private void Form1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int c = (sender as CptS322.SpreadsheetCell).ColumnIndex;
            int r = (sender as CptS322.SpreadsheetCell).RowIndex;

                if ((sender as CptS322.SpreadsheetCell).ReturnText().StartsWith("="))
                    CellDataGridView[r, c].Value = (sender as CptS322.SpreadsheetCell).ReturnValue();

                else { CellDataGridView[r, c].Value = (sender as CptS322.SpreadsheetCell).ReturnText(); }
        }

        private void InitializeCellDataGridView()
        {
            CellDataGridView.Dock = DockStyle.Fill;
            CellDataGridView.AllowUserToAddRows = false;
            CellDataGridView.AllowUserToDeleteRows = false;
        }

        private void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            if (CellDataGridView[e.ColumnIndex, e.RowIndex].Value != null && !(CellDataGridView[e.ColumnIndex, e.RowIndex].Value.Equals(test.cell[e.ColumnIndex, e.RowIndex].ReturnValue())))
                test.cell[e.ColumnIndex, e.RowIndex].SetText(CellDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString());

            //if(test.cell[e.ColumnIndex, e.RowIndex].Deps.Count > 0)
            //{
            foreach (Tuple<int, int> dp in test.cell[e.ColumnIndex, e.RowIndex].Deps)
            {
               CellDataGridView[dp.Item2, dp.Item1].Value = test.cell[dp.Item2, dp.Item1].ReturnValue();
            }
            //}

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Demo_Click(object sender, EventArgs e)
        {
            test.testPropChanged();
        }

    }
}
