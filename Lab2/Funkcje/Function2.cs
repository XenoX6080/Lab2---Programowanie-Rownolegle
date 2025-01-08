internal class Function2 : IFunction
{
    public string Name => "y = 2x^2 + 3";

    public double GetY(double x) => 2 * x * x + 3;
}

