using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace CptS322
{
    public abstract class Cell : INotifyPropertyChanged
    {
        // Variables yo
        readonly int _rowIndex;
        readonly int _columnIndex;
        private string _text;
        private string _value;

        // https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx
        public event PropertyChangedEventHandler PropertyChanged;
        public EventArgs e = null;

        // I used the link below, copied/pasted the MSDN's version of
        // OnPropertyChanged() and edited to the specs of this assignment
        // https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx

        protected void OnPropertyChanged(string desc)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(desc));
            }
        }

        // https://msdn.microsoft.com/en-us/library/acdd6hb7.aspx
        protected Cell(int RowIndex, int ColumnIndex)
        {
            _rowIndex = RowIndex;
            _columnIndex = ColumnIndex;
        }

        protected string Value
        {
            get
            {
                return _value;
            }
            
        }

        protected string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value == _text)
                    return;
                else if ()

                _text = value;
                // Fire the event of the edit
                OnPropertyChanged("Text");
            }

        }

        public int RowIndex
        {
            get
            {
                return _rowIndex;
            }
        } 

        public int ColumnIndex
        {
            get
            {
                return _columnIndex;
            }
        }
    }

    // Edification purposes
    // Intermediary step
    public class SpreadsheetCell : Cell
    {
        int Row;
        int Col;

        public SpreadsheetCell(int CurRow, int CurCol) : base(CurRow, CurCol)
        {
            Row = CurRow;
            Col = CurCol;
        }

        // Hopefully this isn't viewed as circumventing the protections
        public string ReturnText()
        {
            return Text;
        }

        public void SetText(string n)
        {
            Text = n;
        }

    }


    public class Spreadsheet : SpreadsheetCell
    {

        // Lots of cells!
        public SpreadsheetCell[,] cell;
        int CapacityRows, CapacityCols;
        /*********************************************************************/

        public int ColumnCount()
        {
            return CapacityCols;
        }

        public int RowCount()
        {
            return CapacityRows;
        }

        public Cell GetCell(int r, int c)
        {
            if (r <= CapacityRows && c <= CapacityCols)
                return cell[r, c];
            return null;
        }

        public Spreadsheet(int NumRows, int NumCols) : base(NumRows, NumCols)
        {
            // We need to have Cells made new each time
            cell = new SpreadsheetCell[NumRows, NumCols];
            CapacityRows = NumRows;
            CapacityCols = NumCols;

            for (int i = 0; i < NumRows; i++)
            {
                for (int j = 0; j < NumCols; j++)
                {
                    cell[i, j] = new SpreadsheetCell(i, j);
                    cell[i, j].PropertyChanged += new PropertyChangedEventHandler(Spreadsheet_PropertyChanged);
                }
            }
        }

        private void Spreadsheet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        public void testPropChanged()
        {
            Random col = new Random(), row = new Random();
            for (int i = 0; i < 50; i++)
            {
                cell[(row.Next() % 50), (col.Next() % 26)].SetText("Pickle");
            }
        }
    }

}
