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

    public partial class Form1 : Form
    {

        public Form1()
        {
         
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            InitializeComponent();
            dataGridView1.Columns.Clear();
            foreach(char c in alpha)
            {
                dataGridView1.Columns.Add(c.ToString(), c.ToString());
            }
            for(int i = 0; i < 50; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();

            }

            CptS322.Spreadsheet test = new CptS322.Spreadsheet(50, 26);
            dataGridView1.Rows[]
           // test.CellPropertyChanged();
            

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView1.Rows
        }
    }
}
