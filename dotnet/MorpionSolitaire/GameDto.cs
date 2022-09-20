﻿namespace MorpionSolitaire;

public class GameDto
{
    public string Title { get; init; }
    public string Version { get; init; }
    public int SegmentLength { get; init; }
    public bool NoTouchingRule { get; init; }
    public List<GridActionDto> Grid { get; init; }

    public GameDto()
    {
        Title = "";
        Version = "";
        SegmentLength = 0;
        NoTouchingRule = false;
        Grid = new List<GridActionDto>();
    }
    
    public GameDto(Game game)
    {
        Title = "Morpion Solitaire";
        Version = "v1";
        SegmentLength = game.SegmentLength;
        NoTouchingRule = game.NoTouchingRule;
        Grid = new List<GridActionDto>();
        foreach (var action in game.Grid.Actions)
        {
            Grid.Add(new GridActionDto(action));
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

public class GridActionDto
{
    public List<GridElementDto> Elements { get; init; }


    public GridActionDto()
    {
        Elements = new List<GridElementDto>();
    }
    
    public GridActionDto(GridAction gridAction)
    {
        Elements = new List<GridElementDto>();
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

public class GridElementDto
{
    public string Type { get; init; }
    public List<GridCoordinatesDto> Coordinates { get; init; }

    public GridElementDto()
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
                break;
            case "line":
                if (Coordinates.Count != 2)
                {
                    throw new Exception("Element 'dot' must have exactly one coordinate.");
                }
                return new GridLine(Coordinates[0].ToGridCoordinates(), Coordinates[1].ToGridCoordinates());
                break;
            default:
                throw new Exception($"Unknown element of type '{Type}'.");
        }
    }
}

public class GridCoordinatesDto
{
    public int X { get; init; }
    public int Y { get; init; }

    public GridCoordinatesDto()
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