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
        var timeSpan = TimeSpan.FromSeconds(elapsedSeconds);
        var elapsedTime = $"{timeSpan.Seconds} s";
        if (timeSpan.Minutes > 0)
        {
            elapsedTime = $"{timeSpan.Minutes} min " + elapsedTime;
        }
        if (timeSpan.Hours > 0)
        {
            elapsedTime = $"{24 * timeSpan.Days + timeSpan.Hours} h " + elapsedTime;
        }

        Console.WriteLine("Running time: " + elapsedTime);
        Console.WriteLine($"Games per second: {gamesPerSecond}");
    }
}