namespace LiCalculator
{
    public interface IToken
    {
        double? FloatPoint { get; }
        long? Integer { get; }
        string Operator { get; }
        TokenType Type { get; }

        string ToString();
    }
}