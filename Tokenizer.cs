using System;
using System.Collections.Generic;
using System.Linq;
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
            var ops = new[] {')', '+', '-', '*', '/', '^' };
            var list = st.Explode('(');
            list = ops.Aggregate(list, (current, op) => current.Explode(op));
            foreach (var tokstr in list) {
                long inte;
                double fp;
                if (long.TryParse(tokstr, out inte)) {
                    yield return new Token(inte);
                }else if (double.TryParse(tokstr, out fp)) {
                    yield return new Token(fp);
                } else {
                    yield return new Token(tokstr);
                }
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
