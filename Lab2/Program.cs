using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

