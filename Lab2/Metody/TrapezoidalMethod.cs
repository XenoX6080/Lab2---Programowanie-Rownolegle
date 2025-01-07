internal class TrapezoidalMethod : IIntegrationMethod
{
    public string Name => "Trapezoidal Method";

    public double Integrate(IFunction function, double start, double end, int steps, IProgress<int> progress, CancellationToken token)
    {
        double stepSize = (end - start) / steps; // Liczenie rozmiaru kroku
        double result = 0.0;
        object lockObject = new object();
        int completedSteps = 0; // Liczba ukończonych kroków.
        // Wcześniej Progress był liczony 
        int lastReportedProgress = 0; // Ostatnio zgłoszony postęp.

        
        Parallel.For(0, steps, new ParallelOptions { CancellationToken = token }, i =>
        {
            if (token.IsCancellationRequested)
                return; // Sprawdzanie, czy żądanie anulowania zostało zgłoszone.

            double current = start + i * stepSize; // Obliczanie bieżącej pozycji x.
            double next = current + stepSize; // Obliczanie pozycji x dla następnego kroku.
            double localResult = (function.GetY(current) + function.GetY(next)) * stepSize / 2.0; // Obliczanie wyniku dla bieżącego kroku metodą trapezoidalną.

            lock (lockObject)
            {
                result += localResult; // Dodawanie lokalnego wyniku do ogólnego wyniku w sposób zsynchronizowany.
            }

            // Inkrementowanie liczby ukończonych kroków w sposób atomowy.
            int currentProgress = Interlocked.Increment(ref completedSteps) * 100 / steps;
            if (currentProgress >= lastReportedProgress + 10) // Sprawdzanie, czy postęp zwiększył się o co najmniej 10%.
            {
                lock (lockObject)
                {
                    if (currentProgress >= lastReportedProgress + 10)
                    {
                        lastReportedProgress = currentProgress; // Aktualizacja ostatnio zgłoszonego postępu.
                        progress?.Report(lastReportedProgress); // Zgłaszanie postępu.
                    }
                }
            }
            Thread.Sleep(1);
        });

        progress?.Report(100); // Zgłoszenie pełnego postępu po zakończeniu pętli.
        return result; // Zwracanie wyniku całki.
    }
}
