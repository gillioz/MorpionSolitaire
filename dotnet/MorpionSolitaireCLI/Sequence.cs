namespace MorpionSolitaireCLI;

public class Sequence
{
    private readonly List<int> _score;
    private readonly List<int> _start;

    public Sequence()
    {
        _score = new List<int>();
        _start = new List<int>() { 0 };
    }

    public void RecordScore(int value)
    {
        _score.Add(value);
    }

    public void RecordStart(int value)
    {
        _start.Add(value);
    }

    public void Save(string path, bool verbose = true)
    {
        if (verbose)
        {
            Console.Write($"Writing file to '{path}'...");
        }

        var output = new List<string>();
        var count = Math.Min(_score.Count, _start.Count);
        for(var i = 0; i < count; i++)
        {
            output.Add($"{_start[i]}, {_score[i]}");
        }
        File.WriteAllLines(path, output);

        if (verbose)
        {
            Console.WriteLine("done");
        }
    }
}