using System.Collections.Generic;
using System.Linq;
using static System.Math;
namespace LiCalculator
{
    public static class Helper
    {
        public static long GCD(long a, long b) => b == 0 ? a : GCD(b, a%b);

        public static IValue Add(IValue LeftOperand, IValue RightOperand)
        {
            if (LeftOperand is Fraction && RightOperand is Fraction) {
                var res = new Fraction {
                    Numerator =
                        ((Fraction) LeftOperand).Numerator
                        *((Fraction) RightOperand).Denominator
                        +
                        ((Fraction) RightOperand).Numerator
                        *((Fraction) LeftOperand).Denominator,
                    Denominator =
                        ((Fraction) RightOperand).Denominator
                        *((Fraction) LeftOperand).Denominator
                };
                var gcd = GCD(Abs(res.Numerator), Abs(res.Denominator));
                res.Numerator /= gcd;
                res.Denominator /= gcd;
                return res;
            }
            if (LeftOperand is FuncVal && RightOperand is FuncVal) {
                if (((FuncVal) LeftOperand).Fun == FuncType.Log &&
                    ((FuncVal) RightOperand).Fun == FuncType.Log) {
                    return new FuncVal {
                        Fun = FuncType.Log,
                        Operand = Mul(((FuncVal) LeftOperand).Operand,
                            ((FuncVal) RightOperand).Operand)
                    };
                }
            }
            return new FloatPoint {
                Value = LeftOperand.Value + RightOperand.Value
            };
        }

        public static IValue Sub(IValue LeftOperand, IValue RightOperand)
        {
            if (LeftOperand is Fraction && RightOperand is Fraction) {
                var res = new Fraction {
                    Numerator =
                        ((Fraction) LeftOperand).Numerator
                        *((Fraction) RightOperand).Denominator
                        -
                        ((Fraction) RightOperand).Numerator
                        *((Fraction) LeftOperand).Denominator,
                    Denominator =
                        ((Fraction) RightOperand).Denominator
                        *((Fraction) LeftOperand).Denominator
                };
                var gcd = GCD(Abs(res.Numerator), Abs(res.Denominator));
                res.Numerator /= gcd;
                res.Denominator /= gcd;
                return res;
            }
            if (LeftOperand is FuncVal && RightOperand is FuncVal) {
                if (((FuncVal) LeftOperand).Fun == FuncType.Log &&
                    ((FuncVal) RightOperand).Fun == FuncType.Log) {
                    return new FuncVal {
                        Fun = FuncType.Log,
                        Operand = Div(((FuncVal) LeftOperand).Operand,
                            ((FuncVal) RightOperand).Operand)
                    };
                }
            }
            return new FloatPoint {
                Value = LeftOperand.Value - RightOperand.Value
            };
        }

        public static IValue Mul(IValue LeftOperand, IValue RightOperand)
        {
            if (LeftOperand is Fraction && RightOperand is Fraction) {
                var FLeftOperand = (Fraction) LeftOperand;
                var FRightOperand = (Fraction) RightOperand;
                var res = new Fraction {
                    Numerator =
                        FLeftOperand.Numerator
                        *FRightOperand.Numerator,
                    Denominator =
                        FLeftOperand.Denominator
                        *FRightOperand.Denominator
                };
                var gcd = GCD(Abs(res.Numerator), Abs(res.Denominator));
                res.Numerator /= gcd;
                res.Denominator /= gcd;
                return res;
            }
            if (LeftOperand is FuncVal && RightOperand is FuncVal) {
                if (((FuncVal) LeftOperand).Fun == FuncType.Sqrt &&
                    ((FuncVal) RightOperand).Fun == FuncType.Sqrt) {
                    return new FuncVal {
                        Fun = FuncType.Sqrt,
                        Operand = Mul(((FuncVal) LeftOperand).Operand,
                            ((FuncVal) RightOperand).Operand)
                    };
                }
                if (((FuncVal) LeftOperand).Fun == FuncType.Exp &&
                    ((FuncVal) RightOperand).Fun == FuncType.Exp) {
                    return new FuncVal {
                        Fun = FuncType.Exp,
                        Operand = Add(((FuncVal) LeftOperand).Operand,
                            ((FuncVal) RightOperand).Operand)
                    };
                }
            }
            return new FloatPoint {
                Value = LeftOperand.Value*RightOperand.Value
            };
        }

