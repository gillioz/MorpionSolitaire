using System.Reflection;

namespace MorpionSolitaire;

public class Grid
{
    public List<GridAction> Actions { get; }

    public Grid()
    {
        Actions = new List<GridAction>();
    }

    private static Grid GridFromCoordinates(List<GridCoordinates> dots)
    {
        var action = new GridAction();
        foreach (GridCoordinates dot in dots)
        {
            action.Add(new GridDot(dot));
        }

        var grid = new Grid();
        grid.Actions.Add(action);
        return grid;
    }
    
    public static Grid Cross()
    {
        return GridFromCoordinates(new List<GridCoordinates>()
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
    
    public static Grid Pipe()
    {
        return GridFromCoordinates(new List<GridCoordinates>()
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
        Actions.Add(action);
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

    // public string ToJson(string spacing = "")
    // {
    //     var result = spacing + "[\n";
    //     for (int i = 0; i < Actions.Count; i++)
    //     {
    //         result += spacing + "\t{\n" +
    //                   spacing + $"\t\t\"stage\": {i},\n" +
    //                   spacing + "\t\t\"actions\":\n" +
    //                   Actions[i].ToJson(spacing + "\t\t") + "\n" +
    //                   spacing + "\t},\n";
    //     }
    //     result = result.Remove(result.Length - 2) + "\n";
    //     result += spacing + "]";
    //     return result;
    // }
    
    public string ToSvg(string spacing = "")
    {
        var result = string.Empty;
        for (int i = 0; i < Actions.Count; i++)
        {
            result += Actions[i].ToSvg($"grid-action-{i}", spacing) + "\n";
        }

        return result;
    }

    public int GetScore()
    {
        return Actions.Count - 1;
    }
}