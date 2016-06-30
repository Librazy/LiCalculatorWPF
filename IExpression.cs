namespace LiCalculator
{
    public interface IExpression
    {
        string Origin { get; set; }
        bool Init { get; }
        IValue Value { get; }
    }

    public interface IBinary : IExpression
    {
        IExpression LeftOperand { get; set; }
        IExpression RightOperand { get; set; }
    }

    public interface ILBinary : IBinary
    {

    }

    public interface IHBinary : IBinary
    {
    }

    public interface IUnary : IExpression
    {
        IExpression Operand { get; set; }
    }
}
