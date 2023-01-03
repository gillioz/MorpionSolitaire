namespace MorpionSolitaireCLI;

public class Histogram
{
    private string _filename;

    public List<int> Bin { get; }

    public Histogram(string filename)
    {
        _filename = filename;
        Bin = new List<int> { 0 };
    }

    public void Add(int value)
    {
        if (value < 0) return;

        while (value >= Bin.Count)
        {
            Bin.Add(0);
        }

        Bin[value] += 1;
    }

    public void Save(bool verbose = true)
    {
        if (verbose)
        {
            Console.Write($"Writing file to '{_filename}'...");
        }

        File.WriteAllLines(_filename, Bin.Select(x => x.ToString()));

        if (verbose)
        {
            Console.WriteLine("done");
        }
    }
}