using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Char;
namespace LiCalculator
{
    public enum TokenType
    {
        FloatPoint,
        Integer,
        Operator
    }
    public class Tokenizer
    {

        public static IToken[] ToTokens(string str)
            => (from token in NextToken(str)
                   select token
                ).ToArray();


        public static IEnumerable<IToken> NextToken(string str)
        {
            var st = str.Split(' ').Aggregate(string.Concat);
            var arr = st.ToCharArray();
            int start =0, cur =0;
            TokenType ty = TokenType.Integer;
            char[] basicOp = {'+', '-', '*', '/','(',')'};
            while (start != arr.Length) {
                switch (ty) {
                    case TokenType.Integer:
                        if (cur != arr.Length && (arr[cur] == '.' || arr[start] == '.')) {
                            ty = TokenType.FloatPoint;
                            continue;
                        }
                        if (cur == arr.Length || !IsDigit(arr[cur])) {
                            ty = TokenType.Operator;
                            if (cur - start == 0) {
                                break;
                            }
                            var res = new Token(long.Parse(st.Substring(start, cur - start)));
                            start = cur;
                            yield return res;
                        }
                        break;
                    case TokenType.FloatPoint:
                        if (cur == arr.Length || !(IsDigit(arr[cur])||arr[cur]=='.')) {
                            ty = cur == arr.Length ? TokenType.Integer : TokenType.Operator;
                            var res = new Token(double.Parse(st.Substring(start, cur - start)));
                            start = cur;
                            yield return res;
                        }
                        break;
                    case TokenType.Operator:
                        if (cur == start) {
                            ty = TokenType.Integer;
                            break;
                        }
                        while (start < arr.Length && basicOp.Contains(arr[start])) {
                            yield return new Token(st.Substring(start++, 1));
                            cur = start;
                        }
                        if (cur == arr.Length || IsDigit(arr[cur]) || arr[cur] == '.') {
                            ty = TokenType.Integer;
                            if (cur != start) {
                                var res = new Token(st.Substring(start, cur - start));
                                start = cur;
                                yield return res;
                            }
                        }
                        if (cur < arr.Length && basicOp.Contains(arr[cur])) {
                            ty = TokenType.Integer;
                            if (cur != start) {
                                var res = new Token(st.Substring(start, cur - start + 1));
                                start = ++cur;
                                yield return res;
                            }
                            continue;
                        }
                        break;
                }
                if (cur != arr.Length) cur++;
            }

        }
    }

    public class Token : IToken
    {
        public double? FloatPoint { get; protected set; }
        public long? Integer { get; protected set; }
        public string Operator { get; protected set; }
        public TokenType Type { get; protected set; }

        public Token(string str)
        {
            Type = TokenType.Operator;
            Operator = str;
        }

        public Token(double db)
        {
            Type = TokenType.FloatPoint;
            FloatPoint = db;
        }

        public Token(long i)
        {
            Type = TokenType.Integer;
            Integer = i;
        }

        public override string ToString()
        {
            switch (Type) {
                case TokenType.FloatPoint:
                    return FloatPoint.ToString();
                case TokenType.Integer:
                    return Integer.ToString();
                case TokenType.Operator:
                    return Operator;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
