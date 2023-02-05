namespace MorpionSolitaireCLI;

public class ProgressBar
{
    private long _n;
    private long _state;

    public void Initialize(long n)
    {
        _n = n;
        _state = 0;

        Console.WriteLine();
        Console.WriteLine(
            "0%      10%       20%       30%       40%       50%       60%       70%       80%       90%     100%");
        Console.WriteLine(
            "---------+---------+---------+---------+---------+---------+---------+---------+---------+---------+");
        Console.Write("*");
    }

    public void Update(long i)
    {
        var increment = ((100 * i) / _n) - _state;
        if (increment > 0)
        {
            Console.Write(new string('*', (int)increment));
            _state += increment;
        }
    }

    public void Terminate()
    {
        Console.WriteLine();
        Console.WriteLine();
    }
}