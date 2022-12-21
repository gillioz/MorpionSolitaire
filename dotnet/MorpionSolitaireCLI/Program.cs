using MorpionSolitaireGraph;

const int segmentLength = 4;
const bool noTouchingRule = false;

long nmax = 100000;
var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../data/count.csv");
var watch = new System.Diagnostics.Stopwatch();

Console.WriteLine($"Runing {nmax} games");
Console.WriteLine(
    "0%      10%       20%       30%       40%       50%       60%       70%       80%       90%     100%");
Console.WriteLine(
    "---------+---------+---------+---------+---------+---------+---------+---------+---------+---------+");
var occurenceCount = new OccurenceCount();
watch.Start();
long step = nmax < 100 ? 100 : nmax / 100;
long completedSteps = 0;
for (long n = 0; n < nmax; n++)
{
    var graph = new GameGraph();
    graph.PlayAtRandom();
    var score = graph.Game.GetScore();
    occurenceCount.Add(score);
    var newsteps = (n - completedSteps) / step;
    if (newsteps > 0)
    {
        Console.Write(new string('*', (int)newsteps));
        completedSteps += newsteps * step;
    }
}
Console.WriteLine();
watch.Stop();
var elapsedSeconds = Convert.ToDouble(watch.ElapsedMilliseconds) / 1000.0;
var gamesPerSecond = Convert.ToInt32(Convert.ToDouble(nmax) / elapsedSeconds);

Console.WriteLine($"Running time: {elapsedSeconds} s");
Console.WriteLine($"Games per second: {gamesPerSecond}");

Console.Write($"Writing file to '{filename}'...");
File.WriteAllLines(filename, occurenceCount.Value.Select(x => x.ToString()));
Console.WriteLine("done");

public class OccurenceCount
{
    public List<int> Value { get; }

    public OccurenceCount()
    {
        Value = new List<int>(new int[80]);
    }

    public void Add(int index)
    {
        if (index <= 0) return;
        if (index > Value.Count)
        {
            Value.Add(0);
            Add(index);
        }
        else
        {
            Value[index - 1] += 1;
        }
    }
}