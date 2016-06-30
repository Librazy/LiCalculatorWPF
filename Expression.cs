using static LiCalculator.Helper;
using static System.Math;

namespace LiCalculator
{
    public class Val : IExpression
    {
        public Val() {}

        public Val(long? i)
        {
            long ni = 0;
            if (i != null) ni = (long) i;
            Value = new Fraction {
                Numerator = ni,
                Denominator = 1
            };
        }

        public Val(double? d)
        {
            double nd = 0;
            if (d != null) nd = (double) d;
            Value = new FloatPoint {
                Value = nd
            };
        }

        public bool Init => true;
        public IValue Value { get; set; }
        public string Origin { get; set; }
    }



    public class Sum : ILBinary
    {
        public IValue Value => Add(LeftOperand.Value, RightOperand.Value);
        public bool Init => LeftOperand != null && RightOperand != null;
        public IExpression LeftOperand { get; set; }
        public IExpression RightOperand { get; set; }
        public string Origin { get; set; }
    }

    public class Sub : ILBinary
    {
        public IValue Value => Sub(LeftOperand.Value, RightOperand.Value);
        public bool Init => LeftOperand != null && RightOperand != null;
        public IExpression LeftOperand { get; set; }
        public IExpression RightOperand { get; set; }
        public string Origin { get; set; }
    }

    public class Mul : IHBinary
    {
        public IValue Value => Mul(LeftOperand.Value, RightOperand.Value);
        public bool Init => LeftOperand != null && RightOperand != null;
        public IExpression LeftOperand { get; set; }
        public IExpression RightOperand { get; set; }
        public string Origin { get; set; }
    }

    public class Div : IHBinary
    {
        public IValue Value => Div(LeftOperand.Value, RightOperand.Value);
        public bool Init => LeftOperand != null && RightOperand != null;
        public IExpression LeftOperand { get; set; }
        public IExpression RightOperand { get; set; }
        public string Origin { get; set; }
    }

    public class Func : IUnary
    {
        public bool Init => Operand != null;
        public FuncType Fun { get; set; }
        public IExpression Operand { get; set; }
        public IValue Value
        {
            get {
                if (Fun != FuncType.Sqrt || !(Operand.Value is Fraction))
                    return new FuncVal {
                        Operand = Operand.Value,
                        Fun = Fun
                    };
                var v = (Fraction) Operand.Value;
                if (IsSquare(v.Denominator) && IsSquare(v.Numerator)) {
                    return new Fraction
                    {
                        Numerator = (long)Sqrt(v.Numerator),
                        Denominator = (long)Sqrt(v.Denominator)
                    };
                }
                return new FuncVal {
                    Operand = Operand.Value,
                    Fun = Fun
                };
            }
        }

        public string Origin { get; set; }
    }
}