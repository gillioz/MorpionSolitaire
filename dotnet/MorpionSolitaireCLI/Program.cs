using MorpionSolitaire;
using MorpionSolitaireGraph;

namespace MorpionSolitaireCLI;

public static class Program
{
    private static long _n;
    private static Timing? _timing;
    private static ProgressBar? _progressBar;
    private static string _dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
    private static Histogram? _maxHistogram;
    private static bool _maxGrids;
    private static long _sampleGrids;
    private static Sequence? _sequence;
    private static RevertMode _revertMode = RevertMode.Restart;
    private static Func<int, double>? _weightFunction;
    private static double _weightOffset = 1.0;
    private static int _weightPower;

    static void Main(string[] args)
    {
        ParseArguments(args);

        Console.WriteLine($"Writing all data to : '{_dataFolder}'");

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
        var maxScore = 0;
        var graph = new GameGraph(Grid.Cross());
        for (long i = 0; i < _n; i++)
        {
            graph.PlayAtRandom();
            var score = graph.GetScore();
            _maxHistogram?.Add(score);
            _sequence?.RecordScore(score);
            if (score > maxScore)
            {
                maxScore = score;
                if (_maxGrids)
                {
                    graph.Grid.Save(Path.Combine(_dataFolder, $"max_{maxScore}.json"));
                }
            }

            if (_sampleGrids > 0 && i % _sampleGrids == 0)
            {
                var sampleNumber = i / _sampleGrids;
                graph.Grid.Save(Path.Combine(_dataFolder, $"sample_{sampleNumber:D5}.json"));
            }

            switch(_revertMode) 
            {
                case RevertMode.DiscardedBranch:
                    graph.RevertAndPlayRandomDiscardedBranch(_weightFunction);
                    _sequence?.RecordStart(Math.Max(0, graph.GetScore() - 1));
                    break;
                case RevertMode.NextBranch:
                    graph.RevertAndPlayNextDiscardedBranch();
                    _sequence?.RecordStart(Math.Max(0, graph.GetScore() - 1));
                    break;
                case RevertMode.RandomNode:
                    graph.RevertToRandomNode(_weightFunction);
                    _sequence?.RecordStart(graph.GetScore());
                    break;
                default:
                    graph.Restart();
                    _sequence?.RecordStart(0);
                    break;
            }
            _progressBar?.Update(i);
        }
        _timing?.Stop();

        _progressBar?.Terminate();
        _timing?.Print(_n);

        _maxHistogram?.Save(Path.Combine(_dataFolder, "maxHistogram.csv"));
        _sequence?.Save(Path.Combine(_dataFolder, "sequence.csv"));
    }

    private static void Help(string? message = null)
    {
        if (message != null)
        {
            Console.WriteLine("");
            Console.WriteLine(message);
        }

        Console.WriteLine("");
        Console.WriteLine("Usage example:");
        Console.WriteLine("  MorpionSolitaireCLI.exe -n <number of games to run>");
        Console.WriteLine("  dotnet run --project MorpionSolitaireCLI -- -n <number of games to run>");
        Console.WriteLine("");
        Console.WriteLine("Optional flags");
        Console.WriteLine("");
        Console.WriteLine("    --timing      : shows the running time");
        Console.WriteLine("    --progress    : displays a progress bar");
        Console.WriteLine("");
        Console.WriteLine("    --path <path>       : directory in which data is saved");
        Console.WriteLine("    --sampleGrids <int> : saves every n-th game");
        Console.WriteLine("    --maxGrids          : saves the game every time a new highest score is attained");
        Console.WriteLine("    --maxHistogram      : saves an histogram with score occurence");
        Console.WriteLine("    --sequence          : saves a sequence of scores");
        Console.WriteLine("");
        Console.WriteLine("    --revertMode <mode>     : " +
                          "'Restart' (default), 'RandomNode', 'DiscardedBranch', 'NextBranch'");
        Console.WriteLine("    --weightPower <int>     : use a weighted probability given by the function");
        Console.WriteLine("    --weightOffset <double>     [function(score) = score^power + offset]");
        Console.WriteLine("");
    }

    private static void ParseArguments(string[] args)
    {
        if (args.Length == 0)
        {
            Help();
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
                    Help("Missing argument after flag '-n'");
                    return;
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
                    Help("Missing argument after flag '--path'");
                    return;
                }
                _dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, args[index]);
            }
            else if (flag == "--maxGrids")
            {
                _maxGrids = true;
            }
            else if (flag == "--sampleGrids")
            {
                index += 1;
                if (index >= args.Length)
                {
                    Help("Missing argument after flag '--sampleGrids'");
                    return;
                }
                if (!long.TryParse(args[index], out _sampleGrids))
                {
                    Help("Cannot parse --sampleGrids argument");
                    return;
                }
            }
            else if (flag == "--maxHistogram")
            {
                _maxHistogram = new Histogram();
            }
            else if (flag == "--sequence")
            {
                _sequence = new Sequence();
            }
            else if (flag == "--revertMode")
            {
                index += 1;
                if (index >= args.Length)
                {
                    Help("Missing argument after flag '--revertMode'");
                    return;
                }
                if (!Enum.TryParse(args[index], out _revertMode))
                {
                    Help("Cannot parse --revertMode argument");
                    return;
                }
            }
            else if (flag == "--weightPower")
            {
                index += 1;
                if (index >= args.Length)
                {
                    Help("Missing argument after flag '--weightPower'");
                    return;
                }
                if (int.TryParse(args[index], out _weightPower))
                {
                    _weightFunction = (x => Math.Pow(x, _weightPower) + _weightOffset);
                }
                else
                {
                    Help("Cannot parse --weightPower argument");
                    return;
                }
            }
            else if (flag == "--weightOffset")
            {
                index += 1;
                if (index >= args.Length)
                {
                    Help("Missing argument after flag '-weightOffset'");
                    return;
                }
                if (double.TryParse(args[index], out _weightOffset))
                {
                    _weightFunction = (x => Math.Pow(x, _weightPower) + _weightOffset);
                }
                else
                {
                    Help("Cannot parse --weightOffset argument");
                    return;
                }
            }
            else
            {
                Help($"Unknown flag '{flag}'");
                return;
            }

            index += 1;
        }
    }

    private enum RevertMode
    {
        Restart,
        RandomNode,
        DiscardedBranch,
        NextBranch
    }
}