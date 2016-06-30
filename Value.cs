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
        public double Value
        {
            get {
                if (Denominator == 0) throw new DivideByZeroException();
                return (double) Numerator/Denominator;
            }
        }


        public override string ToString()
        {
            if (Denominator == 0) throw new DivideByZeroException();
            return Denominator == 1 ? $"{Numerator}" : $"{Numerator}/{Denominator}";
        }
    }

    public class FuncVal : IValue
    {
        public FuncType Fun { get; set; }
        public IValue Operand { get; set; }

        public override string ToString()
            => Operand is FloatPoint ? Value.ToString(CultureInfo.InvariantCulture) : $"{Fun}({Operand})";
        public double Value
        {
            get {
                switch (Fun) {
                    case FuncType.Arctan:
                    case FuncType.Arcsin:
                    case FuncType.Arccos:
                        if(Operand.Value<-1||Operand.Value>1)
                            throw new ArgumentOutOfRangeException();
                        break;
                    case FuncType.Log:
                        if (Operand.Value <= 0)
                            throw new ArgumentOutOfRangeException();
                        break;
                    case FuncType.Sqrt:
                        if (Operand.Value < 0)
                            throw new ArgumentOutOfRangeException();
                        break;
                       
                }
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