        public static IValue Div(IValue LeftOperand, IValue RightOperand)
        {
            if (LeftOperand is Fraction && RightOperand is Fraction) {
                var FLeftOperand = (Fraction) LeftOperand;
                var FRightOperand = (Fraction) RightOperand;
                var res = new Fraction {
                    Numerator =
                        FLeftOperand.Numerator
                        *FRightOperand.Denominator,
                    Denominator =
                        FLeftOperand.Denominator
                        *FRightOperand.Numerator
                };
                var gcd = GCD(Abs(res.Numerator), Abs(res.Denominator));
                res.Numerator /= gcd;
                res.Denominator /= gcd;
                return res;
            }
            if (LeftOperand is FuncVal && RightOperand is FuncVal) {
                if (((FuncVal) LeftOperand).Fun == FuncType.Sqrt &&
                    ((FuncVal) RightOperand).Fun == FuncType.Sqrt) {
                    return new FuncVal {
                        Fun = FuncType.Sqrt,
                        Operand = Div(((FuncVal) LeftOperand).Operand,
                            ((FuncVal) RightOperand).Operand)
                    };
                }
                if (((FuncVal) LeftOperand).Fun == FuncType.Exp &&
                    ((FuncVal) RightOperand).Fun == FuncType.Exp) {
                    return new FuncVal {
                        Fun = FuncType.Exp,
                        Operand = Sub(((FuncVal) LeftOperand).Operand,
                            ((FuncVal) RightOperand).Operand)
                    };
                }
            }
            return new FloatPoint {
                Value = LeftOperand.Value/RightOperand.Value
            };
        }

        public static bool IsVal(IToken i) => i.Type == TokenType.FloatPoint || i.Type == TokenType.Integer;

        public static bool IsOperator(IToken i, string s = null)
        {
            string[] basicOp = {"+", "-", "*", "/"};
            if (i.Type == TokenType.Operator) {
                if (s != null) {
                    return i.Operator == s;
                }
                return basicOp.Contains(i.Operator);
            }
            return false;
        }

        public static bool HasBrace(IToken i) => i.Operator != null && i.Operator.EndsWith("(");

        public static bool IsClosingBrace(IToken i) => i.Operator == ")";

        public static IToken[] ExtractBrace(Queue<IToken> q)
        {
            int s = 1;
            var a = new Stack<IToken>();
            do {
                if(q.Count == 0)throw new UnexpectedTokenException();
                IToken t = q.Dequeue();
                if (HasBrace(t)) {
                    ++s;
                }else if (IsClosingBrace(t)) {
                    --s;
                }
                if(s>0)a.Push(t);
            } while (s>0);
            return a.Reverse().ToArray();
        }

        public static FuncType? IsFunc(IToken i)
        {
            if (i.Type == TokenType.Operator && HasBrace(i) && i.Operator.Length > 1) {
                string n = i.Operator.ToLower().Remove(i.Operator.Length - 1);
                switch (n) {
                    case "sin":
                        return FuncType.Sin;
                    case "cos":
                        return FuncType.Cos;
                    case "tan":
                        return FuncType.Tan;
                    case "asin":
                    case "arcsin":
                        return FuncType.Arcsin;
                    case "acos":
                    case "arccos":
                        return FuncType.Arccos;
                    case "atan":
                    case "arctan":
                        return FuncType.Arctan;
                    case "exp":
                        return FuncType.Exp;
                    case "sqrt":
                        return FuncType.Sqrt;
                    case "log":
                    case "ln":
                        return FuncType.Log;
                    default:
                        throw new FuntionOutOfRangeException();
                }
            }
            return null;
        }
    

        public static void Push<T>(this IList<T> list, T item)
        {
            list.Add(item);
        }

        public static T Peek<T>(this IList<T> list,int i = 0) => list[list.Count - 1 - i];

        public static T Pop<T>(this IList<T> list,int i = 0)
        {
            var item = list[list.Count - 1 - i];
            list.RemoveAt(list.Count - 1 - i);
            return item;
        }


    }
}