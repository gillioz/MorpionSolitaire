using MorpionSolitaire;
using MorpionSolitaireGraph;

namespace MorpionSolitaireCLI;

public class Program
{
    private static long _n;
    private static Timing? _timing;
    private static ProgressBar? _progressBar;
    private static string _dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
    private static Histogram? _maxHistogram;
    private static RevertMode _revertMode = RevertMode.Restart;

    static void Main(string[] args)
    {
        ParseArguments(args);
 
        Console.WriteLine($"Running {_n} games");

        _progressBar?.Initialize(_n);
        _timing?.Start();
        var graph = new GameGraph(Grid.Cross());
        for (long i = 0; i < _n; i++)
        {
            graph.PlayAtRandom();
            var score = graph.GetScore();

            switch(_revertMode) 
            {
                case RevertMode.DiscardedBranch:
                    graph.RevertAndPlayDiscardedBranchAtRandom();
                    break;
                default:
                    graph.Restart();
                    break;
            }
            
            _maxHistogram?.Add(score);
            _progressBar?.Update(i);
        }
        _timing?.Stop();

        _progressBar?.Terminate();
        _timing?.Print(_n);

        _maxHistogram?.Save(Path.Combine(_dataFolder, "maxHistogram.csv"));
    }

    private static void ParseArguments(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage example:");
            Console.WriteLine("MorpionSolitaireCLI.exe -n 1000 --timing --progress --path data/ --maxHistogram");
            return;
        }

        var index = 0;
        while (index < args.Length)
        {
            var flag = args[index];
            if (flag == "-n")
            {
                index += 1;
                if (index >= args.Length)
                {
                    throw new ArgumentException();
                }
                _n = long.Parse(args[index]);
            }
            else if (flag == "--timing")
            {
                _timing = new Timing();
            }
            else if (flag == "--progress")
            {
                _progressBar = new ProgressBar();
            }
            else if (flag == "--path")
            {
                index += 1;
                if (index >= args.Length)
                {
                    throw new ArgumentException();
                }
                _dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, args[index]);
                Console.WriteLine($"Writing all data to : '{_dataFolder}'");
            }
            else if (flag == "--maxHistogram")
            {
                _maxHistogram = new Histogram();
            }
            else if (flag == "--playDiscardedBranches")
            {
                _revertMode = RevertMode.DiscardedBranch;
            }
            else
            {
                Console.WriteLine($"Unknown flag '{flag}'");
                throw new ArgumentException();
            }

            index += 1;
        }
    }

    private enum RevertMode
    {
        Restart,
        DiscardedBranch
    }
}