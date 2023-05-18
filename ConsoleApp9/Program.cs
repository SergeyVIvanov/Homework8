using System.Diagnostics;

namespace ConsoleApp9
{
    internal class Program
    {
        static void Main()
        {
            var random = new Random();
            long[] values = Enumerable.Range(1, 99999999)
                .Select(x => (long)random.Next(1, 1000))
                .ToArray();
            //Min, Max and Average LINQ extension methods
            Console.WriteLine("Min, Max and Average with LINQ");

            long linqMin = 0;// values.Sum();
            //for (int i = 0; i < values.Length; i++)
            //    linqMin += values[i];

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            linqMin = 0;// values.Sum();
            for (int i = 0; i < values.Length; i++)
                linqMin += values[i];
            stopwatch.Stop();
            var linqTimeMS = stopwatch.ElapsedMilliseconds;
            DisplayResults(linqMin, linqTimeMS);
            //Min, Max and Average PLINQ extension methods
            Console.WriteLine("\nMin, Max and Average with PLINQ");
            stopwatch.Restart();
            var plinqMin = values.AsParallel().Sum();
            stopwatch.Stop();
            var plinqTimeMS = stopwatch.ElapsedMilliseconds;
            DisplayResults(plinqMin, plinqTimeMS);

            Console.ReadKey();
        }

        static void DisplayResults(long sum, double time)
        {
            Console.WriteLine($"Sum: {sum}\nTotal time in milliseconds: {time}");
        }
    }
}