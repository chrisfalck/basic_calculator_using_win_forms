using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// TODO:
// Deal with errors gracefully
// Check for the wrong number of parenthesis
// Deal with multiple operators in a row, like 5 + 3 - 2 * 4

namespace Calculator
{
    class InfixParser
    {
        private const string operators = "+-/*()^";
        private const string numbers = "123456789.";

        // Call after making changes to make sure no existing code was broken
        private static void TestInfixes()
        {
            const UInt16 insOutsLen = 8;
            string[] inputs =
            {
                "(-5) + 3",
                "(-5 + 3) * -3",
                "-5 + 3 * (-3)",
                "(5 * (4 - 3))",
                "5 + (4) - 3 + 4",
                "-(3+6)",
                "-(3*3)+1",
                "-5*4+5/5"
            };

            string[] expectedOutputs = { "-2", "+6", "-14", "+5", "+10", "-9", "-8", "-19" };

            string[] actualOutputs = new string[insOutsLen];
            for (UInt16 i = 0; i < insOutsLen; ++i)
            {
                actualOutputs[i] = ApplyAllResolves(inputs[i]);
            }

            for (UInt16 i = 0; i < insOutsLen; ++i)
            {
                if (expectedOutputs[i] != actualOutputs[i])
                {
                    throw new Exception($"Test case failed for input {inputs[i]}.\n" +
                                        $"Expected {expectedOutputs[i]} but got {actualOutputs[i]}");
                }
            }
        }

        // Returns ["numberone", "numbertwo", "operand"]
        // Should only be passed an expression with 2 numbers
        private static string[] GetLeftAndRightNumsForOperand(string infix)
        {
            if (InfixContainsOnlyOneNumber(infix)) return null;

            string[] leftRightOp = new string[3] { "", "", "" };

            var tokens = TokenizeInfix(infix);

            UInt16 opIndex = 0;
            if (tokens[0] == "+" || tokens[0] == "-")
            {
                // First num and sign
                leftRightOp[0] = tokens[0] + tokens[1];

                // Operator
                leftRightOp[2] = tokens[2];
                opIndex = 2;
            }
            else
            {
                // First num
                leftRightOp[0] = tokens[0];

                // Operator
                leftRightOp[2] = tokens[1];
                opIndex = 1;
            }

            if (tokens[opIndex + 1] == "+" || tokens[opIndex + 1] == "-")
            {
                // Second num and sign
                leftRightOp[1] = tokens[opIndex + 1] + tokens[opIndex + 2];
            }
            else
            {
                // Second num
                leftRightOp[1] = tokens[opIndex + 1];
            }

            return leftRightOp;
        }

        // True for single numbers with a sign in front such as -12
        private static bool InfixContainsOnlyOneNumber(string infix)
        {
            string[] tokens = TokenizeInfix(infix);
            UInt16 countNumbers = 0;
            foreach (var token in tokens)
            {
                if (decimal.TryParse(token, out decimal unused)) ++countNumbers;
            }
            return countNumbers <= 1;
        }

        // Return a string array representing tokens from the infix where each
        // token is either a single number or operator
        private static string[] TokenizeInfix(string infix)
        {
            ArrayList tokens = new ArrayList();

            var sb = new StringBuilder("");
            foreach (var c in infix)
            {
                if (operators.IndexOf(c) > -1 && sb.ToString() != "")
                {
                    // Add number and operator
                    tokens.Add(sb.ToString());
                    tokens.Add(c);
                    sb.Clear();
                }
                else if (operators.IndexOf(c) > -1 && sb.ToString() == "")
                {
                    // Only add operator
                    tokens.Add(c);
                }
                else
                {
                    // Add nothing, but continue building the current number
                    sb.Append(c);
                }
            }
            if (sb.ToString() != "") tokens.Add(sb.ToString());

            return Array.ConvertAll(tokens.ToArray(), x => x.ToString());
        }

        private static string ParseSubExpression(string[] tokens, UInt16 i)
        {
            var sb = new StringBuilder("");

            // If the first number of the expression begins with a positive 
            // or negative sign, include it
            if (i > 1)
            {
                if (tokens[i - 2] == "+" || tokens[i - 2] == "-")
                {
                    sb.Append(tokens[i - 2]);
                }
            }

            // Add the first number and the operator
            sb.Append(tokens[i - 1]);
            sb.Append(tokens[i]);

            // Add what can be a sign or a number
            sb.Append(tokens[i + 1]);

            // If i + 1 was a positive or negative sign like in -3 * -4, make sure 
            // to include the number too
            if (operators.IndexOf(tokens[i + 1]) > -1)
            {
                sb.Append(tokens[i + 2]);
            }

            return sb.ToString();
        }

