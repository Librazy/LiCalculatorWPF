namespace LiCalculator
{
    public interface IExpression
    {
        string Origin { get; set; }
        bool Init { get; }
        IValue Value { get; }
    }
}
