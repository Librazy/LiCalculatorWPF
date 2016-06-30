using System;
using System.Collections.Generic;
using static LiCalculator.Helper;
namespace LiCalculator
{
    public class Parser
    {
        public static IExpression Parse(IToken[] tokens)
        {
            var no = 2;
            var expStack = new List<IExpression>();
            var tokenStack = new List<IToken>();
            var tokenQueue = new Queue<IToken>(tokens);
            while (true) {
                if (tokenQueue.Count > 0) { tokenStack.Push(tokenQueue.Dequeue()); }
                if (tokenStack.Count == 1 && IsVal(tokenStack.Peek())) {
                    var cur = tokenStack.Pop();
                    var exp = cur.Type == TokenType.Integer ? new Val(cur.Integer) : new Val(cur.FloatPoint);
                    expStack.Push(exp);
                } else if (tokenStack.Count == 2
                        && IsVal(tokenStack.Peek())
                        && IsOperator(tokenStack.Peek(1), "-")) {
                    var cur = tokenStack.Pop();
                    tokenStack.Pop();
                    var exp = cur.Type == TokenType.Integer ? new Val(-cur.Integer) : new Val(-cur.FloatPoint);
                    expStack.Push(exp);
                } else if (tokenStack.Count == 1
                        && IsOperator(tokenStack.Peek())
                        && ((expStack.Count > 0
                            && expStack.Peek().Init)
                            || IsOperator(tokenStack.Peek(), "-"))) {
                    var cur = tokenStack.Pop();
                    IExpression exp;
                    switch (cur.Operator) {
                        case "+":
                            exp = new Sum();
                            break;
                        case "-":
                            exp = new Sub();
                            break;
                        case "*":
                            exp = new Mul();
                            break;
                        case "/":
                            exp = new Div();
                            break;
                        default:
                            throw new OperatorOutOfRangeException();
                    }
                    expStack.Push(exp);
                } else if (tokenStack.Count == 1
                           && HasBrace(tokenStack.Peek())) {
                    var cur = tokenStack.Pop();
                    IExpression exp;
                    FuncType? f = IsFunc(cur);
                    if (f != null) {
                        exp = new Func {
                            Fun = f.Value,
                            Operand = Parse(ExtractBrace(tokenQueue))
                        };
                    } else {
                        exp = Parse(ExtractBrace(tokenQueue));
                    }
                    expStack.Push(exp);
                } else {
                    --no;
                }
                if (expStack.Count >= 3
                    && expStack.Peek().Init
                    && expStack.Peek(2).Init
                    && !expStack.Peek(1).Init
                    && expStack.Peek(1) is IHBinary) {
                    var ro = expStack.Pop();
                    var o = (IBinary)expStack.Pop();
                    var lo = expStack.Pop();
                    o.LeftOperand = lo;
                    o.RightOperand = ro;
                    expStack.Push(o);
                }else if
                    (expStack.Count >= 4
                    && !(expStack.Peek() is IHBinary)
                    && expStack.Peek(1).Init
                    && expStack.Peek(3).Init
                    && !expStack.Peek(2).Init
                    && expStack.Peek(2) is ILBinary) {
                    var t = expStack.Pop();
                    var ro = expStack.Pop();
                    var o = (IBinary)expStack.Pop();
                    var lo = expStack.Pop();
                    o.LeftOperand = lo;
                    o.RightOperand = ro;
                    expStack.Push(o);
                    expStack.Push(t);
                } else if(expStack.Count == 3 
                    && tokenQueue.Count == 0 
                    && tokenStack.Count == 0
                    && expStack.Peek().Init
                    && expStack.Peek(2).Init
                    && !expStack.Peek(1).Init
                    && expStack.Peek(1) is IBinary) {
                    var ro = expStack.Pop();
                    var o = (IBinary)expStack.Pop();
                    var lo = expStack.Pop();
                    o.LeftOperand = lo;
                    o.RightOperand = ro;
                    expStack.Push(o);
                } else if (
                    ((expStack.Count >= 3 && !(expStack.Peek(2).Init)) || expStack.Count == 2)
                   && expStack.Peek().Init
                   && !expStack.Peek(1).Init
                   && expStack.Peek(1) is Sub
                   ) {
                    var ro = expStack.Pop();
                    var o = (Sub)expStack.Pop();
                    var lo = new Val(0);
                    o.LeftOperand = lo;
                    o.RightOperand = ro;
                    expStack.Push(o);
                } else if(expStack.Count == 1 && tokenStack.Count == 0 && tokenQueue.Count == 0) {
                    break;
                } else if (no<0||expStack.Count == 2 && tokenQueue.Count == 0) {
                    throw new UnexpectedExpressionException();
                } else if (tokenQueue.Count == 0
                           && tokenStack.Count == 0) {
                    --no;
                }
            }
            return expStack.Pop();
        }
    }
}
