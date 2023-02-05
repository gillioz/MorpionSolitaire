using System.Diagnostics;

namespace MorpionSolitaireCLI;

public class Timing
{
    private readonly Stopwatch _watch;

    public Timing()
    {
        _watch = new Stopwatch();
    }

    public void Start()
    {
        _watch.Start();
    }

    public void Stop()
    {
        _watch.Stop();
    }

    public void Print(long n)
    {
        var elapsedSeconds = Convert.ToDouble(_watch.ElapsedMilliseconds) / 1000.0;
        var gamesPerSecond = Convert.ToInt32(Convert.ToDouble(n) / elapsedSeconds);
        var elapsedTime = elapsedSeconds > 60 ? $"{elapsedSeconds / 60} min {elapsedSeconds % 60} s" : $"{elapsedSeconds} s";

        Console.WriteLine("Running time: " + elapsedTime);
        Console.WriteLine($"Games per second: {gamesPerSecond}");
    }
}