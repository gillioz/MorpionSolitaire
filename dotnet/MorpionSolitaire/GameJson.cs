namespace MorpionSolitaire;

public class GameJson
{
    public string Title { get; set; }
    public string Version { get; set; }
    public int SegmentLength { get; set; }
    public bool NoTouchingRule { get; set; }
    public List<GridActionJson> Grid { get; set; }

    public GameJson(Game game)
    {
        Title = "Morpion Solitaire";
        Version = "v1";
        SegmentLength = game.SegmentLength;
        NoTouchingRule = game.NoTouchingRule;
        Grid = new List<GridActionJson>();
        foreach (var action in game.Grid.Actions)
        {
            Grid.Add(new GridActionJson(action));
        }
    }

    public Game ToGame()
    {
        if (Title != "Morpion Solitaire")
        {
            throw new Exception("This is not a Morpion Solitaire game.");
        }
        
        if (Version != "v1")
        {
            throw new Exception("Version conflict: only version 'v1' is supported.");
        }

        var grid = new Grid();

        foreach (var actionJson in Grid)
        {
            grid.Apply(actionJson.ToGridAction());
        }

        return new Game(SegmentLength, NoTouchingRule, grid);
    }
}

public class GridActionJson
{
    public List<GridElementJson> Elements { get; set; }

    public GridActionJson(GridAction gridAction)
    {
        Elements = new List<GridElementJson>();
        foreach (var element in gridAction.Elements)
        {
            Elements.Add(element.ToGridElementJson());
        }
    }

    public GridAction ToGridAction()
    {
        var gridAction = new GridAction();
        foreach (var element in Elements)
        {
            gridAction.Add(element.ToGridElement());
        }

        return gridAction;
    }
}

public class GridElementJson
{
    public string Type { get; set; }
    public List<GridCoordinates> Coordinates { get; set; }
    
    public GridElementJson(GridDot dot)
    {
        Type = "dot";
        Coordinates = new List<GridCoordinates>()
        {
            dot.Pt
        };
    }

    public GridElementJson(GridLine line)
    {
        Type = "line";
        Coordinates = new List<GridCoordinates>()
        {
            line.Pt1,
            line.Pt2
        };
    }

    public GridElement ToGridElement()
    {
        switch (Type)
        {
            case "dot":
                if (Coordinates.Count != 1)
                {
                    throw new Exception("Element 'dot' must have exactly one coordinate.");
                }
                return new GridDot(Coordinates[0]);
                break;
            case "line":
                if (Coordinates.Count != 2)
                {
                    throw new Exception("Element 'dot' must have exactly one coordinate.");
                }
                return new GridLine(Coordinates[0], Coordinates[1]);
                break;
            default:
                throw new Exception($"Unknown element of type '{Type}'.");
        }
    }
}