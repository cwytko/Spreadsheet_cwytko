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
        protected string _text;
        protected string _value;

        // https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx
        public event PropertyChangedEventHandler PropertyChanged;
        public EventArgs e = null;
        //public delegate void PropertyChangedEventHandler(Cell c, EventArgs e);

        // I used the link below, copied/pasted the MSDN's version of
        // OnPropertyChanged() and edited to the specs of this assignment
        // https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx

        //public delegate void PropertyHandler(Cell c, EventArgs e);

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

        public string Value
        {
            get
            {
                return _value;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            /*
             * The big hint for this: It’s a protected property which means
             * inheriting classes can see it. Inheriting classes should NOT
             * be publically exposed to code outside the class library. 
             * 
             * I'm not sure if I'm doing the protection correct in this
             * sense.
             * 
             * Clarification for myself
             * https://msdn.microsoft.com/en-us/library/a1khb4f8.aspx
             */
            set
            {
                if (value == _text)
                    return;

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
}


    public class Spreadsheet
    {
        // Lots of cells!
        public SpreadsheetCell[,] cell;
        public event PropertyChangedEventHandler PropertyChanged;
        public delegate EventHandler(object source, EventArgs e);

        int CapacityRows, CapacityCols;

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

        public Spreadsheet(int NumRows, int NumCols)
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
                   
                }
            }
        }

        void CellPropertyChanged(string NText)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(NText));
            }
        }

        private void Acknowledged(object c, EventArgs e)
        {

        }

        public void testPropChanged()
        {
            this.cell[5, 5].Text = 33.ToString();
            //[5, 5].Text = 33.ToString();
        }
    }

}
