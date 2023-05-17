using System.Text.Json;

namespace MorpionSolitaire;

public class Grid
{
    public int SegmentLength { get; }
    public bool NoTouchingRule { get; }
    public Stack<GridAction> Actions { get; }

    public Grid(int segmentLength, bool noTouchingRule, List<GridPoint>? dots = null)
    {
        SegmentLength = segmentLength;
        NoTouchingRule = noTouchingRule;
        Actions = new Stack<GridAction>();

        if (dots is not null)
        {
            var initialAction = new GridAction
            {
                Elements = dots.Select(dot => new GridDot(dot) as GridElement).ToList()
            };

            Actions.Push(initialAction);
        }
    }
    
    public static Grid Cross(bool noTouchingRule = false)
    {
        return new Grid(4, noTouchingRule, new List<GridPoint>
        {
            new(3, 0), new(4, 0), new(5, 0), new(6, 0),
            new(3, 1), new(6, 1), new(3, 2), new(6, 2),
            new(0, 3), new(1, 3), new(2, 3), new(3, 3),
            new(6, 3), new(7, 3), new(8, 3), new(9, 3),
            new(0, 4), new(9, 4), new(0, 5), new(9, 5),
            new(0, 6), new(1, 6), new(2, 6), new(3, 6),
            new(6, 6), new(7, 6), new(8, 6), new(9, 6),
            new(3, 7), new(6, 7), new(3, 8), new(6, 8),
            new(3, 9), new(4, 9), new(5, 9), new(6, 9)
        });
    }
    
    public static Grid Pipe(bool noTouchingRule = false)
    {
        return new Grid(4, noTouchingRule, new List<GridPoint>
        {
            new(3, 0), new(4, 0), new(5, 0), new(6, 0),
            new(2, 1), new(7, 1), new(1, 2), new(8, 2),
            new(0, 3), new(3, 3), new(4, 3), new(5, 3), new(6, 3), new(9, 3),
            new(0, 4), new(3, 4), new(6, 4), new(9, 4),
            new(0, 5), new(3, 5), new(6, 5), new(9, 5),
            new(0, 6), new(3, 6), new(4, 6), new(5, 6), new(6, 6), new(9, 6),
            new(1, 7), new(8, 7), new(2, 8), new(7, 8),
            new(3, 9), new(4, 9), new(5, 9), new(6, 9)
        });
    }

    public void Apply(GridAction action)
    {
        Actions.Push(action);
    }

    public void Undo(int steps = 1)
    {
        while (steps > 0 && Actions.Count > 1)
        {
            Actions.Pop();
            steps -= 1;
        }
    }
    
    public GridFootprint GetFootprint()
    {
        var footprint = new GridFootprint();
        foreach (var action in Actions)
        {
            action.ComputeFootprint(footprint);
        }
        return footprint;
    }
    
    public string ToSvg()
    {
        var result = "";
        var actions = Actions.Reverse();
        foreach (var action in actions)
        {
            result += action.ToSvg(grouped: true);
        }

        return result;
    }

    public int GetScore()
    {
        return Actions.Count - 1;
    }
    
    public string ToJson()
    {
        var dto = new GameDto(this);
        var options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(dto, options);
    }

    public void Save(string file, bool overwrite = false)
    {
        if (!overwrite && File.Exists(file))
        {
            throw new Exception($"File '{file}' exists already.");
        }
        var json = ToJson();
        using (var outputFile = new StreamWriter(file))
        {
            outputFile.Write(json);
        }
    }

    public static Grid Load(string file)
    {
        if (!File.Exists(file))
        {
            throw new Exception($"File '{file}' cannot be found.");
        }
        string json;
        using (var reader = new StreamReader(file))
        {
            json = reader.ReadToEnd();
        }

        return GameDto.FromJson(json).ToGrid();
    }
}