        private static string RemoveWhiteSpace(string str)
        {
            var sb = new StringBuilder("");
            foreach (char c in str)
            {
                if (c != ' ')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        // Finds a substring given an infix string with no parentheses and the operators to look for
        // The returned substring will be parsable by the GetLeftAndRightNumsForOperand method
        // Returns the first sub expression it finds according to ops while searching left to right
        private static string FindSubstringForOps(char[] ops, string infix)
        {
            string[] tokens = TokenizeInfix(infix);

            if (tokens.Length <= 2) return infix;

            for (UInt16 i = 0; i < tokens.Length; ++i)
            {
                // Found a token containing an ops value
                if (tokens[i].Contains(ops[0]) || tokens[i].Contains(ops[1]))
                {
                    // i will be the index of a string that is either +, -, *, or /
                    switch (tokens[i])
                    {
                        case "*":
                        case "/":
                            return ParseSubExpression(tokens, i);

                        case "+":
                        case "-":
                            if (i == 0) continue;
                            return ParseSubExpression(tokens, i);
                    }
                }
            }

            return null;
        }

        // From https://stackoverflow.com/questions/8809354/replace-first-occurrence-of-pattern-in-a-string
        private static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        // Resolve a mult/div or plus/minus expression containing no parentheses
        const string SelectMultDiv = "MultDiv";
        const string SelectPlusMinus = "PlusMinus";
        private static string Resolve(string selector, string infix)
        {
            char[] ops = null;
            if (selector.Equals(SelectMultDiv)) ops = new char[] { '*', '/' };
            else if (selector.Equals(SelectPlusMinus)) ops = new char[] { '+', '-' };
            else return null; // Invalid selector

            while (true)
            {
                if (infix.Contains("-+") || infix.Contains("+-"))
                {
                    infix = infix.Replace("+-", "-");
                    infix = infix.Replace("-+", "-");
                }

                // Don't run if the substring has been maximally reduced
                bool nothingToResolve = (infix.IndexOf(ops[0]) == -1 && infix.IndexOf(ops[1]) == -1);
                if (nothingToResolve || InfixContainsOnlyOneNumber(infix))
                {
                    break;
                }

                // The GetLeftAndRightNumsForOperand function must be passed an expression containing
                // only one main operator and two numbers, such as -5 + 3 or 2 * 2
                var subExpression = FindSubstringForOps(ops, infix);
                string[] leftRightOp = GetLeftAndRightNumsForOperand(subExpression);

                // Use decimal, a 128-bit data type, for more precision and a greater range
                decimal firstNum = Convert.ToDecimal(leftRightOp[0]);
                decimal secondNum = Convert.ToDecimal(leftRightOp[1]);
                if (selector.Equals(SelectMultDiv))
                {
                    var reducedExpression = (leftRightOp[2] == "*") ? Convert.ToString(firstNum * secondNum) :
                                                         Convert.ToString(firstNum / secondNum);
                    infix = ReplaceFirst(infix, subExpression, reducedExpression.IndexOf("-") > -1 ? reducedExpression : "+" + reducedExpression);
                }
                if (selector.Equals(SelectPlusMinus))
                {
                    var reducedExpression = (leftRightOp[2] == "+") ? Convert.ToString(firstNum + secondNum) :
                                                         Convert.ToString(firstNum - secondNum);
                    infix = ReplaceFirst(infix, subExpression, reducedExpression.IndexOf("-") > -1 ? reducedExpression : "+" + reducedExpression);
                }
            }

            return infix;
        }

        // Parens cannot be placed next to each other to indicate multiplication
        // For example, we cannot write (5)(4) to get 20, we must write (5)*(4)
        private static string ResolveParens(string infix)
        {
            while (true)
            {
                // Make sure the expression contains parentheses
                if (infix.IndexOf('(').Equals(-1) || infix.IndexOf(')').Equals(-1))
                {
                    return infix;
                }

                // Find the rightmost open paren
                UInt16 idx1 = 0;
                for (UInt16 i = 0; i < infix.Length; ++i)
                {
                    if (infix[i].Equals('(')) idx1 = i;
                }

                // Find the first close paren after the rightmost open paren
                UInt16 idx2 = 0;
                for (UInt16 i = idx1; i < infix.Length; ++i)
                {
                    if (infix[i].Equals(')'))
                    {
                        idx2 = i;
                        break;
                    }
                }

                // Notes for the below logic:
                //  We want to resolve the most nested parenthesis here
                //  Suppose we have an expression like (5 * (5 - 2) + 3)
                //      sub1 will be the string "(5 *"
                //      sub2 will be the string "+ 3)"
                //      (5 - 2) will be resolved to 3, and will replace (5 - 2)
                //  The final string will be (5 * 3 + 3)
                string sub1 = "";
                string sub2 = "";
                if (idx1 != 0)
                {
                    sub1 = infix.Substring(0, idx1);
                }
                if (idx2 != infix.Length - 1)
                {
                    sub2 = infix.Substring(idx2 + 1);
                }

                string mostNestedExpression = infix.Substring(idx1 + 1, idx2 - idx1 - 1);
                string reducedExpression = Resolve(SelectPlusMinus, Resolve(SelectMultDiv, mostNestedExpression));

                infix = sub1 + reducedExpression + sub2;
            }

        }

        private static string ApplyAllResolves(string infix)
        {
            infix = RemoveWhiteSpace(infix);

            // Resolve parens
            infix = ResolveParens(infix);

            // Resolve mult/div 
            infix = Resolve(SelectMultDiv, infix);

            // Resolve plus/minus
            infix = Resolve(SelectPlusMinus, infix);

            return infix;
        }

        public static string ParseInfixString(string infix)
        {
            //TestInfixes(); // Take this out for production code
            try
            {
                return ApplyAllResolves(infix);
            }
            catch (Exception e)
            {
                return "Error, invalid input";
            }
        }
    }
}
