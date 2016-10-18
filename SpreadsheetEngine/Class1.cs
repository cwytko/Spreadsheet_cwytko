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
                else
                {
                    _text = value;
                    _value = value;
                }
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

        //public void SetText(string n, ...)
        //{
        //    Text = n;
        //}

        public string ReturnValue()
        {
            return Value;
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

            if ((sender as SpreadsheetCell).ReturnText().StartsWith("="))
            {
                // Set the text of a value of another box
                cell[(sender as SpreadsheetCell).RowIndex, (sender as SpreadsheetCell).ColumnIndex].SetText(cell[int.Parse((sender as SpreadsheetCell).ReturnText().Substring(2)), Convert.ToInt32((sender as SpreadsheetCell).ReturnText()[1]) - 65].ReturnText());
               // cell[int.Parse((sender as SpreadsheetCell).ReturnText().Substring(2)), Convert.ToInt32((sender as SpreadsheetCell).ReturnText()[1]) - 65].SetText((sender as SpreadsheetCell).ReturnText());
            }
        }

        public void testPropChanged()
        {
            Random col = new Random(), row = new Random();
            for (int i = 0; i < 50; i++)
            {
                cell[(row.Next() % 50), (col.Next() % 26)].SetText("Pickle");
            }
            for (int i = 0; i < 50; i++)
            {
                string thing = String.Format("This is cell B{0}", i);
               string thing2 = String.Format("=B{0}", i);
                cell[i, 1].SetText(thing);
                cell[i, 0].SetText(thing2);
            }                        
            
        }
    }

    class Node
    {
        double Eval() { return 0; }
    }
    class constNode : Node
    {
        public double value;
        public constNode(double assign)
        {
            value = assign;
        }
        double Eval() { return value; }
    }

    // Will this rely on the existence on an ExpTree object in order to see
    // variables and their associated value?
    class VarNode : Node
    {
        public string Name;
        public VarNode(string expression)
        {
            Name = expression;
        }
        // reference the variable from the ExpTree class
        //private Dictionary<string, double> m_vars;
        //public double Eval()
        //{
        //    return ExpTree.GetVarValue(Name);
        //}
    }
    class OpNode : Node
    {
        public OpNode(char NewOp)
        {
            Op = NewOp;
        }

        public char Op;
        public Node Left, Right;
    }

    public class ExpTree
    {

        // force VarNodes to grab the double value from this dictionary
        private Dictionary<string, double> m_vars = new Dictionary<string, double>();

        public double GetVarValue(string name)
        {
            return m_vars[name];
        }

        void SetVar(string varName, double varValue)
        {
            m_vars[varName] = varValue;
        }

        // this will hold either the 'last to be evaluated' operator of the 
        // expression or a single variable OR single value
        private Node m_root;
       

        ExpTree(string expression_yo)
        {
            m_root = Compile(expression_yo);
        }

        // This function builds the node tree
        Node Compile(string exp)
        {
            int index = GetOpIndex(exp);
            if (-1 == index)
                return BuildSimple(exp);
            OpNode root = new OpNode(exp[index]);
            root.Left = Compile(exp.Substring(0, index));
            root.Right = Compile(exp.Substring(index + 1));
            return root;
        }

        // We reach this function when we have encountered no more operators
        // There is only a variable or constant left
        // This will determine if the leaf is a constant or a var
        private Node BuildSimple(string exp)
        {
            double num = 0;
            if (double.TryParse(exp, out num))
                return new constNode(num);
            // During the construction of this VarNode I need to populate the 
            // dictionary with the exp as the Name, how do I know if the Name
            // has a value association?
            return new VarNode(exp);
        }

        // This will return an operator's index location within a given string
        private int GetOpIndex(string exp)
        {
            // Parenthesis count (will determine precedence of execution
            int pCount = 0;

            for(int i = exp.Length - 1; i > 0; i--)
            {
                // By placing the '+' and the '-' first we take care of 
                // precedence issues on solving multiplication first
                if ((exp[i] == '+' || exp[i] == '-') && pCount == 0)
                    return i;
                else if (exp[i] == ')')
                    pCount++;
                else if (exp[i] == '(')
                    pCount--;
                else if ((exp[i] == '*' || exp[i] == '/') && pCount == 0)
                    return i;
            }
            return -1;
        }

        // This will render the tree
        // Here's an example on how the tree can be read 
        // http://cs.nyu.edu/courses/fall11/CSCI-GA.1133-001/rct257_files/Expression_Trees.pdf
        double resolve(Node traverse)
        {
            double evaluation = 0;
            if (traverse is constNode)
                return (traverse as constNode).value;
            else if (traverse is VarNode)
                return m_vars[(traverse as VarNode).Name];
            // The only other thing traverse could be would be an OpNode
            else
            {
                OpNode on = (traverse as OpNode);
                if (on.Op == '+')
                    evaluation += resolve(on.Left) + resolve(on.Right);
                else if (on.Op == '-')
                    evaluation += resolve(on.Left) - resolve(on.Right);
                else if (on.Op == '*')
                    evaluation += resolve(on.Left) * resolve(on.Right);
                else
                    evaluation += resolve(on.Left) / resolve(on.Right);
            }
            return evaluation;
        }

        double Eval()
        {
            double evaluation = 0;

            evaluation = resolve(m_root);

            return evaluation;
        }
    }
}
