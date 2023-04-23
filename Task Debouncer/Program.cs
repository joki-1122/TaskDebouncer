using System.Diagnostics;
using Task_Debouncer;

var iterations = Enumerable.Range(0, 50);
var results = new List<string>();
var debouncer = new Debouncer();

var stopwatch = Stopwatch.StartNew();

await Parallel.ForEachAsync(iterations, async (_, _) =>
{
    var result = await debouncer.DebounceAsync("longRunningTask",
        async () => await GetResultFromLongRunningTaskAsync());

    results.Add(result);
});

stopwatch.Stop();
Console.WriteLine($"Elapsed milliseconds: {stopwatch.ElapsedMilliseconds}");

foreach (var result in results)
{
    Console.WriteLine(result);
}

async Task<string> GetResultFromLongRunningTaskAsync()
{
    await Task.Delay(10000);
    return "Task finished";
}