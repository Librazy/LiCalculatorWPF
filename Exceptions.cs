using System;

namespace LiCalculator
{
    public class LiCalculatorException : ArgumentException
    {

    }
    public class FuntionOutOfRangeException : LiCalculatorException
    {

    }
    public class OperatorOutOfRangeException : LiCalculatorException
    {
        
    }
    public class UnexpectedTokenException : LiCalculatorException
    {

    }
    public class UnexpectedExpressionException : LiCalculatorException
    {

    }
}