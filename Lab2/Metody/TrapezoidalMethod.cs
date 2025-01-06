internal class TrapezoidalMethod : IIntegrationMethod
{
    public string Name => "Trapezoidal Method";

    public double Integrate(IFunction function, double start, double end, int steps, IProgress<int> progress, CancellationToken token)
    {
        double stepSize = (end - start) / steps;
        double result = 0.0;
        double current = start;

        for (int i = 0; i < steps; i++)
        {
            if (token.IsCancellationRequested)
                throw new OperationCanceledException();

            double next = current + stepSize;
            result += (function.GetY(current) + function.GetY(next)) * stepSize / 2.0;
            current = next;

            if (i % (steps / 10) == 0)
                progress?.Report((i * 100) / steps);
                Thread.Sleep(1); //zakomentować jeśli trzeba szybciej
        }

        progress?.Report(100);
        return result;
    }
}

