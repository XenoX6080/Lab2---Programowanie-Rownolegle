internal class RectangularLeftMethod : IMethod
{
    public string Name => "Rectangular (left) Method";

    public double Integrate(IFunction function, double start, double end, int steps, IProgress<int> progress, CancellationToken token)
    {
        double stepSize = (end - start) / steps; // Obliczanie wielkości kroku
        double result = 0.0;
        object lockObject = new object(); // Obiekt do synchronizacji
        int completedSteps = 0;
        int lastReportedProgress = 0;
        int chunkSize = steps / Environment.ProcessorCount; // dzielenie na chunki równemu liczby procesorów

        Parallel.Invoke(
            () =>
            {
                Parallel.For(0, Environment.ProcessorCount, i =>
                {
                    int chunkStart = i * chunkSize;
                    int chunkEnd = (i == Environment.ProcessorCount - 1) ? steps : chunkStart + chunkSize;

                    Parallel.For(chunkStart, chunkEnd, new ParallelOptions { CancellationToken = token }, j =>
                    {
                        Thread.Sleep(1); // Symulowanie długiej pracy kodu

                        if (token.IsCancellationRequested)
                            return;

                        double current = start + j * stepSize; // Pozycja x
                        double next = current + stepSize; // Następna pozycja x
                        double localResult = function.GetY(current) * stepSize; // Liczenie prostokątne

                        lock (lockObject)
                        {
                            result += localResult; // dodawanie lokalnego wyniku do ogólnego
                        }

                        int currentProgress = Interlocked.Increment(ref completedSteps) * 100 / steps;
                        if (currentProgress >= lastReportedProgress + 10) // Sprawdź czy progress zwiększył się o 10%
                        {
                            lock (lockObject)
                            {
                                lastReportedProgress = currentProgress; // Update
                                progress?.Report(lastReportedProgress); // Report
                            }
                        }
                    });
                });
            });

        return result;
    }
}