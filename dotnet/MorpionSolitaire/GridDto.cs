using System.Text.Json;

namespace MorpionSolitaire;

public class GridDto
{
    public string Title { get; init; }
    public string Version { get; init; }
    public int SegmentLength { get; init; }
    public bool NoTouchingRule { get; init; }
    public List<GridActionDto> Grid { get; init; }

    public GridDto() // default constructor is needed for JSON deserialization
    {
        Title = "";
        Version = "";
        SegmentLength = 0;
        NoTouchingRule = false;
        Grid = new List<GridActionDto>();
    }
    
    public GridDto(Grid grid)
    {
        Title = "Morpion Solitaire";
        Version = "v1";
        SegmentLength = grid.SegmentLength;
        NoTouchingRule = grid.NoTouchingRule;
        Grid = grid.Actions.Reverse().Select(action => new GridActionDto(action)).ToList();
    }

    public static GridDto FromJson(string json)
    {
        var gridDto = JsonSerializer.Deserialize<GridDto>(json);
        if (gridDto is null)
        {
            throw new Exception("Could not parse JSON file.");
        }
        return gridDto;
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

        foreach (var gridActionDto in Grid)
        {
            grid.Apply(gridActionDto.ToGridAction());
        }

        return grid;
    }
}

public class GridActionDto
{
    public List<GridElementDto> Elements { get; init; }


    public GridActionDto() // default constructor is needed for JSON deserialization
    {
        Elements = new List<GridElementDto>();
    }
    
    public GridActionDto(GridAction gridAction)
    {
        Elements = gridAction.Elements.Select(element => element.ToGridElementJson()).ToList();
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

public class GridElementDto
{
    public string Type { get; init; }
    public List<GridCoordinatesDto> Coordinates { get; init; }

    public GridElementDto() // default constructor is needed for JSON deserialization
    {
        Type = "";
        Coordinates = new List<GridCoordinatesDto>();
    }
    
    public GridElementDto(GridDot dot)
    {
        Type = "dot";
        Coordinates = new List<GridCoordinatesDto>()
        {
            new GridCoordinatesDto(dot.Pt)
        };
    }

    public GridElementDto(GridLine line)
    {
        Type = "line";
        Coordinates = new List<GridCoordinatesDto>()
        {
            new GridCoordinatesDto(line.Pt1),
            new GridCoordinatesDto(line.Pt2)
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
                return new GridDot(Coordinates[0].ToGridCoordinates());
            case "line":
                if (Coordinates.Count != 2)
                {
                    throw new Exception("Element 'dot' must have exactly one coordinate.");
                }
                return new GridLine(Coordinates[0].ToGridCoordinates(), Coordinates[1].ToGridCoordinates());
            default:
                throw new Exception($"Unknown element of type '{Type}'.");
        }
    }
}

public class GridCoordinatesDto
{
    public int X { get; init; }
    public int Y { get; init; }

    public GridCoordinatesDto() // default constructor is needed for JSON deserialization
    {
        X = 0;
        Y = 0;
    }

    public GridCoordinatesDto(GridCoordinates pt)
    {
        X = pt.X;
        Y = pt.Y;
    }

    public GridCoordinates ToGridCoordinates()
    {
        return new GridCoordinates(X, Y);
    }
}
