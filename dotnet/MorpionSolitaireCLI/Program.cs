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
    private static Func<int, double>? _weightFunction = null;
    private static double _weightOffset = 1.0;
    private static int _weightPower = 0;

    static void Main(string[] args)
    {
        ParseArguments(args);

        if (_n > 0)
        {
            Console.WriteLine($"Running {_n} games");
            Loop();
        }
    }

    private static void Loop()
    {
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
                case RevertMode.RandomNode:
                    graph.RevertToRandomNode(x => (double)x + 10.0);
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
            Console.WriteLine("");
            Console.WriteLine("Usage example:");
            Console.WriteLine("  MorpionSolitaireCLI.exe -n <number of games to run>");
            Console.WriteLine("  dotnet run --project MorpionSolitaireCLI -- -n <number of games to run>");
            Console.WriteLine("");
            Console.WriteLine("Optional flags");
            Console.WriteLine("");
            Console.WriteLine("    --timing      : show the running time");
            Console.WriteLine("    --progress    : display a progress bar");
            Console.WriteLine("");
            Console.WriteLine("    --path <path>    : directory in which data is saved");
            Console.WriteLine("    --maxHistogram   : save histogram with score occurence");
            Console.WriteLine("");
            Console.WriteLine("    --revertMode <mode>     : 'Restart' (default), 'RandomNode', 'DiscardedBranch'");
            Console.WriteLine("    --weightPower <int>     : use a weighted probability with satisfying");
            Console.WriteLine("    --weightOffset <double>     [function(score) = score^power + offset]");
            Console.WriteLine("");
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
            else if (flag == "--revertMode")
            {
                index += 1;
                if (index >= args.Length)
                {
                    throw new ArgumentException();
                }
                if (!Enum.TryParse<RevertMode>(args[index], out _revertMode))
                {
                    throw new ArgumentException();
                }
            }
            else if (flag == "--weightPower")
            {
                index += 1;
                if (index >= args.Length)
                {
                    throw new ArgumentException();
                }
                if (int.TryParse(args[index], out _weightPower))
                {
                    _weightFunction = (x => Math.Pow(x, _weightPower) + _weightOffset);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else if (flag == "--weightOffset")
            {
                index += 1;
                if (index >= args.Length)
                {
                    throw new ArgumentException();
                }
                if (double.TryParse(args[index], out _weightOffset))
                {
                    _weightFunction = (x => Math.Pow(x, _weightPower) + _weightOffset);
                }
                else
                {
                    throw new ArgumentException();
                }
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
        RandomNode,
        DiscardedBranch
    }
}