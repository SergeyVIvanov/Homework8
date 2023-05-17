using System.Diagnostics;

using Duration = System.Int64;
using Durations = System.Collections.Generic.Dictionary<ComputationMethods, System.Int64>;
using NumberCount = System.Int32;

var statistics = new List<Dictionary<NumberCount, Durations>>();

for (int i = 0; i < 20; i++)
{
    var statisticsItem = new Dictionary<NumberCount, Durations>();
    Array.ForEach(new NumberCount[] { 100_000, 1_000_000, 10_000_000 }, (numberCount) => {
        statisticsItem.Add(numberCount, Do(numberCount));
    });
    statistics.Add(statisticsItem);
}

var numberCounts = statistics.FirstOrDefault()!.Keys.ToArray();
foreach (var numberCount in numberCounts)
{
    Console.WriteLine(numberCount);
    foreach (var method in Enum.GetValues<ComputationMethods>())
    {
        var durations = statistics
            .Select(item => item[numberCount][method])
            .ToArray();
        Console.WriteLine($"    {method}: {string.Join(';', durations)} ms");
        var averageDuration = statistics
            .Select(item => item[numberCount][method])
            .Average();
        Console.WriteLine($"    {method}: {Math.Round(averageDuration, 2, MidpointRounding.AwayFromZero)} ms");
    }
}

Console.ReadKey();

static Durations Do(NumberCount numberCount)
{
    long[] a = new long[numberCount];
    for (int i = 0; i < numberCount; i++)
        a[i] = i + 1;

    return new Durations
    {
        { ComputationMethods.SingleThread, DoSingleThread(a) },
        { ComputationMethods.MultipleThreads, DoMultipleThreads(a) },
        { ComputationMethods.PLINQ, DoPLINQ(a) }
    };
}

static Duration DoPLINQ(long[] a)
{
    return MeasureDuration(() =>
        a.AsParallel().Sum()
    );
}

static Duration DoMultipleThreads(long[] a)
{
    const int ThreadCount = 10;

    return MeasureDuration(() =>
    {
        long[] sums = new long[ThreadCount];
        Thread[] threads = new Thread[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            threads[i] = new Thread(i =>
            {
                int index = (int)i!;
                int countForThread = a.Length / ThreadCount;
                sums[index] = DoSum(a, countForThread * index, countForThread * (index + 1) - 1);
            });
        }

        for (int i = 0; i < ThreadCount; i++)
            threads[i].Start(i);
        Array.ForEach(threads, t => t.Join());

        return sums.Sum();
    });
}

static Duration DoSingleThread(long[] a)
{
    return MeasureDuration(() => DoSum(a, 0, a.Length - 1));
}

static long DoSum(long[] a, int firstIndex, int lastIndex)
{
    long sum = 0;
    for (int i = firstIndex; i <= lastIndex; i++)
        sum += a[i];
    return sum;
}

static Duration MeasureDuration(Func<long> f)
{
    Stopwatch timer = new();
    timer.Start();

    f();

    timer.Stop();

    return timer.ElapsedMilliseconds;
}

enum ComputationMethods { SingleThread, MultipleThreads, PLINQ }
