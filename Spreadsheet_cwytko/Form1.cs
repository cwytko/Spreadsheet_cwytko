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


            // http://stackoverflow.com/questions/29633018/show-2d-array-in-datagridview
            for (int r = 0; r < 50; r++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.CellDataGridView);

                for (int c = 0; c < 26; c++)
                {
                    row.Cells[c].Value = test.cell[r, c].Text;
                }

                this.CellDataGridView.Rows.Add(row);
                CellDataGridView.Rows[r].HeaderCell.Value = (r + 1).ToString();
            }

            //for (int i = 0; i < 50; i++)
            //{
            //    CellDataGridView.Rows.Add();
            //    CellDataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();

            //}


            InitializeCellDataGridView();
        }

        private void InitializeCellDataGridView()
        {
            CellDataGridView.Dock = DockStyle.Fill;
            CellDataGridView.AllowUserToAddRows = false;
            CellDataGridView.AllowUserToDeleteRows = false;


            // https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.datasource%28v=vs.110%29.aspx
            // BIND the Data layer to the DATA SOURCE
            //CellBindingSource.DataSource = ;

            // https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.datasource%28v=vs.110%29.aspx
            // BIND THE DATA SOURCE
            //CellDataGridView.DataSource = CellBindingSource;

            //CellDataGridView.Rows[5].Cells[5].Value = 10;
            test.testPropChanged();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
 
        }

        private void Test_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
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
