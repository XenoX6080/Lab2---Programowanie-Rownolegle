using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal interface IFunction
{
    string Name { get; }
    double GetY(double x);
}

internal interface IIntegrationMethod
{
    string Name { get; }
    double Integrate(IFunction function, double start, double end, int steps, IProgress<int> progress, CancellationToken token);
}


internal class LinearFunction : IFunction
{
    public string Name => "y = 2x + 2x^2";

    public double GetY(double x) => 2 * x + 2 * x * x;
}

internal class QuadraticFunction : IFunction
{
    public string Name => "y = 2x^2 + 3";

    public double GetY(double x) => 2 * x * x + 3;
}

internal class CubicFunction : IFunction
{
    public string Name => "y = 3x^2 + 2x - 3";

    public double GetY(double x) => 3 * x * x + 2 * x - 3;
}


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



class Program
{
    static async Task Main(string[] args)
    {
        var functions = new List<IFunction>
        {
            new LinearFunction(),
            new QuadraticFunction(),
            new CubicFunction()
        };

        var method = new TrapezoidalMethod();

        Console.WriteLine("Wybierz funkcję:");
        for (int i = 0; i < functions.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {functions[i].Name}");
        }

        int functionIndex = int.Parse(Console.ReadLine()) - 1;
        var selectedFunction = functions[functionIndex];

        Console.Write("Podaj początek przedziału: ");
        double start = double.Parse(Console.ReadLine());
        Console.Write("Podaj koniec przedziału: ");
        double end = double.Parse(Console.ReadLine());
        Console.Write("Podaj liczbę kroków: ");
        int steps = int.Parse(Console.ReadLine());

        var cts = new CancellationTokenSource();
        var progress = new Progress<int>(percent =>
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"Postęp: {percent}%   "); 
        });

        try
        {
            Console.WriteLine("Rozpoczynanie obliczeń...");
            var task = Task.Run(() => method.Integrate(selectedFunction, start, end, steps, progress, cts.Token));

            Console.WriteLine("Naciśnij 'q', aby przerwać.");
            while (!task.IsCompleted)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    cts.Cancel();
                }
            }

            double result = await task;
            Console.WriteLine($"Wynik: {result}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Obliczenia zostały przerwane.");
        }
    }
}

