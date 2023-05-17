﻿namespace MorpionSolitaire;

public class Game
{
    public Grid Grid { get; init; }
    public Image Image { get; init; }

    public const int PixelsPerUnit = 20;
    private IReadOnlyList<GridPoint> _directions = new List<GridPoint>
        { new (1, 0), new (0, 1), new (1, 1), new (1, -1)};

    protected Game(int segmentLength, bool noTouchingRule)
    {
        Grid = new Grid(segmentLength, noTouchingRule);
        Image = new Image(dimensions: new GridPoint(20, 20),
            origin: new GridPoint(5, 5));
    }

    protected Game(Game game)
    {
        Grid = game.Grid;
        Image = game.Image;
    }

    public Game(Grid grid)
    {
        Grid = new Grid(grid.SegmentLength, grid.NoTouchingRule);
        Image = new Image(dimensions: new GridPoint(20, 20),
            origin: new GridPoint(5, 5));

        var actions = grid.Actions.Reverse().ToList();
        if (actions.Count == 0)
        {
            throw new Exception("Attempt to create a game with an invalid grid");
        }

        // starting configuration
        var initialAction = actions.First();
        Grid.Actions.Push(initialAction);
        if (initialAction.Elements.OfType<GridLine>().Any())
        {
            throw new Exception("Line elements are not supported at the initial stage");
        }
        foreach (var dot in initialAction.Elements.OfType<GridDot>())
        {
            Image.Set(dot.Pt.ToImagePoint(), true);
        }

        // add segments one by one
        actions.RemoveAt(0);
        foreach (var action in actions)
        {
            var dots = action.Elements.OfType<GridDot>().ToList();
            var lines = action.Elements.OfType<GridLine>().ToList();

            if (lines.Count != 1 || dots.Count != 1)
                throw new Exception("Invalid grid element.");

            var line = lines.Single();
            var dot = dots.Single();
            if (!TryApplySegment(line.Pt1, line.Pt2, dot.Pt))
            {
                throw new Exception("Invalid segment.");
            }
        }
    }

    public Segment? NewSegment(GridPoint pt1, GridPoint pt2, GridPoint? newPt = null)
    {
        var segment = Image.NewSegment(pt1, pt2, Grid.SegmentLength, Grid.NoTouchingRule);
        if (newPt.HasValue && segment is not null && !segment.Dot.Pt.Equals(newPt))
        {
            return null;
        }

        return segment;
    }

    public void ApplySegment(Segment segment)
    {
        Grid.Apply(segment.ToGridAction());
        Image.Apply(segment.ToImageAction());
    }

    public bool TryApplySegment(GridPoint pt1, GridPoint pt2, GridPoint? newPt = null)
    {
        var segment = NewSegment(pt1, pt2, newPt);
        if (segment is null) return false;

        ApplySegment(segment);
        return true;

    }

    public int GetScore()
    {
        return Grid.GetScore();
    }

    public GridFootprint GetFootPrint(bool crop = false)
    {
        return (crop) ? Grid.GetFootprint() : Image.GetFootprint();
    }
    
    private bool TryAddSegment(GridPoint pt1, GridPoint pt2, ICollection<Segment> list)
    {
        var segment = NewSegment(pt1, pt2);
        if (segment is not null)
        {
            list.Add(segment);
            return true;
        }

        return false;
    }

    public List<Segment> FindAllSegments()
    {
        var footprint = Grid.GetFootprint();
        var segments = new List<Segment>();

        for (var x = footprint.MinX; x <= footprint.MaxX; x++)
        {
            for (var y = footprint.MinY - 1; y <= footprint.MaxY - Grid.SegmentLength + 1; y++)
            {
                TryAddSegment(new GridPoint(x, y), new GridPoint(x, y + Grid.SegmentLength), segments);
            }
        }

        for (var y = footprint.MinY; y <= footprint.MaxY; y++)
        {
            for (var x = footprint.MinX - 1; x <= footprint.MaxX - Grid.SegmentLength + 1; x++)
            {
                TryAddSegment(new GridPoint(x, y), new GridPoint(x + Grid.SegmentLength, y), segments);
            }
        }

        for (var x = footprint.MinX - 1; x <= footprint.MaxX - Grid.SegmentLength + 1; x++)
        {
            for (var y = footprint.MinY - 1; y <= footprint.MaxY - Grid.SegmentLength + 1; y++)
            {
                TryAddSegment(new GridPoint(x, y), new GridPoint(x + Grid.SegmentLength, y + Grid.SegmentLength), segments);
                TryAddSegment(new GridPoint(x, y + Grid.SegmentLength), new GridPoint(x + Grid.SegmentLength, y), segments);
            }
        }

        return segments;
    }
    
    public List<Segment> FindNewSegments(GridPoint lastDot)
    {
        var segments = new List<Segment>();
        
        foreach (var direction in _directions)
        {
            for (int position = 0; position <= Grid.SegmentLength; position++)
            {
                var pt1 = lastDot + position * direction;
                var pt2 = lastDot + (position - Grid.SegmentLength) * direction;
                
                var segment = NewSegment(pt1, pt2);
                if (segment is not null)
                {
                    segments.Add(segment);
                }
            }
        }

        return segments;
    }

    public virtual void Undo(int steps = 1)
    {
        Grid.Undo(steps);
        Image.Load(Grid);
    }

    public virtual void Restart()
    {
        Undo(GetScore());
    }
    public int SvgWidth(GridFootprint footprint)
    {
        return PixelsPerUnit * (footprint.MaxX - footprint.MinX + 1);
    }

    public int SvgHeight(GridFootprint footprint)
    {
        return PixelsPerUnit * (footprint.MaxY - footprint.MinY + 1);
    }
    
    public string SvgViewBox(GridFootprint footprint)
    {
        return $"{footprint.MinX - 0.5:F1} {footprint.MinY - 0.5:F1} " +
               $"{footprint.MaxX - footprint.MinX + 1} {footprint.MaxY - footprint.MinY + 1}";
    }

    public string SvgBackground(GridFootprint footprint, bool grouped = false)
    {
        var width = footprint.MaxX - footprint.MinX + 1;
        var height = footprint.MaxY - footprint.MinY + 1;
        var minX = footprint.MinX - 0.5;
        var maxX = footprint.MaxX + 0.5;
        var minY = footprint.MinY - 0.5;
        var maxY = footprint.MaxY + 0.5;
        
        var result = $"<rect width=\"{width}\" height=\"{height}\" "
                          + $"x=\"{minX}\" y=\"{minY}\" style=\"fill:white\" />";
        
        const string gridStyle = "stroke:lightgray;stroke-width:0.1";
        for (var i = 0; i < width; i++)
        {
            var x = footprint.MinX + i;
            result += $"<line x1=\"{x}\" y1=\"{minY}\" x2=\"{x}\" y2=\"{maxY}\" style=\"{gridStyle}\" />";
        }
        for (var i = 0; i < height; i++)
        {
            var y = footprint.MinY + i;
            result += $"<line x1=\"{minX}\" y1=\"{y}\" x2=\"{maxX}\" y2=\"{y}\" style=\"{gridStyle}\" />";
        }

        return (grouped) ? "<g>" + result + "</g>" : result;
    }
    
    public string ToSvg(bool crop = false)
    {
        var footprint = (crop) ? Grid.GetFootprint() : Image.GetFootprint();

        return $"<svg width=\"{SvgWidth(footprint)}\" height=\"{SvgHeight(footprint)}\" " +
               $"viewbox=\"{SvgViewBox(footprint)}\">" +
               SvgBackground(footprint) +
               Grid.ToSvg() +
               "</svg>";
    }
}