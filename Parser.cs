using System;
using System.Collections.Generic;
using static LiCalculator.Helper;
namespace LiCalculator
{
    public class Parser
    {
        public static IExpression Parse(IToken[] tokens)
        {
            var expStack = new List<IExpression>();
            var tokQueue = new Queue<IToken>(tokens);
            while (true) {
                if (expStack.Peek(1) is Functional &&
                   !expStack.Peek(1).Init) {
                    if (expStack.Peek() is Brace) {
                        ((Functional)expStack.Peek(1)).Operand = ((Brace)expStack.Peek()).Contents;
                        expStack.Pop();
                        continue;
                    }
                    if (expStack.Peek().Init) {
                        ((Functional) expStack.Peek(1)).Operand.Add(new Item() {Exp = expStack.Peek()});
                        expStack.Pop();
                        continue;
                    }
                    throw new InvalidOperationException();
                }
                if (expStack.Peek(1) is Multiplicative &&
                   !expStack.Peek(1).Init &&
                   IsFunc(tokQueue.Peek()) == null)
                {
                    if (expStack.Peek(2) is Multiplicative
                        && expStack.Peek(2).Init) {
                        ((Multiplicative)expStack.Peek(2)).Operand.Add(new Item() { Exp = expStack.Peek(), Attr = ((Multiplicative)expStack.Peek(1)).Sign });
                        expStack.Pop();
                        expStack.Pop();
                        continue;
                    }
                    if (expStack.Peek().Init && expStack.Peek(2).Init)
                    {
                        ((Multiplicative)expStack.Peek(1)).Operand.Add(new Item() { Exp = expStack.Peek(2),Attr = Multiplicative.NUMERATOR });
                        ((Multiplicative)expStack.Peek(1)).Operand.Add(new Item() { Exp = expStack.Peek(),Attr = ((Multiplicative)expStack.Peek(1)).Sign });
                        expStack.Pop(2);
                        expStack.Pop();
                        continue;
                    }
                    throw new InvalidOperationException();
                }
                if (expStack.Peek(1) is Additive &&
                   !expStack.Peek(1).Init &&
                   IsFunc(tokQueue.Peek()) == null &&
                   !IsMul(tokQueue.Peek()))
                {
                    if (expStack.Peek(2) is Additive
                        && expStack.Peek(2).Init)
                    {
                        ((Additive)expStack.Peek(2)).Operand.Add(new Item() { Exp = expStack.Peek(), Attr = ((Additive)expStack.Peek(1)).Sign });
                        expStack.Pop();
                        expStack.Pop();
                        continue;
                    }
                    if (expStack.Peek().Init && expStack.Peek(2).Init)
                    {
                        ((Additive)expStack.Peek(1)).Operand.Add(new Item() { Exp = expStack.Peek(2), Attr = Additive.POSITIVE });
                        ((Additive)expStack.Peek(1)).Operand.Add(new Item() { Exp = expStack.Peek(), Attr = ((Additive)expStack.Peek(1)).Sign });
                        expStack.Pop(2);
                        expStack.Pop();
                        continue;
                    }
                    throw new InvalidOperationException();
                }
                break;
            }
            return null;
        }
    }
}
