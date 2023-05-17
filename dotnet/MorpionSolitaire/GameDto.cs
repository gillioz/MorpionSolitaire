using System.Text.Json;

namespace MorpionSolitaire;

public class GameDto
{
    public string Title { get; init; }
    public string Version { get; init; }
    public int SegmentLength { get; init; }
    public bool NoTouchingRule { get; init; }
    public List<List<List<sbyte>>> GridData { get; init; }

    public GameDto() // default constructor is needed for JSON deserialization
    {
        Title = "";
        Version = "";
        SegmentLength = 0;
        NoTouchingRule = false;
        GridData = new List<List<List<sbyte>>>();
    }
    
    public GameDto(Grid grid)
    {
        Title = "Morpion Solitaire";
        Version = "v1";
        SegmentLength = grid.SegmentLength;
        NoTouchingRule = grid.NoTouchingRule;
        GridData = grid.Actions.Reverse().Select(GetCoordinatesList).ToList();
    }
    
    public static GameDto FromJson(string json)
    {
        var gameDto = JsonSerializer.Deserialize<GameDto>(json);
        if (gameDto is null)
        {
            throw new Exception("Could not parse JSON file.");
        }
        return gameDto;
    }

    public Grid ToGrid()
    {
        if (Title != "Morpion Solitaire")
        {
            throw new Exception("This is not a Morpion Solitaire game.");
        }
        
        if (Version != "v1")
        {
            throw new Exception("Version conflict: only version 'v1' is supported.");
        }

        var grid = new Grid(SegmentLength, NoTouchingRule);

        foreach (var action in GetActionsList(GridData))
        {
            grid.Apply(action);
        }

        return grid;
    }

    private static List<List<sbyte>> GetCoordinatesList(GridAction action)
    {
        return action.Elements.Select(element => element.ToCoordinatesList()).ToList();
    }

    private static List<GridAction> GetActionsList(List<List<List<sbyte>>> data)
    {
        return data.Select(GetAction).ToList();
    }

    private static GridAction GetAction(List<List<sbyte>> actionData)
    {
        var action = new GridAction();
        foreach (var elementData in actionData)
        {
            action.Add(GetGridElement(elementData));
        }

        return action;
    }

    private static GridElement GetGridElement(List<sbyte> data)
    {
        return data.Count switch
        {
            2 => new GridDot(new GridPoint(data[0], data[1])),
            4 => new GridLine(new GridPoint(data[0], data[1]), new GridPoint(data[2], data[3])),
            _ => throw new Exception("Invalid grid element cannot be extracted")
        };
    }
}
