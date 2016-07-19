using System.Collections.Generic;
using static LiCalculator.Helper;
using static System.Math;
using ItemList = System.Collections.Generic.List<LiCalculator.Item>;

namespace LiCalculator
{
    public class Item
    {
        public IExpression Exp;
        public bool? Attr { get; set; } = null;
    }

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

    public class Brace : IExpression
    {
        public ItemList Contents;
        public bool Init => true;
        public IValue Value => null;
        public string Origin { get; set; }
    }

    public class Additive : IExpression
    {
        public const bool POSITIVE = true;
        public const bool NEGATIVE = false;
        public bool Sign;
        public ItemList Operand;
        public bool Init => Operand.Count >= 2;
        public IValue Value { get; set; }
        public string Origin { get; set; }
    }

    public class Multiplicative : IExpression
    {
        public const bool NUMERATOR = true;
        public const bool DENOMINATOR = false;
        public bool Sign;
        public ItemList Operand;
        public bool Init => Operand.Count >= 2;
        public IValue Value { get; set; }
        public string Origin { get; set; }
    }

    public class Functional : IExpression
    {
        public FuncType Func;
        public ItemList Operand;
        public bool Init => Operand.Count >= 1;
        public IValue Value { get; set; }
        public string Origin { get; set; }
    }
}