namespace MorpionSolitaireCLI;

public class ProgressBar
{
    private long _n;
    private long _step;
    private long _completedSteps;

    public void Initialize(long n)
    {
        _n = n;
        _step = n < 100 ? 100 : n / 100;
        _completedSteps = 0;

        Console.WriteLine();
        Console.WriteLine(
            "0%      10%       20%       30%       40%       50%       60%       70%       80%       90%     100%");
        Console.WriteLine(
            "---------+---------+---------+---------+---------+---------+---------+---------+---------+---------+");
    }

    public void Iterate()
    {
        var newsteps = (_n - _completedSteps) / _step;
        if (newsteps > 0)
        {
            Console.Write(new string('*', (int)newsteps));
            _completedSteps += newsteps * _step;
        }
    }

    public void Terminate()
    {
        Console.WriteLine();
        Console.WriteLine();
    }
}