using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace LiCalculator
{
    public static class Helper
    {
        public static long GCD(long a, long b) => b == 0 ? a : GCD(b, a%b);

        public static bool IsSquare(long a) => Abs(a - (long) Sqrt(a)*(long) Sqrt(a)) == 0; 

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
            var s = 1;
            var a = new Stack<IToken>();
            do {
                if (q.Count == 0) throw new UnexpectedTokenException();
                var t = q.Dequeue();
                if (HasBrace(t)) {
                    ++s;
                } else if (IsClosingBrace(t)) {
                    --s;
                }
                if (s > 0) a.Push(t);
            } while (s > 0);
            return a.Reverse().ToArray();
        }

        public static FuncType? IsFunc(IToken i)
        {
            if (i.Type == TokenType.Operator && HasBrace(i) && i.Operator.Length > 1) {
                var n = i.Operator.ToLower().Remove(i.Operator.Length - 1);
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

        public static bool IsMul(IToken i) => i.Operator == "*" || i.Operator == "/";

        public static void Push<T>(this IList<T> list, T item)
        {
            list.Add(item);
        }

        public static T Peek<T>(this IList<T> list, int i = 0) => list[list.Count - 1 - i];

        public static T Pop<T>(this IList<T> list, int i = 0)
        {
            var item = list[list.Count - 1 - i];
            list.RemoveAt(list.Count - 1 - i);
            return item;
        }

        public static IEnumerable<string> Explode(this string str, char del) => str.Split(del).Concat(del.ToString());

        public static IEnumerable<string> Explode(this IEnumerable<string> arr, char del)
            =>
                from i in arr
                from j in i.Explode(del)
                select j;


        public static IEnumerable<T> Concat<T>(this IEnumerable<T> arr, T element)
        {
            var it = arr.GetEnumerator();
            yield return it.Current;
            while (it.MoveNext()) {
                yield return element;
                yield return it.Current;
            }
        }
    }
}