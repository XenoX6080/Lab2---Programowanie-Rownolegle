internal class CubicFunction : IFunction
{
    public string Name => "y = 3x^2 + 2x - 3";

    public double GetY(double x) => 3 * x * x + 2 * x - 3;
}

