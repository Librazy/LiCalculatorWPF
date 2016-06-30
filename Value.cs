using System;
using System.Globalization;
using static System.Math;

namespace LiCalculator
{
    public enum FuncType
    {
        Sin,
        Cos,
        Tan,
        Arctan,
        Arcsin,
        Arccos,
        Sqrt,
        Exp,
        Log
    }

    public class FloatPoint : IValue
    {
        public double Value { get; set; }
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }

    public class Fraction : IValue
    {
        public long Numerator { get; set; }
        public long Denominator { get; set; }
        public double Value => (double) Numerator/Denominator;
        public override string ToString() => $"{Numerator}/{Denominator}";
    }

    public class FuncVal : IValue
    {
        public FuncType Fun { get; set; }
        public IValue Operand { get; set; }
        public override string ToString() => $"{Fun}({Operand})";
        public double Value
        {
            get {
                switch (Fun) {
                    case FuncType.Sin:
                        return Sin(Operand.Value);
                    case FuncType.Cos:
                        return Cos(Operand.Value);
                    case FuncType.Tan:
                        return Tan(Operand.Value);
                    case FuncType.Arctan:
                        return Atan(Operand.Value);
                    case FuncType.Arcsin:
                        return Asin(Operand.Value);
                    case FuncType.Arccos:
                        return Acos(Operand.Value);
                    case FuncType.Sqrt:
                        return Sqrt(Operand.Value);
                    case FuncType.Exp:
                        return Exp(Operand.Value);
                    case FuncType.Log:
                        return Log(Operand.Value);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}