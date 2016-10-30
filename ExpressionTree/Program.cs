using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTree
{


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
        {
            val = newVal;
        }

        public double val;
    }

    public class VarNode : Node
    {

        public VarNode(string newName)
        {
            Name = newName;
        }

        public string Name;
    }

    public class OpNode : Node
    {
        public OpNode (char newOp)
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
            for(int i = 0; i < exp.Length; i++)
            {
                switch (exp[i])
                {
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
                    default:
                        int j = i;
                        for(; j < exp.Length; j++)
                        {
                            if (exp[j] == '*' || exp[j] == '+' || exp[j] == '/' || exp[j] == '-' || exp[j] == '(' || exp[j] == ')')
                                    break;
                        }
                        int chosen = 0;
                        if (Int32.TryParse(exp.Substring(i, (j - i)), out chosen))
                            wood.Push(new ConstNode(chosen));
                        else
                        {
                            wood.Push(new VarNode(exp.Substring(i, (j - i))));
                            SetVar(exp.Substring(i, (j - i)), 0);
                        }
                        i = j - 1;
                        break;
                }
            }
            while (joints.Count > 0)
                LinkBranches();

            // There is only one 'branch' left in wood, but everything is attached to it
            // DA Groot
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

    class test
    {
        int Precedence(char op)
        {
            if (op == '/' || op == '*')
                return 1;
            return 0;
        }

        public void GenRPN(string exp)
        {
            List<string> Postfix = new List<string>();
            Stack<char> Ops = new Stack<char>();
            int j = 0;

            for(int i = 0; i < exp.Length; i++)
            {
                //char cand = exp[i];
                switch (exp[i])
                {
                    case '(':
                        Ops.Push(exp[i]);
                        break;
                    case '+':
                    case '*':
                    case '/':
                    case '-':
                        Console.WriteLine(exp[i]);
                        if (Ops.Count == 0 || Ops.Peek() == '(' || Precedence(Ops.Peek()) < Precedence(exp[i]))
                        {
                            Ops.Push(exp[i]);

                        }
                        else
                        {
                            do
                            {
                                Postfix.Add(Ops.Pop().ToString());
                            } while (Ops.Count > 0 && Ops.Peek() != '(' && Precedence(exp[i]) < Precedence(Ops.Peek()));
                            Ops.Push(exp[i]);
                        }
                        break;
                    case ')':
                        while(Ops.Peek() != '(')
                            Postfix.Add(Ops.Pop().ToString());
                        // Remove the '('
                        Console.WriteLine("Pop: {0}", Ops.Pop());
                        break;
                    default: // on a var or const
                        j = i;
                        for(; j < exp.Length; j++)
                        {
                            if (exp[j] == '*' || exp[j] == '+' || exp[j] == '/' || exp[j] == '-' || exp[j] == '(' || exp[j] == ')')
                                break;
                        }
                        Postfix.Add(exp.Substring(i, (j - i)));
                        i = j - 1;
                        break;
                }
            }

            do
            {
                Postfix.Add(Ops.Pop().ToString());
            } while (Ops.Count > 0);

            for (j = 0; j < Postfix.Count; j++)
                Console.WriteLine("{0}: {1}", j, Postfix[j]);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            ExpTree thing = new ExpTree();
            thing.Compile("22*(92-2)*(3+4)-(A2/3)");
            thing.SetVar("A2", 3);
            Console.WriteLine(thing.Eval());
            Console.ReadLine();
        }
    }
}
