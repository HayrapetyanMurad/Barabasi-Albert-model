using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace bianconi_barabasi_model
{
    class DistributedRandom
    {
        class SimpleFunction
        {
            public SimpleFunction(string infix_function)
            {
                postfix_list = new List<string>();
                valid_expr = init(infix_function);
            }

            public bool is_valid()
            {
                return valid_expr;
            }
            public double get_value(double x)
            {
                double value = 0;

                Debug.Assert(valid_expr, "invalid function");
                bool result = compute_postfix_expression(x, out value);
                Debug.Assert(result, "invalid function");

                return value;
            }

            private List<string> postfix_list;
            private bool valid_expr;

            private static int Prec(string c)
            {
                switch (c)
                {
                    case "+":
                    case "-":
                        return 1;

                    case "*":
                    case "/":
                        return 2;

                    case "^":
                        return 3;

                    case "sqrt":
                    case "sin":
                    case "cos":
                    case "tan":
                    case "ln":
                        return 4;

                }
                return -1;
            }

            private bool init(string infix)
            {
                Stack<string> stack = new Stack<string>();

                int lenght = infix.Length;
                string token;

                for (int i = 0; i < lenght; i++)
                {
                    token = "";
                    char c = infix[i];
                    if (char.IsDigit(c))
                    {
                        while (i < lenght && char.IsDigit(infix[i]))
                        {
                            token += infix[i];
                            i++;
                        }
                        i--;
                        postfix_list.Add(token);
                    }
                    else if (c == 'x')
                    {
                        postfix_list.Add("x");
                    }
                    else if (c == '(')
                    {
                        stack.Push("(");
                    }
                    else if (c == ')')
                    {
                        while (stack.Count > 0 && stack.Peek() != "(")
                        {
                            postfix_list.Add(stack.Pop());
                        }

                        if (stack.Count > 0 && stack.Peek() != "(")
                        {
                            return false;
                        }
                        else
                        {
                            stack.Pop();
                        }
                    }
                    else
                    {
                        if (c == '+' || c == '-' || c == '*' || c == '/' || c == '^')
                        {
                            token += c;
                        }
                        else if (i + 4 <= lenght && infix.Substring(i, 4) == "sqrt")
                        {
                            token = "sqrt";
                            i += 3;
                        }
                        else if (i + 3 <= lenght && infix.Substring(i, 3) == "sin")
                        {
                            token = "sin";
                            i += 2;
                        }
                        else if (i + 3 <= lenght && infix.Substring(i, 3) == "cos")
                        {
                            token = "cos";
                            i += 2;
                        }
                        else if (i + 3 <= lenght && infix.Substring(i, 3) == "tan")
                        {
                            token = "tan";
                            i += 2;
                        }
                        else if (i + 2 <= lenght && infix.Substring(i, 2) == "ln")
                        {
                            token = "ln";
                            i++;
                        }
                        else
                        {
                            return false;
                        }

                        while (stack.Count > 0 && Prec(token) <= Prec(stack.Peek()))
                        {
                            postfix_list.Add(stack.Pop());
                        }

                        stack.Push(token);

                    }
                }

                while (stack.Count > 0)
                {
                    postfix_list.Add(stack.Pop());
                }

                return true;
            }

            private bool compute_postfix_expression(double x, out double result)
            {
                result = 0;
                Stack<double> stack = new Stack<double>();
                string token;

                for (int i = 0; i < postfix_list.Count; i++)
                {
                    token = postfix_list[i];
                    if (char.IsDigit(token[0]))
                    {
                        double number;
                        bool res = Double.TryParse(token.Replace('.', ','), out number);
                        if (res == false)
                        {
                            return false;
                        }
                        stack.Push(number);
                    }
                    else if (token == "x")
                    {
                        stack.Push(x);
                    }
                    else if (token == "+")
                    {
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double second = stack.Peek();
                        stack.Pop();

                        double first;
                        if (stack.Count == 0)
                        {
                            first = 0;
                        }
                        else
                        {
                            first = stack.Peek();
                            stack.Pop();
                        }

                        stack.Push(first + second);
                    }
                    else if (token == "-")
                    {
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double second = stack.Peek();
                        stack.Pop();

                        double first;
                        if (stack.Count == 0)
                        {
                            first = 0;
                        }
                        else
                        {
                            first = stack.Peek();
                            stack.Pop();
                        }

                        stack.Push(first - second);
                    }
                    else if (token == "*")
                    {
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double second = stack.Peek();
                        stack.Pop();

                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double first = stack.Peek();
                        stack.Pop();

                        stack.Push(first * second);
                    }
                    else if (token == "/")
                    {
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double second = stack.Peek();
                        stack.Pop();

                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double first = stack.Peek();
                        stack.Pop();

                        stack.Push(first / second);
                    }
                    else if (token == "^")
                    {
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double second = stack.Peek();
                        stack.Pop();

                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double first = stack.Peek();
                        stack.Pop();

                        stack.Push(Math.Pow(first, second));
                    }
                    else if (token == "sqrt")
                    {
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double first = stack.Peek();
                        stack.Pop();

                        stack.Push(Math.Sqrt(first));
                    }
                    else if (token == "sin")
                    {
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double first = stack.Peek();
                        stack.Pop();

                        stack.Push(Math.Sin(first));
                    }
                    else if (token == "cos")
                    {
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double first = stack.Peek();
                        stack.Pop();

                        stack.Push(Math.Cos(first));
                    }
                    else if (token == "tan")
                    {
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double first = stack.Peek();
                        stack.Pop();

                        stack.Push(Math.Tan(first));
                    }
                    else if (token == "ln")
                    {
                        if (stack.Count == 0)
                        {
                            return false;
                        }
                        double first = stack.Peek();
                        stack.Pop();

                        stack.Push(Math.Log(first));
                    }
                }

                if (stack.Count != 1)
                {
                    return false;
                }

                result = stack.Peek();
                return true;
            }

        }

        public DistributedRandom(string function_str)
        {
            function_ = new SimpleFunction(function_str);
            random_ = new Random();
        }
        public double get_probability()
        {
            return random_.NextDouble();
        }
        public double get_distributed_probability()
        {
            double probability = random_.NextDouble();

            double result = 0.5;
            double delta = 0.5;
            double eps = 0.00001;
            double steps = 1000;

            double norm = numeric_integral(0, 1, eps, 1000);

            while (true)
            {
                delta /= 2;
                double integral_result = numeric_integral(0, result, eps, 1000);
                integral_result /= norm;
                if (Math.Abs(integral_result - probability) < eps || steps == 0)
                {
                    return result;
                }
                if (integral_result > probability)
                {
                    result -= delta;
                }
                else
                {
                    result += delta;
                }

                steps--;
            }

        }

        private double numeric_integral(double a, double b, double eps, int maxSteps)
        {
            double h = 0.5 * (b - a);
            double s = function_.get_value(a) + function_.get_value(b) + 2 * function_.get_value(a + h);
            double intF = s * h * 0.5;
            double intFprev = 0;
            double t = a;
            int i, j;
            int n = 1;
            for (i = 1; i <= maxSteps; i++)
            {
                n += n;
                t = a + 0.5 * h;
                intFprev = intF;
                for (j = 1; j <= n; j++)
                {
                    s += 2 * function_.get_value(t);
                    t += h;
                }
                h *= 0.5;
                intF = s * h * 0.5;
                if (Math.Abs(intF - intFprev) <= eps)
                    return intF;
            }
            return intF;
        }

        private SimpleFunction function_;
        private Random random_;

    }
}
