namespace MorpionSolitaireCLI;

public class Histogram
{
    private List<int> _bin { get; }

    public Histogram()
    {
        _bin = new List<int> { 0 };
    }

    public void Add(int value)
    {
        if (value < 0) return;

        while (value >= _bin.Count)
        {
            _bin.Add(0);
        }

        _bin[value] += 1;
    }

    public void Save(string path, bool verbose = true)
    {
        if (verbose)
        {
            Console.Write($"Writing file to '{path}'...");
        }

        File.WriteAllLines(path, _bin.Select(x => x.ToString()));

        if (verbose)
        {
            Console.WriteLine("done");
        }
    }
}