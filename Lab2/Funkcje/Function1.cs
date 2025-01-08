internal class Function1 : IFunction
{
    public string Name => "y = 2x + 2x^2";

    public double GetY(double x) => 2 * x + 2 * x * x;
}

