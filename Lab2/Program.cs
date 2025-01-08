using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var functions = new List<IFunction>
        {
            new Function1(),
            new Function2(),
            new Function3()
        };

        var methods = new List<IMethod>
        {
            new TrapezoidalMethod(),
            new RectangularLeftMethod(),
            new RectangularRightMethod(),
            new RectangularMiddleMethod()
        };

        var selectedFunction = SelectFromList(functions, "\nWybierz funkcję:");
        var selectedMethod = SelectFromList(methods, "\nWybierz metodę:");

        double start = ReadDouble("Podaj początek przedziału: ");
        double end = ReadDouble("Podaj koniec przedziału: ");
        int steps = ReadInt("Podaj liczbę kroków: ");

        var cts = new CancellationTokenSource();
        var progress = new Progress<int>(percent =>
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"Postęp: {percent}%   \n");
        });

        try
        {
            Console.WriteLine("Rozpoczynanie obliczeń...");
            var overallStopwatch = Stopwatch.StartNew(); // Start measuring overall runtime

            var task = Task.Run(() => selectedMethod.Integrate(selectedFunction, start, end, steps, progress, cts.Token));

            Console.WriteLine("Naciśnij 'q', aby przerwać.");
            while (!task.IsCompleted)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    cts.Cancel();
                }
            }

            double result = await task;
            overallStopwatch.Stop(); // Stop measuring overall runtime

            Console.WriteLine($"Wynik: {result}");
            Console.WriteLine($"Całkowity czas wykonania: {overallStopwatch.ElapsedMilliseconds} ms");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Obliczenia zostały przerwane.");
        }
    }

    static IFunction SelectFromList(List<IFunction> options, string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i].Name}");
            }

            Console.Write("\nWybierz opcję: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= options.Count)
            {
                return options[choice - 1];
            }

            Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
        }
    }

    static IMethod SelectFromList(List<IMethod> options, string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i].Name}");
            }

            Console.Write("\nWybierz opcję: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= options.Count)
            {
                return options[choice - 1];
            }

            Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
        }
    }
    static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (double.TryParse(Console.ReadLine(), out double value))
            {
                return value;
            }

            Console.WriteLine("Nieprawidłowy format liczby. Spróbuj ponownie.");
        }
    }

    static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value))
            {
                return value;
            }

            Console.WriteLine($"Nieprawidłowy format liczby. Spróbuj ponownie.");
        }
    }
}
