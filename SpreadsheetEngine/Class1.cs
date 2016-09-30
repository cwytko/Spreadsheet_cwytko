using System;
using System.ComponentModel;
using System.Collections.Generic;



namespace CptS322
{
    public delegate void MyDelegate();

    public abstract class Cell : INotifyPropertyChanged
    {
        // Variables yo
        readonly int _rowIndex;
        readonly int _columnIndex;
        protected string _text;
        protected string _value;

        // https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx
        // An attempted replication of https://msdn.microsoft.com/en-us/library/8627sbea(v=vs.71).aspx
        
        event MyDelegate MyEvent;

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
        public event MyDelegate MyEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        int Row;
        int Col;

        public SpreadsheetCell(int CurRow, int CurCol) : base(CurRow, CurCol)
        {
            Row = CurRow;
            Col = CurCol;
        }

        public void test()
        {
            if (MyEvent != null)
                MyEvent();
        }

        protected void OnPropertyChanged(string desc)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(desc));
            }
        }
    }


    public class Spreadsheet
    {
        // Lots of cells!
        public SpreadsheetCell[,] cell;
        public event PropertyChangedEventHandler PropertyChanged;
        //public delegate void EventHandler(object source, EventArgs e);
        //EventHandler ;

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

        private void f()
        {
            //
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
                    // So close
                    cell[i, j].MyEvent += new MyDelegate(f);
                }
            }

            cell[4, 4].test();
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
