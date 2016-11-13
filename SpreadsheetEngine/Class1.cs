using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        protected void SetValue(string evaluation)
        {
            _value = string.Copy(evaluation);
        }

        protected string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value.Equals(_text))
                    return;
                else
                {
                    _text = (value);
                    OnPropertyChanged("Text");
                }
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

        public string ReturnValue()
        {
            return Value;
        }

        internal void NewValue(string eval)
        {
            SetValue(eval);
        }
    }

    public class Spreadsheet : SpreadsheetCell
    {

        // Lots of cells!
        public SpreadsheetCell[,] cell;
        int CapacityRows, CapacityCols;

        public int ColumnCount() { return CapacityCols; }

        public int RowCount() { return CapacityRows; }

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

        // this is for the dependency tree and reevaluation of dependencies
        public void reEval(int col, int row)
        {
            //(sender as SpreadsheetCell).NewValue(new ExpTree((sender as SpreadsheetCell).ReturnText().Substring(1)).Eval().ToString());
            cell[col, row].NewValue(new ExpTree(cell[col, row].ReturnText().Substring(1)).Eval().ToString());
        }

        private void Spreadsheet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((sender as SpreadsheetCell).ReturnText().StartsWith("="))
            {
                (sender as SpreadsheetCell).NewValue(new ExpTree((sender as SpreadsheetCell).ReturnText().Substring(1)).Eval().ToString());
            }

            else
            {
                (sender as SpreadsheetCell).NewValue((sender as SpreadsheetCell).ReturnText());
            }
        }

        public void testPropChanged()
        {

            string thing = string.Empty;
            string thing2 = (string.Empty);

            Random col = new Random(), row = new Random();
            for (int i = 0; i < 50; i++)
            {
                cell[(col.Next() % 26), (row.Next() % 50)].SetText("Pickle");
            }
            for (int i = 0; i < 50; i++)
            {
                thing = string.Copy(string.Format("This is cell B{0}", i));

                cell[1, i].SetText(thing);
            }

            for (int i = 0; i < 50; i++)
            {
                thing2 = string.Copy(string.Format("=B{0}", i));
                cell[0, i].SetText(thing2);
            }

        }
    }

    // I found it Logan 
    // THE KRABBY PATTY FORMULA
    // This clarified so much for building the 'false' bottom
    // stack for operator precedence
    // https://www.seas.gwu.edu/~csci131/fall96/exp_to_tree.html
    //
    public class Node { }

    public class ConstNode : Node
    {
        public ConstNode(double newVal)
        { val = newVal; }

        public double val;
    }

    public class VarNode : Node
    {

        public VarNode(string newName) { Name = newName; }

        public string Name;
    }

    public class OpNode : Node
    {
        public OpNode(char newOp)
        {
            Op = newOp;
            Left = null; Right = null;
        }

        private char Op;

        public char OP
        {
            get
            { return Op; }
            set { }
        }
        public Node Left, Right;
    }

    public class ExpTree
    {
        public Node root = new Node();
        Stack<Node> wood = new Stack<Node>();
        Stack<OpNode> joints = new Stack<OpNode>();
        Dictionary<string, double> m_vars = new Dictionary<string, double>();


        public ExpTree(string exp)
        {
            Compile(exp);
        }

        public double GetVarValue(string name)
        {
            return m_vars[name];
        }

        public void SetVar(string varName, double varValue)
        {
            m_vars[varName] = varValue;
        }

        int Precedence(char op)
        {
            if (op == '/' || op == '*')
                return 1;
            return 0;
        }

        // This helper function is so fucking cool
        public void LinkBranches()
        {
            OpNode branch = new OpNode(joints.Pop().OP);
            branch.Right = wood.Pop();
            branch.Left = wood.Pop();
            wood.Push(branch);
        }

        public void Compile(string exp)
        {
            for (int i = 0; i < exp.Length; i++)
            {
                switch (exp[i])
                {
                    case ' ':
                        // Ignore whitespace
                        break;
                    case '(':
                        joints.Push(new OpNode(exp[i]));
                        break;
                    case ')':
                        while (joints.Peek().OP != '(')
                            LinkBranches();
                        // Get rid of '('
                        joints.Pop();
                        break;
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                        if (joints.Count == 0)
                            joints.Push(new OpNode(exp[i]));
                        else if (joints.Peek().OP == '(')
                            joints.Push(new OpNode(exp[i]));
                        else if (Precedence(joints.Peek().OP) < Precedence(exp[i]))
                            joints.Push(new OpNode(exp[i]));
                        else
                        {
                            do
                            {
                                LinkBranches();
                            } while (joints.Count > 0 && joints.Peek().OP == '(' &&
                            Precedence(joints.Peek().OP) < Precedence(exp[i]));
                            joints.Push(new OpNode(exp[i]));
                        }
                        break;
                    // Found a const or a var so parse it, check here if one
                    // of these is referring to a cell, if so then we need to 
                    // add this cell 'ref' to the List of m_vars
                    default:
                        int j = i;
                        for (; j < exp.Length; j++)
                        {
                            if (exp[j] == '*' || exp[j] == '+' || exp[j] == '/' || exp[j] == '-' || exp[j] == '(' || exp[j] == ')' || exp[j] == ' ')
                                break;
                        }
                        int chosen = 0;
                        if (Int32.TryParse(exp.Substring(i, (j - i)), out chosen))
                            wood.Push(new ConstNode(chosen));
                        // Here is when we know we have either a cell ref or 
                        // some gibberish
                        // If it's a cell ref grab the value, if the value is
                        // null then we SetVar to 0, but we also need to make 
                        // sure if there's a change we need to know, so we
                        // insert what was referenced into a hashset, at any
                        // point when that cell is edited again we need to 
                        // run the evaluate again on the cells that rely on the
                        // cell that was edited
                        // First use the SetVar function to set the new value 
                        // of the cell and then recreate the exp tree for the 
                        // 
                        else
                        {
                            wood.Push(new VarNode(exp.Substring(i, (j - i))));
                            // Here check the Substring if it's a cell ref
                            // If so use SetVar and set it to the cell's val
                            // else set the var to 0
                            Regex isAlpha = new Regex(@"^[A-Z]{1}(?([0-4]?[0-9]{1}$)|50$)");
                            if(isAlpha.IsMatch(exp.Substring(i, (j - i))))
                            {
                                // If we're in here this means are trying to
                                // reference a real cell in the spreadsheet so
                                // get its value, if there is no value, then 
                                // we make the val 0
                                bool here = true;
                            }
                            SetVar(exp.Substring(i, (j - i)), 0);
                        }
                        i = j - 1;
                        break;
                }
            }
            while (joints.Count > 0)
                LinkBranches();

            // There is only one 'branch' left in wood, but everything is 
            //attached to it :: DA Groot
            root = wood.Pop();
        }

        double resolve(Node traverse)
        {
            double evaluation = 0;

            if (traverse is ConstNode)
                return (traverse as ConstNode).val;
            else if (traverse is VarNode)
                return m_vars[(traverse as VarNode).Name];
            else
            {
                OpNode on = traverse as OpNode;
                if (on.OP == '+')
                    evaluation += resolve(on.Left) + resolve(on.Right);
                else if (on.OP == '-')
                    evaluation += resolve(on.Left) - resolve(on.Right);
                else if (on.OP == '*')
                    evaluation += resolve(on.Left) * resolve(on.Right);
                else
                    evaluation += resolve(on.Left) / resolve(on.Right);
            }
            return evaluation;
        }

        public double Eval()
        {
            double evaluation = resolve(root);
            return evaluation;
        }
    }
}
