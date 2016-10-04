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
        private BindingSource CellBindingSource = new BindingSource();
        private DataGridView CellDataGridView = new DataGridView();
        CptS322.Spreadsheet test = new CptS322.Spreadsheet(50, 26);

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
                    
                    CellDataGridView[col, r].Value = ((CptS322.Spreadsheet)CellBindingSource.DataSource).cell[r, col];
                    // THIS part took for freaking ever yo, I'm a potato
                    ((CptS322.Spreadsheet)CellBindingSource.DataSource).cell[r, col].PropertyChanged += new PropertyChangedEventHandler(Form1_PropertyChanged);
                }

                this.CellDataGridView.Rows.Add(row);
                CellDataGridView.Rows[r].HeaderCell.Value = (r + 1).ToString();
            }

            InitializeCellDataGridView();
        }

        private void Form1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CellDataGridView[(sender as CptS322.SpreadsheetCell).ColumnIndex, (sender as CptS322.SpreadsheetCell).RowIndex].Value = (sender as CptS322.SpreadsheetCell).ReturnText();
        }

        private void InitializeCellDataGridView()
        {
            CellDataGridView.Dock = DockStyle.Fill;
            CellDataGridView.AllowUserToAddRows = false;
            CellDataGridView.AllowUserToDeleteRows = false;
            
            test.testPropChanged();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
 
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView1.Rows
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
